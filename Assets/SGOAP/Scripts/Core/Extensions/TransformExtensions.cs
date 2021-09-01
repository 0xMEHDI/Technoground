using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace SGoap
{
    public static class TransformExtensions
    {
        public static T GetClosestNearby<T>(this Transform transform, float radius) where T: Component
        {
            var cols = Physics.OverlapSphere(transform.position, radius);

            var list = new List<T>();
            foreach (var col in cols)
            {
                var match = col.GetComponentInParent<T>();
                if (match != null)
                {
                    list.Add(match);
                }
            }
            return list.OrderBy(t => (t.transform.position - transform.position).sqrMagnitude)
                .FirstOrDefault();
        }

        public static Transform GetClosest<T>(this Transform transform) where T : Component
        {
            var grenades = GameObject.FindObjectsOfType<T>().Where(x => x.transform != transform).Select(x => x.transform);

            return grenades.OrderBy(t => (t.position - transform.position).sqrMagnitude)
                .FirstOrDefault();
        }

        public static T GetClosestWithType<T>(this Transform transform) where T : Component
        {
            var grenades = GameObject.FindObjectsOfType<T>().Where(x => x.transform != transform);

            return grenades.OrderBy(t => (t.transform.position - transform.position).sqrMagnitude)
                .FirstOrDefault();
        }

        public static T GetClosestByInterface<T>(this Transform transform, List<T> grenades = null) where T : ITarget
        {
            grenades = grenades ?? Object.FindObjectsOfType<MonoBehaviour>().OfType<T>().Where(x => x.transform != transform).ToList();
            return grenades.OrderBy(t => (t.transform.position - transform.position).sqrMagnitude)
                .FirstOrDefault();
        }

        public static List<T> FindObjectsByInterface<T>(this Transform transform) where T : ITarget
        {
            var grenades = Object.FindObjectsOfType<MonoBehaviour>().OfType<T>().Where(x => x.transform != transform).ToList();
            return grenades;
        }

        public static Transform GetRandom<T>(this Transform transform) where T : Component
        {
            var grenades = Object.FindObjectsOfType<T>().Where(x => x.transform != transform).ToList();
            return grenades[Random.Range(0, grenades.Count)].transform;
        }

        public static Transform GetRandomByInterface<T>(this Transform transform) where T : ITarget
        {
            var grenades = Object.FindObjectsOfType<MonoBehaviour>().OfType<T>().Where(x => x.transform != transform).ToList();
            return grenades[Random.Range(0, grenades.Count)].transform;
        }
    }
}