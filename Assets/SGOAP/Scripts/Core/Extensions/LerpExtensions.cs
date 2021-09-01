using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace SGoap
{
    public static class LerpExtensions
    {
        /// <summary>
        /// An extension to lerping with a MonoBehaviour, any methods placed in on 
        /// </summary>
        public static Coroutine Lerp(Action<float> onValueChanged, float duration, System.Action OnComplete = null)
        {
            return CoroutineService.Instance.StartCoroutine(Routine());

            IEnumerator Routine()
            {
                yield return Lerp();
                OnComplete?.Invoke();
            }

            IEnumerator Lerp()
            {
                for (float i = 0; i < 1.0F; i += Time.deltaTime / duration)
                {
                    onValueChanged(i);
                    yield return null;
                }

                onValueChanged(1);
            }
        }

        public static Coroutine Lerp(this CanvasGroup group, float alpha, float duration)
        {
            if (duration == 0)
            {
                group.alpha = alpha;
                return null;
            }

            var baseAlpha = group.alpha;
            return Lerp(val => { group.alpha = Mathf.Lerp(baseAlpha, alpha, val); },
                duration);
        }

        public static Coroutine Lerp(this Graphic graphic, Color color, float duration)
        {
            var colorA = graphic.color;
            return Lerp(val => { graphic.color = Color.Lerp(colorA, color, val); },
                duration);
        }

        public static Coroutine Lerp(this Material material, Color color, float duration)
        {
            var colorA = material.color;
            return Lerp(val => { material.color = Color.Lerp(colorA, color, val); },
                duration);
        }

        public static Coroutine GoTo(this Transform transform, Vector3 position, float duration, bool lookAt = false)
        {
            var startPosition = transform.position;

            return Lerp(val =>
                {
                    if (lookAt)
                        transform.LookAt(position);

                    transform.position = Vector3.Lerp(startPosition, position, val);
                },
                duration);
        }

        public static Coroutine Push(this Transform transform, Transform target, float distance, float duration)
        {
            var pushDir = target.transform.position - transform.position;
            pushDir.Normalize();
            var pushedPosition = target.transform.position + pushDir * distance;

            return target.transform.GoTo(pushedPosition, duration);
        }

        public static Vector3 GetPushPosition(this Transform transform, Transform target, float distance)
        {
            var pushDir = target.transform.position - transform.position;
            pushDir.Normalize();
            var pushedPosition = target.transform.position + pushDir * distance;

            return pushedPosition;
        }
    }
}