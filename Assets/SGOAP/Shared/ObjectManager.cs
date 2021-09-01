using System.Collections.Generic;
using UnityEngine;

namespace SGoap.Services
{
    // Same as an ObjectManager but a workaround to the current KdTree not accepting interfaces
    public static class TargetManager
    {
        public static KdTree<Transform> KdTree = new KdTree<Transform>();
        private static Dictionary<Transform, ITarget> _map = new Dictionary<Transform, ITarget>();

        public static void Add(ITarget target)
        {
            KdTree.Add(target.transform);
            _map.Add(target.transform, target);
        }

        public static void Remove(ITarget instance)
        {
            KdTree.RemoveAll(x => x.gameObject == instance.transform.gameObject);
            _map.Remove(instance.transform);
        }

        public static void Clear()
        {
            KdTree.Clear();
            _map.Clear();
        }

        public static ITarget FindClosest(Vector3 position)
        {
            return _map[KdTree.FindClosest(position)];
        }
    }

    public static class ObjectManager<T> where T : Component
    {
        public static List<T> List = new List<T>();
        public static KdTree<T> KdTree = new KdTree<T>();

        public static void Add(T obj)
        {
            List.Add(obj);
            KdTree.Add(obj);
        }

        public static void AddRange(T[] objs)
        {
            List.AddRange(objs);
            KdTree.AddAll(List);
        }

        public static void Remove(T instance)
        {
            List.Remove(instance);
            KdTree.RemoveAll(x => x.gameObject == instance.gameObject);
        }

        public static T FindClosest(Vector3 position)
        {
            return KdTree.FindClosest(position);
        }

        public static void Clear()
        {
            List.Clear();
            KdTree.Clear();
        }

        public static int Count => List.Count;
    }

}
