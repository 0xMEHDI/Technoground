using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public static class DataBinderExtensions
{
    public static Dictionary<Type, Object> Table = new Dictionary<Type, Object>();

    public static void Bind<T, TGet>(this MonoBehaviour mono, TGet dependent = null, bool singleton = false) where T : class, IDataBind<TGet> where TGet : MonoBehaviour
    {
        var dependencies = mono.GetComponentsInChildren<T>(includeInactive: true);

        if (singleton && dependent == null && Table.ContainsKey(typeof(TGet)))
            dependent = Table[typeof(TGet)] as TGet;

        if (dependent == null)
            dependent = mono.GetComponentInChildren<TGet>(includeInactive: true);

        if (dependent == null)
            dependent = SceneUtils.GetComponentByScenes<TGet>(true);

        if (dependent != null)
            Table[typeof(TGet)] = dependent;

        if (dependent == null && dependencies.Length > 0)
        {
            Debug.LogError($"Could not bind {typeof(TGet)} from {mono.name} dependencies count: {dependencies.Length}  dependent found: {dependent != null}", mono);
            return;
        }

        foreach (var dependency in dependencies)
            dependency.Bind(dependent);
    }
}