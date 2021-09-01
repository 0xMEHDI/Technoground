using UnityEngine;

namespace SGoap
{
    public class SetAnimatorPropertyBool : SetAnimatorPropertyBase<BoolProperty>
    {
        public override void SetProperty(BoolProperty property, Animator animator)
        {
            animator.SetBool(property.Name, property.Value);
        }
    }
}