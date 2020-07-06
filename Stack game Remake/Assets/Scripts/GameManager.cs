using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

namespace CoolBears
{
    public class GameManager : MonoBehaviour
    {
        #region Variables
        public const string HighScoreKey = "HighScore";
        public static event Action<int> OnCubeSpawned = delegate { };

        [SerializeField] private CubeSpawner[] cubeSpawners = null;
        private CubeSpawner currentSpawner;
        private int spawnerIndex;

        public CinemachineVirtualCamera VirtualCamera;
        #endregion

        #region Builtin Methods
        private void Update()
        {
            if(Input.GetButtonDown("Fire1"))
            {
                if (MovingCube.CurrentCube) MovingCube.CurrentCube.Stop(VirtualCamera);

                spawnerIndex = spawnerIndex == 0 ? 1 : 0;
                currentSpawner = cubeSpawners[spawnerIndex];

                currentSpawner.SpawnCube();

                OnCubeSpawned(MovingCube.ScoreCount);
            }
        }
        #endregion

        #region Custom Methods
        public static void GameOver()
        {
            int highScore = ScoreText.Instance.Score;

            int currentHighScore = PlayerPrefs.GetInt(HighScoreKey);

            if (highScore > currentHighScore)
                PlayerPrefs.SetInt(HighScoreKey, highScore);

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        #endregion
    }
}