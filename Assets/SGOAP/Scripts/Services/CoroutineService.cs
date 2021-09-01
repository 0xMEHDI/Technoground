using UnityEngine;

namespace SGoap
{
    public class CoroutineService : MonoBehaviour
    {
        private static CoroutineService _instance;

        public static CoroutineService Instance
        {
            get => _instance ?? new GameObject("Coroutine Service").AddComponent<CoroutineService>();
            set => _instance = value;
        }

        private void Awake()
        {
            transform.parent = null;
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
    }
}