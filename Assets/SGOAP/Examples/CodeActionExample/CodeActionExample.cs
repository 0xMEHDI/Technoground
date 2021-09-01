using System.Collections.Generic;
using SGoap;

namespace SGOAP.Examples
{
    /// <summary>
    /// This shows an example on how to build your own precondition and effects as well as show all common actions to override.
    /// </summary>
    public class CodeActionExample : BasicAction
    {
        #region Procedural Example

        public override float CooldownTime => 2;

        // Freezes the Agent's planning.
        public override float StaggerTime => 1;

        // Decide if this action is to be even part of consideration for the plan. i.e return false will not even check its cost/precondition.
        // Filtering your actions out here will highly improve performance.
        public override bool IsUsable()
        {
            var hasWeapon = true;

            // The base IsUsable checks if CoolDown is running so I suggest keeping it.
            return base.IsUsable() && hasWeapon;
        }

        public override void DynamicallyEvaluateCost()
        {
            // If you keep this, you can still use the evaluator. Generally, I do not suggest mixing the two though as it introduces a layer of complexity.
            base.DynamicallyEvaluateCost();

            // Assuming you have a target and a distance to the target.
            var distance = 10;
            // Dynamically determine how expensive this action is to the Agent.
            Cost = distance;
        }


        // Inject your own precondition here. 
        public override bool IsProceduralPreconditionMet(Dictionary<string, float> currentState)
        {
            // Create a custom precondition
            var state = new KeyValuePair<string, float>("ProceduralPrecondition", 1);
            if (!currentState.ContainsKey(state.Key))
                return false;

            // Add this to still use the precondition list.
            return base.IsProceduralPreconditionMet(currentState);
        }

        // Inject your own procedural effect
        public override void ProceduralEffect(Dictionary<string, float> currentState)
        {
            var state = new KeyValuePair<string, float>("ProceduralEffect", 1);
            currentState.Add(state.Key, state.Value);

            base.ProceduralEffect(currentState);
        }

        #endregion

        #region Action Overrides

        // This is called before Perform and if you return false, it'll not execute perform.
        public override bool PrePerform()
        {
            // By default if the action is cooling down, this returns false.
            return base.PrePerform();
        }

        public override EActionStatus Perform()
        {
            return EActionStatus.Success;
        }

        public override bool PostPerform()
        {
            // By default, when post perform happens, the Agent is staggered and has a cool down. You can override.
            return base.PostPerform();
        }

        // Decide what your Agent will do when the action fails, i.e cancel an animation or play an Angry animation!
        public override void OnFailed()
        {
            base.OnFailed();
        }

        // If PlannerSettings.CanAbortPlans is set to true, returning true here will force a replan, generally I don't reccomend using this and instead return Failed in the action status.
        public override bool CanAbort()
        {
            return base.CanAbort();
        }

        #endregion
    }
}