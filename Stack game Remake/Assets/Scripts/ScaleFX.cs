using System.Collections;
using UnityEngine;

namespace CoolBears
{
    public class ScaleFX : MonoBehaviour
    {
        #region Variables
        private static Vector3 normalScale = new Vector3(0.2f, 0.27f, 1f);

        [SerializeField] private SpriteRenderer spriteRenderer = null;

        private static int fxStep = 20;
        private static float fxDuration = 1.5f;

        #endregion

        #region Builtin Methods
        private void OnEnable()
        {
            if (spriteRenderer)
            {
                spriteRenderer.color = new Color(
                                 spriteRenderer.color.r,
                                 spriteRenderer.color.g,
                                 spriteRenderer.color.b,
                                 1f);
                
                spriteRenderer.enabled = false;
            }
            transform.localScale = normalScale;
            MovingCube.CurrentCube.OnScaleIncreaseEvent += MovingCube_OnScaleIncreaseEvent;
        }

        private void OnDestroy()
        {
            spriteRenderer.enabled = false;

            MovingCube.CurrentCube.OnScaleIncreaseEvent -= MovingCube_OnScaleIncreaseEvent;
        }

        private void OnDisable()
        {
            spriteRenderer.enabled = false;

            MovingCube.CurrentCube.OnScaleIncreaseEvent -= MovingCube_OnScaleIncreaseEvent;
        }

        #endregion

        #region Custom Methods
        private void MovingCube_OnScaleIncreaseEvent()
        {
            spriteRenderer.enabled = true;
            StopAllCoroutines();
            StartCoroutine(IEnumScaleFX());
        }

        private IEnumerator IEnumScaleFX()
        {
            int i = 0;
            while (i < fxStep)
            {
                if (i > 0)
                {
                    transform.localScale = normalScale * i;

                    if (spriteRenderer)
                        spriteRenderer.color = new Color(
                            spriteRenderer.color.r,
                            spriteRenderer.color.g,
                            spriteRenderer.color.b,
                            a: Mathf.Clamp01(1f / i));
                }
                yield return new WaitForSeconds(Time.fixedDeltaTime * fxDuration);
                i++;
            }

            spriteRenderer.enabled = false;
        }
        #endregion
    }
}