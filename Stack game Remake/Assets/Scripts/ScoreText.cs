using TMPro;
using UnityEngine;

namespace CoolBears
{
    public class ScoreText : Singleton<ScoreText>
    {
        #region Variables
        private int score = 0;

        public int Score => score;

        [SerializeField] private TextMeshProUGUI textMesh = null;
        [SerializeField] private TextMeshProUGUI highScoreMesh = null;
        #endregion

        #region Builtin Methods
        private void Start()
        {
            textMesh = GetComponent<TextMeshProUGUI>();

            int highScore = PlayerPrefs.GetInt(GameManager.HighScoreKey);

            textMesh.text = "Click to Play";

            if (highScore > 0)
            {
                highScoreMesh.gameObject.SetActive(true);
                highScoreMesh.text = $"High Score: {highScore}";
            }
            else
            {
                highScoreMesh.gameObject.SetActive(false);
            }
           

            GameManager.OnCubeSpawned += GameManager_OnCubeSpawned;
        }

        private void OnDestroy()
        {
            GameManager.OnCubeSpawned -= GameManager_OnCubeSpawned;
        }

        #endregion

        #region Custom Methods
        private void GameManager_OnCubeSpawned(int count)
        {
            highScoreMesh.gameObject.SetActive(false);

            score += count;

            textMesh.text = $"Score: {score}";
        }

        #endregion
    }
}