using Cinemachine;
using System;
using UnityEngine;

namespace CoolBears
{
    public class MovingCube : MonoBehaviour
    {
        #region Variables
        public event Action OnScaleIncreaseEvent = delegate { };
        public static MovingCube CurrentCube { get; private set; }
        public static MovingCube LastCube { get; private set; }
        public MoveDirection MoveDirection { get; set; }

        private CubeMoveTo movePlace = CubeMoveTo.Forward;

        [SerializeField] private float moveSpeed = 5f;

        private const int maxComboCount = 3;
        private const float scaleFactor = 0.15f;

        private static int comboCount;

        public static int ScoreCount = 1;

        private float xStartValue;
        private float xEndValue;
        private float zStartValue;
        private float zEndValue;

        #endregion

        #region Builtin Methods

        private void OnBecameVisible()
        {
            if (BackgroundRender.Instance)
                BackgroundRender.Instance.ColorMesh(GetComponent<MeshRenderer>().material, ScoreText.Instance.Score);
        }

        private void OnEnable()
        {
            if (!LastCube)
            {
                if (GameObject.Find("Platform"))
                    LastCube = GameObject.Find("Platform").GetComponent<MovingCube>();
            }

            CurrentCube = this;

            if (LastCube)
                transform.localScale = new Vector3(LastCube.transform.localScale.x, transform.localScale.y, LastCube.transform.localScale.z);
        }

        private void Update()
        {
            Movement();
        }

        private void OnBecameInvisible()
        {
            Destroy(this.gameObject);
        }

        #endregion

        #region Custom Methods
        private void Movement()
        {
            if (movePlace == CubeMoveTo.Forward)
            {
                if (MoveDirection == MoveDirection.Z)
                {
                    transform.position += transform.forward * Time.deltaTime * moveSpeed;

                    if (transform.position.z >= zEndValue) movePlace = CubeMoveTo.Back;
                }
                else
                {
                    transform.position += transform.right * Time.deltaTime * moveSpeed;

                    if (transform.position.x >= xEndValue) movePlace = CubeMoveTo.Back;
                }
            }
            else
            {
                if (MoveDirection == MoveDirection.Z)
                {
                    transform.position -= transform.forward * Time.deltaTime * moveSpeed;

                    if (transform.position.z <= zStartValue) movePlace = CubeMoveTo.Forward;
                }
                else
                {
                    transform.position -= transform.right * Time.deltaTime * moveSpeed;

                    if (transform.position.x <= xStartValue) movePlace = CubeMoveTo.Forward;
                }
            }
        }

        internal void Stop(CinemachineVirtualCamera _camera)
        {
            moveSpeed = 0;

            float hangover = GetHangover();

            float max = MoveDirection == MoveDirection.Z ? 
                LastCube.transform.localScale.z : LastCube.transform.localScale.x;

            if (Mathf.Abs(hangover) >= max)
            {
                LastCube = null;
                CurrentCube = null;

                GameManager.GameOver();
                return;
            }

            float direction = hangover > 0 ? 1f : -1f;

            if (Mathf.Abs(hangover) >= 0.06f)
            {
                if (MoveDirection == MoveDirection.Z)
                    SplitCubeOnZ(hangover, direction);
                else
                    SplitCubeOnX(hangover, direction);

                comboCount = 0;
                ScoreCount = 1;
            }
            else
            {
                transform.position = new Vector3(
                    LastCube.transform.position.x,
                             transform.position.y,
                     LastCube.transform.position.z);

                comboCount++;

                if (comboCount >= maxComboCount)
                {
                    IncreaseScale();
                }
            }

            _camera.Follow = LastCube.transform;
            _camera.LookAt = LastCube.transform;

            LastCube = this;
        }

        private void IncreaseScale()
        {
            if(transform.localScale.x < 1)
            {
                float x = transform.localScale.x;
                transform.localScale = new Vector3(x += scaleFactor, transform.localScale.y, transform.localScale.z);
            }

            if(transform.localScale.z < 1)
            {
                float z = transform.localScale.z;
                transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, z += scaleFactor);
            }

            ScoreCount = 2;
            OnScaleIncreaseEvent.Invoke();
        }

        internal void SetReplayPlace(float _xStartValue, float _xEndValue, float _zStartValue, float _zEndValue)
        {
            xStartValue = _xStartValue;
            xEndValue = _xEndValue;
            zStartValue = _zStartValue;
            zEndValue = _zEndValue;
        }

        private float GetHangover()
        {
            if (MoveDirection == MoveDirection.Z)
                return transform.position.z - LastCube.transform.position.z;
            else
                return transform.position.x - LastCube.transform.position.x;
        }

        private void SplitCubeOnX(float hangover, float direction)
        {
            float newSizeX = LastCube.transform.localScale.x - Mathf.Abs(hangover);
            float fallingBlockSize = transform.localScale.x - newSizeX;

            float newXPosition = LastCube.transform.position.x + (hangover / 2);

            transform.localScale = new Vector3(newSizeX, transform.localScale.y, transform.localScale.z);
            transform.position = new Vector3(newXPosition, transform.position.y, transform.position.z);

            float cubeEdge = transform.position.x + (newSizeX * 0.5f * direction);
            float fallingBlockXPosition = cubeEdge + (fallingBlockSize * 0.5f * direction);

            SpawnDropCube(fallingBlockXPosition, fallingBlockSize);
        }

        private void SplitCubeOnZ(float hangover, float direction)
        {
            float newSizeZ = LastCube.transform.localScale.z - Mathf.Abs(hangover);
            float fallingBlockSize = transform.localScale.z - newSizeZ;

            float newZPosition = LastCube.transform.position.z + (hangover / 2);

            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, newSizeZ);
            transform.position = new Vector3(transform.position.x, transform.position.y, newZPosition);

            float cubeEdge = transform.position.z + (newSizeZ * 0.5f * direction);
            float fallingBlockZPosition = cubeEdge + (fallingBlockSize * 0.5f * direction);

            SpawnDropCube(fallingBlockZPosition, fallingBlockSize);
        }

        private void SpawnDropCube(float fallingBlockPosition, float blockSize)
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

            if (MoveDirection == MoveDirection.Z)
            {
                cube.transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, blockSize);

                cube.transform.position = new Vector3(transform.position.x, transform.position.y, fallingBlockPosition);
            }
            else
            {
                cube.transform.localScale = new Vector3(blockSize, transform.localScale.y, transform.localScale.z);

                cube.transform.position = new Vector3(fallingBlockPosition, transform.position.y, transform.position.z);
            }

            Color thisColor = GetComponent<Renderer>().material.color;

            cube.GetComponent<Renderer>().material.color = thisColor;

            cube.AddComponent<Rigidbody>();

            Destroy(cube.gameObject, 1f);
        }
        #endregion
    }
}