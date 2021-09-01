using SGoap;

namespace SGOAP.Examples.CombatChain
{
    /// <summary>
    /// Disable PlannerSettings.RunOnLateUpdate and instead call FindPlan() when you need the plan.
    /// </summary>
    public class TurnBaseAgent : Agent
    {
        // Find a plan and assign it to ActionQueue
        public override void FindPlan()
        {
            base.FindPlan();
        }

        // See if the goal has been met.
        public override void EvaluationGoal()
        {
            base.EvaluationGoal();
        }

        // Run the Plan
        public override bool ExecutePlan()
        {
            return base.ExecutePlan();
        }
    }
}