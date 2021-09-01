using System.Collections;
using SGoap;
using UnityEngine;

namespace Games.Common
{
    public class TimedAction : CoroutineAction
    {
        public float Duration = 1;

        public override IEnumerator PerformRoutine()
        {
            yield return new WaitForSeconds(Duration);
        }
    }
}

