using UnityEngine;

namespace SGoap
{
    public class SetAnimatorPropertyFloat : SetAnimatorPropertyBase<FloatProperty>
    {
        public override void SetProperty(FloatProperty property, Animator animator)
        {
            animator.SetFloat(property.Name, property.Value);
        }
    }
}