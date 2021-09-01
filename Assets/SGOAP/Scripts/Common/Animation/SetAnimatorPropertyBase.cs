using UnityEngine;

namespace SGoap
{
    public abstract class SetAnimatorPropertyBase<T> : MonoBehaviour
    {
        public CoroutineAction Action;
        public T Property;

        private void OnValidate()
        {
            Action = GetComponent<CoroutineAction>();
        }

        private void Awake()
        {
            Action.OnFirstPerform += OnActionPerform;
        }

        private void OnDestroy()
        {
            Action.OnFirstPerform -= OnActionPerform;
        }

        public void OnActionPerform()
        {
            SetProperty(Property, Action.AgentData.Animator);
        }

        public abstract void SetProperty(T property, Animator animator);
    }
}