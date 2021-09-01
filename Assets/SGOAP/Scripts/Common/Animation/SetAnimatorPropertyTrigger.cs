using UnityEngine;

namespace SGoap
{
    public class SetAnimatorPropertyTrigger : SetAnimatorPropertyBase<PropertyValue>
    {
        public override void SetProperty(PropertyValue property, Animator animator)
        {
            animator.SetTrigger(property.Name);
        }
    }
}