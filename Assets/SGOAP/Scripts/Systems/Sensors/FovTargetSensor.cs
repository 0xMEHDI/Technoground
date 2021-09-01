using UnityEngine;

namespace SGoap
{
    public class FovTargetSensor : MonoBehaviour, IDataBind<AgentBasicData>
    {
        [Range(0, 360)]
        public float Angle = 60;
        public float Radius = 20;

        private AgentBasicData _agentData;

        public ITarget Target { get; private set; }
        public bool HasTarget => Target != null;

        public LayerMask Layer;

        private void Update()
        {
            if (Target != null)
                return;

            FindTargetWithinSight();
        }

        public void FindTargetWithinSight()
        {
            var cols = Physics.OverlapSphere(_agentData.Position, Radius, Layer);
            foreach (var col in cols)
            {
                var target = col.GetComponentInParent<ITarget>();
                if (target == null)
                    continue;

                if (ReferenceEquals(target, _agentData.Agent as ITarget))
                    continue;

                if (WithinSight(target.transform))
                {
                    Target = target;
                    SetTarget(Target);
                    break;
                }
            }
        }

        public void SetTarget(ITarget target)
        {
            Target = target;

            _agentData.Target = target.transform;
        }

        public void Bind(AgentBasicData data)
        {
            _agentData = data;
        }

        private void OnDrawGizmos()
        {
            Vector3 posL = GetPosition(-(Angle / 2), 1000.0F, transform.position);
            Vector3 posR = GetPosition(Angle / 2, 1000.0F, transform.position);

            Debug.DrawRay(transform.position, transform.forward * 1000.0F, Color.red);
            Debug.DrawLine(transform.position, posL, Color.blue);
            Debug.DrawLine(transform.position, posR, Color.blue);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, Radius);
        }

        public bool WithinSight(Transform sampleTransform)
        {
            return AngleToObject(sampleTransform) <= Angle / 2;
        }

        public float AngleToObject(Transform sampleTransform)
        {
            float angleToObject = 0;

            Vector3 tarPosition = sampleTransform.position;
            tarPosition.y = transform.position.y;

            Vector3 targetDir = tarPosition - transform.position;
            Vector3 forward = transform.forward;

            angleToObject = Vector3.SignedAngle(targetDir, forward, Vector3.up);
            angleToObject = Mathf.Abs(angleToObject);

            return angleToObject;
        }

        //Calculate Position
        public Vector3 GetPosition(float angle, float dist, Vector3 root)
        {
            var transformAngle = transform.eulerAngles.y;

            var r = Mathf.Deg2Rad * (angle + 90 - transformAngle);
            var x = Mathf.Cos(r);
            var y = Mathf.Sin(r);

            Vector3 displacement = new Vector3(x, 0, y) * dist;
            Vector3 pos = root + displacement;
            return pos;
        }
    }
}