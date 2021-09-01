using System.Collections;
using UnityEngine;

namespace SGoap
{
    public class CoroutineActionExample : CoroutineAction
    {
        public override IEnumerator PerformRoutine()
        {
            Debug.Log("Example Started");
            yield return new WaitForSeconds(2);
            Debug.Log("2 Seconds In");
            yield return new WaitForSeconds(4);
            Debug.Log("Done!");
        }
    }
}