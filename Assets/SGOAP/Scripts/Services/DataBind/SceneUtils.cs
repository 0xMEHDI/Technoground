using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneUtils
{
    public static T GetComponentByScenes<T>(bool includeInactive = false) where T : Component
    {
        T instance = null;
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            instance = GetComponent<T>(scene, includeInactive);

            if (instance != null)
                break;
        }

        return instance;
    }

    public static T GetComponent<T>(Scene scene, bool includeInactive = false) where T : Component
    {
        T instance = null;
        var rootObjects = scene.GetRootGameObjects();

        foreach (var rootObject in rootObjects)
        {
            instance = rootObject.GetComponentInChildren<T>(includeInactive: includeInactive);
            if (instance != null)
                break;
        }

        return instance;
    }
}