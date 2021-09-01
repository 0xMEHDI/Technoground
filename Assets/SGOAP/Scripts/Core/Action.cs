using System.Collections.Generic;
using System.Diagnostics;

namespace SGoap
{
    using UnityEngine;

    /// <summary>
    /// The base action to all GOAP Agents.
    /// </summary>
    public abstract class Action : MonoBehaviour, IContext, IDataBind<Agent>
    {
        public string Name = "Action";

        [Tooltip("A high cost will have less priority in the planner where a lower cost will be more favourable.")]
        public float Cost = 1;

        public CostEvaluator CostEvaluator;
        public UsableEvaluator UsableEvaluator;
        public UsableEvaluator AbortEvaluator;

        [Tooltip("All preconditions must be met for the action to begin. For a precondition to be met, another action must have the same state as the effect")]
        public State[] PreConditions;
        [Tooltip("The after effects is only used in the planner, simply for chaining actions and does not modify the world state or the agent's state.")]
        [Effect]
        public State[] AfterEffects;
        public States States;

        public System.Action OnPrePerform, OnPostPerform, OnPerform, OnPerformFailed;

        [HideInInspector]
        public int CostEvaluationType, AchievableEvaluationType, AbortEvaluatorType;

        public Stopwatch Stopwatch = new Stopwatch();
        public CoolDown Cooldown = new CoolDown();

        public float TimeElapsed => (float)Stopwatch.Elapsed.TotalSeconds;
        public bool Running { get; set; }
        public virtual float CooldownTime => 0;
        public virtual bool TrackStopWatch => true;

        /// <summary>
        /// By default, actions that fails IsUsable are not included in the plan but you may want to include it still for some behaviours.
        /// </summary>
        public virtual bool AlwaysIncludeInPlan => false;

        /// <summary>
        /// Returning true will include this action in the graph.
        /// </summary>
        public virtual bool IsUsable() => AchievableEvaluationType == 0 ? !Cooldown.Active : !Cooldown.Active && UsableEvaluator.Evaluate(this);

        /// <summary>
        /// This is used at the same time as Preconditions, passing in the current state of the node in the graph.
        /// </summary>
        public virtual bool IsProceduralPreconditionMet(Dictionary<string, float> currentState) => true;

        /// <summary>
        /// This is called during the same time AfterEffects is added to the current node. Update the currentState to chain effects by code.
        /// </summary>
        public virtual void ProceduralEffect(Dictionary<string, float> currentState) { }
        public virtual void DynamicallyEvaluateCost()
        {
            if (CostEvaluationType == 1)
                Cost = CostEvaluator.Evaluate(this);
        }

        /// <summary>
        /// This is a called for each node in the planning graph.
        /// Override this method to set costs based on a future state inside the planner.
        /// </summary>
        /// <param name="currentState"></param>
        public virtual float DynamicallyEvaluateCost(Dictionary<string, float> currentState) => 0;

        /// <summary>
        /// Return true if the action has all the requirements to start. This happens after IsAchieveable().
        /// </summary>
        public bool IsPreconditionsMet(Dictionary<string, float> preConditions) => GOAPUtils.IsStateMatch(PreConditions, preConditions);

        /// <summary>
        /// Returns success by default. Override this method and return Running for an active that work over multiple frames.
        /// </summary>
        public virtual EActionStatus Perform() => EActionStatus.Success;

        /// <summary>
        /// Returns true if the action wants to be aborted. Some actions may run over time and not end until a certain situation.
        /// </summary>
        public bool ShouldAbort() => AbortEvaluator != null && AbortEvaluator.Evaluate(this);

        public virtual bool CanAbort() => true;

        public virtual void OnFailed() { }
        public abstract bool PrePerform();
        public abstract bool PostPerform();
        public T Get<T>() where T : class => this as T;

        public void Bind(Agent agent)
        {
            States = agent.States;
        }
    }
}