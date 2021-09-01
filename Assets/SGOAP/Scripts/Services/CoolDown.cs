using System;
using System.Collections;
using UnityEngine;

namespace SGoap
{
    public class CoolDown
    {
        public bool Active { get; set; }
        public bool Done { get; set; }

        private float _startTime;
        private float _targetDuration;

        public float TimeElapsed => Time.time - _startTime;
        public float TimeRemaining => _targetDuration - TimeElapsed;
        public float Progress { get; private set; }

        private Coroutine Coroutine;

        public void ResetTime()
        {
            _startTime = Time.time;
        }

        public Coroutine Run(float duration, System.Action onComplete = null, System.Action onUpdate = null, bool skipWhenDurationIsZero = true)
        {
            if (Active)
                return null;

            if (skipWhenDurationIsZero && duration == 0)
                return null;

            return Coroutine = CoroutineService.Instance.StartCoroutine(Routine());

            IEnumerator Routine()
            {
                Active = true;
                Done = false;

                _targetDuration = duration;
                _startTime = Time.time;
                while (Time.time - _startTime < _targetDuration)
                {
                    Progress = TimeElapsed / _targetDuration;

                    onUpdate?.Invoke();
                    yield return null;
                }

                Active = false;
                Progress = 1;
                Done = true;

                onUpdate?.Invoke();
                onComplete?.Invoke();
            }
        }


        /// <summary>
        /// Run until the predicate is satisfied.
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="onComplete"></param>
        /// <param name="onUpdate"></param>
        /// <returns></returns>
        public Coroutine RunUntil(Func<bool> predicate, System.Action onComplete = null, System.Action onUpdate = null)
        {
            if (Active)
                return null;

            if (predicate.Invoke())
                return null;

            return Coroutine = CoroutineService.Instance.StartCoroutine(Routine());

            IEnumerator Routine()
            {
                Active = true;
                Done = false;

                _startTime = Time.time;

                while (Active)
                {
                    if (predicate.Invoke())
                        break;

                    onUpdate?.Invoke();
                    yield return null;

                }

                Active = false;
                Progress = 1;
                Done = true;

                onUpdate?.Invoke();
                onComplete?.Invoke();
            }
        }

        public void Stop()
        {
            Done = false;
            _targetDuration = 0;
            Active = false;
            _startTime = Time.time;

            if (Coroutine != null)
                CoroutineService.Instance.StopCoroutine(Coroutine);
        }
    }

}