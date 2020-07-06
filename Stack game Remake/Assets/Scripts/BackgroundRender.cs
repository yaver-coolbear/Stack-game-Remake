using UnityEngine;

namespace CoolBears
{
    public class BackgroundRender : Singleton<BackgroundRender>
    {
        [SerializeField] private Color[] gameColors = new Color[4];

        [SerializeField] private Transform gradient = null;

        private Texture2D _tex;
        private Renderer _rend;

        private void Start()
        {
            gameColors[0] = UnityEngine.Random.ColorHSV(0f, 1f, 0.7f, 1f, 0.8f, 1f);
            gameColors[1] = UnityEngine.Random.ColorHSV(0.5f, 0.8f, 0.5f, 1f, 0.55f, 1f);
            gameColors[2] = UnityEngine.Random.ColorHSV(0f, 0.75f, 0f, 0.75f, 0f, 1f);
            gameColors[3] = UnityEngine.Random.ColorHSV(0f, 0.5f, 0f, 0.5f, 0f, 0.5f);

            gradient = this.transform;
            _rend = this.GetComponent<Renderer>();

            //create new texture, width == amount of colors
            _tex = new Texture2D(4, 1);

            //Set each pixel to specified color (StarGradient is function below)
            _tex.SetPixels32(StarGradient(
                gameColors[0],
                gameColors[1],
                gameColors[2],
                gameColors[3]));

            _tex.filterMode = FilterMode.Trilinear;
            _tex.wrapMode = TextureWrapMode.Clamp;
            _tex.Apply();

            //Pass _tex to the material
            _rend.material.SetTexture("_MainTex", _tex);
        }

        private Color32 Lerp4(Color32 a, Color32 b, Color32 c, Color32 d, float t)
        {
            if (t < 0.33f) return Color.Lerp(a, b, t / 0.33f);
            else if (t < 0.66f) return Color.Lerp(b, c, (t - 0.33f) / 0.33f);
            else return Color.Lerp(c, d, (t - 0.66f) / 0.66f);
        }

        public void ColorMesh(Material mat, int count)
        {
            float f = Mathf.PingPong(count * 0.05f, 1);
            Color32 color = Lerp4(gameColors[0], gameColors[1], gameColors[2], gameColors[3], f);
            mat.SetColor("_Color", color);

            float xRot = Mathf.Lerp(75f, -75f, f);

            gradient.localEulerAngles = new Vector3(xRot, gradient.localEulerAngles.y, gradient.localEulerAngles.z);
        }

        //Returns a Color32 array of specified colors
        private Color32[] StarGradient(Color32 a, Color32 b, Color32 c, Color32 d)
        {
            Color32[] _colors = new Color32[4];
            _colors[0] = a;
            _colors[1] = b;
            _colors[2] = c;
            _colors[3] = d;
            return _colors;
        }
    }
}