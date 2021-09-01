using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using UnityEngine.Profiling;

namespace SGoap
{
    public class Planner
    {
        private List<Action> _actions = new List<Action>();

        private Stack<Node> _usedNodes = new Stack<Node>(10000);
        private Stack<List<Action>> _usedActions = new Stack<List<Action>>(10000);
        private Queue<Action> _actionQueue = new Queue<Action>(20);

        public static bool Initialized;
        public Dictionary<string, string> GoalRecords = new Dictionary<string, string>();
        public Dictionary<string, string> ErrorRecords = new Dictionary<string, string>();

        public StringBuilder ErrorBuilder = new StringBuilder();

        public bool GenerateGoalsReport = false;
        public bool GenerateFailedPlansReport = false;

        public Planner()
        {
            if (Initialized)
                return;

            DictionaryPool<string, float>.Initialize(10000);
            Pool<Node>.Initialize(10000);
            ListPool<Action>.Initialize(10000);
            ListPool<Node>.Initialize(10000);
            Initialized = true;
        }

        public void AddAchievableActions(List<Action> actions, bool simulate = false)
        {
            _actions.Clear();

            foreach (var action in actions)
            {
                if (simulate)
                {
                    _actions.Add(action);
                    continue;
                }

                if (action.IsUsable() || action.AlwaysIncludeInPlan)
                {
                    action.DynamicallyEvaluateCost();
                    _actions.Add(action);
                }
            }
        }

        public Node BuildFirstNode(Dictionary<string, float> worldStates, Dictionary<string, float> agentStates)
        {
            var firstNode = Pool<Node>.Get();
            firstNode.States = DictionaryPool<string, float>.Get();

            foreach (var agentState in agentStates)
            {
                if (!firstNode.States.ContainsKey(agentState.Key))
                    firstNode.States.Add(agentState.Key, agentState.Value);
            }

            foreach (var state in worldStates)
            {
                if (!firstNode.States.ContainsKey(state.Key))
                    firstNode.States.Add(state.Key, state.Value);
            }

            return firstNode;
        }

        public Queue<Action> Plan(Agent agent, string goal, Dictionary<string, float> worldStates, List<Action> actions, Dictionary<string, float> agentStates, bool simulate = false)
        {
            GenerateFailedPlansReport = agent.PlannerSettings.GenerateFailedPlansReport;
            GenerateGoalsReport = agent.PlannerSettings.GenerateGoalReport;

            Profiler.BeginSample("Plan Editor Reports");
            if (GenerateFailedPlansReport)
                ErrorBuilder.Clear();
            Profiler.EndSample();

            AddAchievableActions(actions, simulate);

            var firstNode = BuildFirstNode(worldStates, agentStates);
            _usedNodes.Push(firstNode);

            var leaves = BuildLeaves(goal, worldStates, actions, agentStates, out var success, simulate);

            Profiler.BeginSample("Plan Editor Reports");
            if(GenerateGoalsReport)
                GoalRecords[goal] = JsonConvert.SerializeObject(leaves);

            if (GenerateFailedPlansReport)
                ErrorRecords[goal] = ErrorBuilder.ToString();
            Profiler.EndSample();

            if (!success)
            {
                Clean(leaves);
                return null;
            }

            Node cheapest = null;
            foreach (var leaf in leaves)
            {
                if (cheapest == null)
                    cheapest = leaf;
                else
                {
                    if (leaf.Cost < cheapest.Cost)
                        cheapest = leaf;
                }
            }

            var result = ListPool<Action>.Get();
            _usedActions.Push(result);

            var n = cheapest;

            while (n != null)
            {
                if (n.Action != null)
                    result.Insert(0, n.Action);

                n = n.Parent;
            }

            _actionQueue.Clear();
            foreach (var action in result)
                _actionQueue.Enqueue(action);

            Clean(leaves);

            return _actionQueue;
        }

        public void Clean(List<Node> leaves)
        {
            while (_usedNodes.Count > 0)
            {
                var node = _usedNodes.Pop();

                DictionaryPool<string, float>.Return(node.States);
                node.Parent = null;
                node.States = null;
                node.Action = null;
                node.ActionName = string.Empty;
                node.Cost = 0;
                Pool<Node>.Return(node);
            }

            while (_usedActions.Count > 0)
                ListPool<Action>.Return(_usedActions.Pop());

            ListPool<Node>.Return(leaves);
        }

        public List<Node> BuildLeaves(string goal, Dictionary<string, float> worldStates, List<Action> actions, Dictionary<string, float> agentStates, out bool success, bool simulate = false)
        {
            Profiler.BeginSample("Build Leaves");
            AddAchievableActions(actions, simulate);

            var firstNode = BuildFirstNode(worldStates, agentStates);
            _usedNodes.Push(firstNode);

            var leaves = ListPool<Node>.Get();

            //success = BuildGraph(firstNode, leaves, _actions, goal); - This leads to a bug where success is false when there is a leaf
            BuildGraph(firstNode, leaves, _actions, goal);
            success = leaves.Count > 0;

            Profiler.EndSample();
            return leaves;
        }

