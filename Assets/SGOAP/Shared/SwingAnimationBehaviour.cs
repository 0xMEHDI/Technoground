using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingAnimationBehaviour : StateMachineBehaviour
{
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var time = animator.GetFloat("Time");
        time = Mathf.Clamp(time, 0, 0.999f);
        animator.Play(stateInfo.fullPathHash, layerIndex, time);
    }
}
