using UnityEngine;

namespace SGoap
{
    public class SetAnimatorPropertyInt : SetAnimatorPropertyBase<IntProperty>
    {
        public override void SetProperty(IntProperty property, Animator animator)
        {
            animator.SetInteger(property.Name, property.Value);
        }
    }
}