        public void AddError(Node node, Action currentAction, Dictionary<string, float> currentStates)
        {
            Profiler.BeginSample("Plan Editor Report");
            var n = node.Parent;
            ErrorBuilder.Append("<color=yellow>");
            while (n != null && n.Action != null)
            {
                ErrorBuilder.Append($"   {n.Action.Name} -->");
                n = n.Parent;
            }

            ErrorBuilder.Append($"{currentAction.Name}");
            ErrorBuilder.Append("</color>");

            ErrorBuilder.Append("\n<color=red>   [Preconditions not met]</color>");

            ErrorBuilder.Append("\n   [Preconditions]\n      ");
            foreach (var condition in currentAction.PreConditions)
                ErrorBuilder.Append($"{condition.Operator} {condition.Key}, ");

            ErrorBuilder.Append("\n   [Current States]\n      ");

            foreach (var state in currentStates)
                ErrorBuilder.Append($"{state.Key}, ");

            ErrorBuilder.Append("\n\n");
            Profiler.EndSample();
        }

        public bool BuildGraph(Node parent, List<Node> leaves, List<Action> usableActions, string goal)
        {
            var foundPath = false;

            foreach (var usableAction in usableActions)
            {
                var usable = usableAction.IsPreconditionsMet(parent.States);

                if (!usable)
                {
                    if(GenerateFailedPlansReport)
                        AddError(parent, usableAction, parent.States);

                    continue;
                }

                if (!usableAction.IsProceduralPreconditionMet(parent.States))
                    continue;

                var currentState = DictionaryPool<string, float>.Get();

                var node = Pool<Node>.Get();
                _usedNodes.Push(node);

                node.States = currentState;
                node.Action = usableAction;
                node.ActionName = usableAction.Name;
                node.Parent = parent;
                node.Cost = parent.Cost + usableAction.Cost;
                node.Cost += usableAction.DynamicallyEvaluateCost(parent.States);

                foreach (var parentState in parent.States)
                {
                    if(!currentState.ContainsKey(parentState.Key))
                        currentState.Add(parentState.Key, parentState.Value);
                }

                foreach (var effect in usableAction.AfterEffects)
                {
                    if (!currentState.ContainsKey(effect.Key))
                        currentState.Add(effect.Key, effect.Value);
                }

                usableAction.ProceduralEffect(currentState);

                // Goal Achieved
                if (currentState.ContainsKey(goal))
                {
                    leaves.Add(node);
                    foundPath = true;
                }
                else
                {
                    var subsetActions = ListPool<Action>.Get();
                    _usedActions.Push(subsetActions);

                    foreach (var action in usableActions)
                        subsetActions.Add(action);

                    subsetActions.Remove(usableAction);
                    foundPath = BuildGraph(node, leaves, subsetActions, goal);
                }
            }

            return foundPath;
        }
    }

    public class Node
    {
        public Dictionary<string, float> States;

        [JsonIgnore]
        public Action Action;

        public string ActionName { get; set; }

        public Node Parent;
        public float Cost;
        public override string ToString()
        {
            if (Action == null)
                return "Null";

            return Action.Name;
        }
    }

    public static class Pool<T> where T : new()
    {
        public static Stack<T> Cached;
        public static void Initialize(int count = 100)
        {
            Cached = new Stack<T>();

            for (int i = 0; i < count; i++)
                Cached.Push(new T());
        }

        public static T Get()
        {
            if (Cached.Count == 0)
                Initialize(100);

            return Cached.Pop();
        }

        public static void Return(T instance)
        {
            Cached.Push(instance);
        }
    }

    public static class ListPool<T>
    {
        public static Stack<List<T>> Cached = new Stack<List<T>>();
        public static void Initialize(int count = 100)
        {
            Cached = new Stack<List<T>>();

            for (int i = 0; i < count; i++)
                Cached.Push(new List<T>(20));
        }

        public static List<T> Get()
        {
            return Cached.Pop();
        }

        public static void Return(List<T> instance)
        {
            instance.Clear();
            Cached.Push(instance);
        }
    }

    public static class DictionaryPool<T, W>
    {
        public static Stack<Dictionary<T, W>> Cached;
        public static void Initialize(int count = 100)
        {
            Cached = new Stack<Dictionary<T, W>>();

            for (int i = 0; i < count; i++)
                Cached.Push(new Dictionary<T, W>(20));
        }

        public static Dictionary<T, W> Get()
        {
            return Cached.Pop();
        }

        public static void Return(Dictionary<T, W> instance)
        {
            instance.Clear();
            Cached.Push(instance);
        }

    }
}

[Flags]
public enum ELogLevel
{
    Default = 1 << 0,
    DeepBreakdown = 1 << 1,
    Goals = 1 << 2
}
