using UnityEngine;
using UnityEngine.AI;
using Debug = UnityEngine.Debug;

namespace SGoap
{
    public class NavMeshAction : BasicAction
    {
        public NavMeshAgent Agent;
        public GameObject Target;

        public float StopDistance = 2;
        public float Duration = 0;
        
        public override bool TrackStopWatch => false;

        public override bool PrePerform()
        {
            if (Target == null)
                return false;

            Agent.SetDestination(Target.transform.position);

            return base.PrePerform();
        }

        public override EActionStatus Perform()
        {
            if (Target == null)
            {
                Debug.LogWarning("Aborting as there is no target");
                return EActionStatus.Failed;
            }

            Agent.SetDestination(Target.transform.position);

            if (HasBegun())
                Stopwatch.Start();

            if (Stopwatch.IsRunning && Stopwatch.Elapsed.TotalSeconds >= Duration)
                return EActionStatus.Success;

            return EActionStatus.Running;
        }

        public bool HasBegun()
        {
            var dist = Vector3.Distance(Agent.transform.position, Target.transform.position);
            return dist <= StopDistance + 0.2f && !Stopwatch.IsRunning;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, StopDistance);
        }
    }
}