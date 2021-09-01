using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

namespace SGoap
{
    public class Agent : MonoBehaviour
    {
        public List<Goal> Goals;
        public PlannerSettings PlannerSettings;

        public States States = new States();
        public Queue<Action> StoredActionQueue = new Queue<Action>();
        private float _planStartTime;
        private bool _replan = true;
        private List<Goal> _orderedGoals;

        public Goal CurrentGoal { get; private set; }
        public Action CurrentAction { get; private set; }
        public List<Action> Actions { get; private set; }
        public Queue<Action> ActionQueue { get; private set; }
        public bool Initialized { get; private set; }

        public Planner Planner { get; } = new Planner();

        public virtual void Start()
        {
            var agentDependencies = GetComponentsInChildren<IDataBind<Agent>>(includeInactive: true);
            foreach (var dependency in agentDependencies)
                dependency.Bind(this);

            var stateMonitor = GetComponentInChildren<AgentStateMonitor>();
            if (stateMonitor != null)
            {
                foreach (var state in stateMonitor.PreStates)
                    States.AddState(state.Key, state.Value);
            }

            InitializeActions();
            _orderedGoals = Goals.OrderByDescending(entry => entry.Priority).ToList();

            if (!Application.isEditor)
            {
                PlannerSettings.GenerateFailedPlansReport = false;
                PlannerSettings.GenerateGoalReport = false;
            }

            Initialized = true;
        }

        public bool ShouldAbort()
        {
            if (CurrentAction.ShouldAbort())
            {
                AbortPlan();
                return true;
            }

            return false;
        }

        public virtual void LateUpdate()
        {
            if (PlannerSettings.RunOnLateUpdate)
                Run();
        }

        public virtual void Run()
        {
            if (ExecutePlan())
                return;

            FindPlan();
            EvaluationGoal();
            PrePerformPlanAction();
        }

        public virtual bool ExecutePlan()
        {
            if (CurrentAction != null && CurrentAction.Running)
            {
                if (ShouldAbort())
                    return true;

                PerformPlanAction();
                return true;
            }

            return false;
        }

        public virtual void PrePerformPlanAction()
        {
            Profiler.BeginSample("Agent.PrePerform");
            if (ActionQueue != null && ActionQueue.Count > 0)
            {
                CurrentAction = ActionQueue.Dequeue();

                if (CurrentAction.PrePerform())
                {
                    if (CurrentAction.TrackStopWatch)
                        CurrentAction.Stopwatch.Restart();
                    else
                        CurrentAction.Stopwatch.Reset();

                    CurrentAction.Running = true;
                    CurrentAction.OnPrePerform?.Invoke();
                }
                else
                    AbortPlan();
            }
            Profiler.EndSample();
        }

        public virtual void PerformPlanAction()
        {
            Profiler.BeginSample("Agent.Run");

            var actionStatus = CurrentAction.Perform();
            CurrentAction.OnPerform?.Invoke();

            CheckActionFailed(actionStatus);
            CheckActionSuccess(actionStatus);

            Profiler.EndSample();

            Profiler.BeginSample("Agent.CanAbortPlan");

            if (PlannerSettings.CanAbortPlans && CurrentAction.CanAbort() && Time.time - _planStartTime > PlannerSettings.PlanRate)
                AbortPlan();

            Profiler.EndSample();
        }

        public virtual void CheckActionFailed(EActionStatus status)
        {
            if (status == EActionStatus.Failed)
            {
                CurrentAction?.OnPerformFailed?.Invoke();
                CurrentAction.OnFailed();
                CurrentAction.Running = false;
            }
        }

        public virtual void CheckActionSuccess(EActionStatus status)
        {
            if (status == EActionStatus.Success)
            {
                CurrentAction.PostPerform();
                CurrentAction.OnPostPerform?.Invoke();
                CurrentAction.Running = false;
            }
        }

        /// <summary>
        /// Find the plan and set it in the ActionQueue.
        /// </summary>
        public virtual void FindPlan()
        {
            if (_replan || ActionQueue == null)
            {
                foreach (var goal in _orderedGoals)
                {
                    Profiler.BeginSample("Agent.Plan");
                    ActionQueue = Planner.Plan(this, goal.Key, World.Instance.StateMap, Actions, States.GetStates(), false);
                    _planStartTime = Time.time;
                    Profiler.EndSample();

                    if (ActionQueue == null)
                        continue;

                    CurrentGoal = goal;
                    _replan = false;

                    StoredActionQueue.Clear();
                    foreach (var action in ActionQueue)
                        StoredActionQueue.Enqueue(action);

                    break;
                }
            }
        }

        /// <summary>
        /// Check and Remove a goal if it has been reached and then reinitiate the plan by setting _replan to be true.
        /// </summary>
        public virtual void EvaluationGoal()
        {
            if (ActionQueue != null && ActionQueue.Count == 0)
            {
                if (CurrentGoal.Once)
                {
                    Goals.Remove(CurrentGoal);
                    _orderedGoals.Remove(CurrentGoal);
                }

                _replan = true;
            }
        }

        public void ForceReplan()
        {
            _replan = true;
            AbortPlan();
        }

        public void AbortPlan()
        {
            if (CurrentAction != null)
            {
                CurrentAction.OnFailed();
                CurrentAction.Running = false;
            }

            CleanPlan();
        }

        /// <summary>
        /// Remove any plans the agent has without invoking the OnFailed
        /// </summary>
        public void CleanPlan()
        {
            if(CurrentAction != null)
                CurrentAction.Running = false;

            ActionQueue = null;
            StoredActionQueue.Clear();
            CurrentAction = null;
        }

        /// <summary>
        /// Call this method to update Actions.
        /// Useful for disabling agents actions at runtime.
        /// </summary>
        /// <param name="actions"></param>
        public void InitializeActions(List<Action> actions = null)
        {
            Actions = actions ?? GetComponentsInChildren<Action>().ToList();
            AbortPlan();
        }

        /// <summary>
        /// Call this method when your goal priority has changed to recache the goals order. Calling this every frame can generate GC collection.
        /// </summary>
        public void UpdateGoalOrderCache()
        {
            _orderedGoals = Goals.OrderByDescending(entry => entry.Priority).ToList();
        }
    }
}
