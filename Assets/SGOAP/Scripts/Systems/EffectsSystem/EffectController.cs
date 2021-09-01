using System.Collections;
using UnityEngine;

namespace SGoap
{
    public class EffectController : MonoBehaviour
    {
        public GameObject DashEffect;

        public void PlayDash(float duration)
        {
            CoroutineService.Instance.StartCoroutine(Routine());

            IEnumerator Routine()
            {
                DashEffect.SetActive(true);
                yield return new WaitForSeconds(duration);
                DashEffect.SetActive(false);
            }
        }
    }
}