using UnityEngine;

namespace SGoap
{
    [ExecuteInEditMode]
    public class AngleZone : MonoBehaviour
    {
        [Range(0, 360)] public float Angle = 45;
        public float Offset = 0;

        void Update()
        {
            Vector3 posL = GetPosition(-(Angle / 2), 1000.0F, transform.position);
            Vector3 posR = GetPosition(Angle / 2, 1000.0F, transform.position);

            Debug.DrawRay(transform.position, transform.forward * 1000.0F, Color.red);
            Debug.DrawLine(transform.position, posL, Color.blue);
            Debug.DrawLine(transform.position, posR, Color.blue);
        }

        public bool IsObjectInZone(Transform sampleTransform)
        {
            if (AngleToObject(sampleTransform) <= Angle / 2)
                return true;
            else
                return false;
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
            float transformAngle = transform.localEulerAngles.y;

            float r = Mathf.Deg2Rad * (angle + Offset - transformAngle);
            float x = Mathf.Cos(r);
            float y = Mathf.Sin(r);

            Vector3 displacement = new Vector3(x, 0, y) * dist;
            Vector3 pos = root + displacement;
            return pos;
        }
    }
}