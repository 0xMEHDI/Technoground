using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SGoap
{
    [Serializable]
    public class CoroutineActionData
    {
        public float MinimumRuntime = 0.5f;

        [MinMax(0, 15)]
        public RangeValue CooldownRangeValue;

        [MinMax(0, 15)]
        public RangeValue StaggerRangeValue = new RangeValue(0, 0);

        public bool StopOnFailed = true;
        public bool RunCooldownOnFailed = true;
    }

    public abstract class CoroutineAction : BasicAction
    {
        public CoroutineActionData CoroutineData;

        public System.Action OnFirstPerform;

        public override float CooldownTime => CoroutineData.CooldownRangeValue.GetRandomValue();
        public override float StaggerTime => CoroutineData.StaggerRangeValue.GetRandomValue();

        // When Agent planner can abort, this means a coroutine can get interrupted which we don't want.
        public override bool CanAbort() => false;

        public EActionStatus Status { get; set; }

        private Coroutine _coroutine;

        public override bool PrePerform()
        {
            if (!base.PrePerform())
                return false;

            if (_coroutine != null)
                StopCoroutine(_coroutine);

            _coroutine = StartCoroutine(Routine());

            return true;

            IEnumerator Routine()
            {
                Status = EActionStatus.Running;

                OnFirstPerform?.Invoke();
                yield return PerformRoutine();
                Status = EActionStatus.Success;
            }
        }

        public override EActionStatus Perform()
        {
            if (TimeElapsed < CoroutineData.MinimumRuntime)
                return EActionStatus.Running;

            return Status;
        }

        public override void OnFailed()
        {
            if (CoroutineData.RunCooldownOnFailed)
                Cooldown.Run(CooldownTime);

            if (CoroutineData.StopOnFailed)
                StopCoroutine(_coroutine);

            base.OnFailed();
        }

        public abstract IEnumerator PerformRoutine();
    }
}
