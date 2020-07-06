using UnityEngine;

namespace CoolBears
{
    public class CubeSpawner : MonoBehaviour
    {
        #region Variables
        [SerializeField] private MovingCube cubePrefab = null;

        [SerializeField] private MoveDirection moveDirection = MoveDirection.X;

        #endregion

        #region Custom Methods
        public void SpawnCube()
        {
            var cube = Instantiate(cubePrefab);

            if (MovingCube.LastCube != null && MovingCube.LastCube.gameObject != GameObject.Find("Platform"))
            {
                float x = moveDirection == MoveDirection.X ? 
                    transform.position.x : 
                    MovingCube.LastCube.transform.position.x;

                float z = moveDirection == MoveDirection.Z ?
                    transform.position.z :
                    MovingCube.LastCube.transform.position.z;


                cube.transform.position = new Vector3(x,
                    MovingCube.LastCube.transform.position.y + cubePrefab.transform.localScale.y, z);

                cube.SetReplayPlace(x, -x, z, -z);
            }
            else
            {
                cube.transform.position = transform.position;

                float x = transform.position.x;
                float z = transform.position.z;

                cube.SetReplayPlace(x, -x, z, -z);
            }

            cube.MoveDirection = moveDirection;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, cubePrefab.transform.localScale);
        }
        #endregion
    }
}