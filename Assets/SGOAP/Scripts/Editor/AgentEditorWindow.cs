using SGoap;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using Node = SGoap.Node;

public class SimulationData
{
    public Dictionary<string, float> WorldStates = new Dictionary<string, float>();
    public Dictionary<string, bool> WorldStatesVisibility = new Dictionary<string, bool>();
}

public class AgentEditorWindow : EditorWindow
{
    private GUIStyle _headerStyle = new GUIStyle();
    public static string SelectedText;
    private int _tab;
    private Vector2 pageScrollPosition;
    private Agent _agent;

    public bool UnityPro => EditorGUIUtility.isProSkin;

    [MenuItem("Window/Goap Agent Debugger")]
    static void Init()
    {
        var window = (AgentEditorWindow)EditorWindow.GetWindow(typeof(AgentEditorWindow), false, "Agent Debugger");
        window.Show();
    }

    private void OnEnable()
    {
        var worldStatesJson = EditorPrefs.GetString("worldStatesJson");
        var worldStatesVisibilityJson = EditorPrefs.GetString("worldStatesVisibilityJson");

        if(!string.IsNullOrEmpty(worldStatesJson))
            WorldStates = JsonConvert.DeserializeObject<Dictionary<string, float>>(worldStatesJson);

        if (!string.IsNullOrEmpty(worldStatesVisibilityJson))
            WorldStatesVisibility = JsonConvert.DeserializeObject<Dictionary<string, bool>>(worldStatesVisibilityJson);
    }

    public void DrawCenterText(string text, float size = 25)
    {
        var color = UnityPro ? "white" : "black";

        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Box($"<size={size}><color={color}>{text}</color></size>", _headerStyle);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();
    }

    private void OnGUI()
    {
        _headerStyle.richText = true;
        _headerStyle.alignment = TextAnchor.MiddleLeft;
        GUI.skin.label.richText = true;

        if (Selection.activeGameObject?.GetComponent<Agent>())
            _agent = Selection.activeGameObject?.GetComponent<Agent>();

        var agent = _agent;

        if (agent == null)
        {
            DrawCenterText("Select an Agent to inspect");
            return;
        }

        _tab = GUILayout.Toolbar(_tab, new string[] {"Overview", "Simulator", "Realtime"}, GUILayout.Height(35));

        pageScrollPosition = GUILayout.BeginScrollView(pageScrollPosition, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));

        GUILayout.Space(20);

        if (_tab == 0)
            DrawOverview(agent);
        if (_tab == 1)
            DrawSimulator(agent);

        if (_tab == 2)
            DrawRealTime(agent);

        GUILayout.Space(20);

        GUILayout.EndScrollView();
    }

    private Dictionary<Action, bool> _actionDropdown = new Dictionary<Action, bool>();

    private void DrawRealTime(Agent agent)
    {
        if (!Application.isPlaying)
        {
            DrawCenterText("Runtime inspector for your agent");
            return;
        }

        var size = 50;
        GUI.skin.box.richText = true;

        GUILayout.BeginVertical("Box", GUILayout.Height(size));
        if (agent.CurrentGoal != null)
        {
            DrawSubHeader("Current Goal");
            GUILayout.Box($"{agent.CurrentGoal.Key}");
        }
        GUILayout.EndVertical();

        GUILayout.Space(20);

        var actions = agent.StoredActionQueue;

        GUILayout.BeginHorizontal();
        DrawSubHeader("Plan");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginVertical("Box", GUILayout.Height(size/2));
        if (agent.ActionQueue == null)
        {
            var lastAction = agent.CurrentAction != null ? $"\nLast Action: {agent.CurrentAction.Name}" : "";
            GUILayout.Box($" No available actions{lastAction}");
        }
        else
        {
            var actionNames = "";
            var last = actions.Last();

            foreach (var action in actions)
            {
                var extra = action == last ? "" : "-->";
                var name = action.Name;
                if (action == agent.CurrentAction)
                    name = $"<color=yellow>{name}</color>";

                actionNames += $"{name}{extra}";
            }

            GUILayout.Box($"{actionNames}");
        }
        GUILayout.EndVertical();



        var labelSize = 80;
        var actionLabelSize = 150;

        DrawSubHeader("Actions");
        GUILayout.Label("All usable actions will be part of the plan");
        GUILayout.Label("The preconditions is based on the current state and can be true during the planning.");
        GUILayout.BeginHorizontal();

        GUILayout.Label("", GUILayout.Width(actionLabelSize));
        GUILayout.Label("CoolDown", GUILayout.Width(labelSize));
        GUILayout.Label("Running", GUILayout.Width(labelSize));
        GUILayout.Label("Usable", GUILayout.Width(labelSize));
        GUILayout.Label("Preconditions", GUILayout.Width(labelSize));
        GUILayout.Label("Cost", GUILayout.Width(labelSize));

        GUILayout.EndHorizontal();

        GUILayout.BeginVertical("Box");
        var c = GUI.backgroundColor;
        foreach (var action in agent.Actions)
        {
            if (ReferenceEquals(action, agent.CurrentAction))
                GUI.backgroundColor = Color.yellow;
            else
                GUI.backgroundColor = c;

            GUILayout.BeginHorizontal("Box");

            if (!_actionDropdown.ContainsKey(action))
                _actionDropdown.Add(action, false);

            var buttonIcon = _actionDropdown[action] ? "-" : ">";

            if (GUILayout.Button(buttonIcon, GUILayout.Width(15)))
                _actionDropdown[action] = !_actionDropdown[action];

            GUILayout.Label($"{action.Name}", GUILayout.Width(actionLabelSize));

            action.Cooldown.Active = GUILayout.Toggle(action.Cooldown.Active, "", GUILayout.Width(labelSize));
            action.Running = GUILayout.Toggle(action.Running, "", GUILayout.Width(labelSize));
            GUILayout.Toggle(action.IsUsable(), "", GUILayout.Width(labelSize));

            var states = new Dictionary<string, float>(agent.States.GetStates());
            foreach (var state in World.Instance.StateMap)
            {
                if (!states.ContainsKey(state.Key))
                    states.Add(state.Key, state.Value);
            }

            var preconditionMatch = action.IsPreconditionsMet(states);
            GUILayout.Toggle(preconditionMatch, "", GUILayout.Width(labelSize));

            try
            {
                action.DynamicallyEvaluateCost();
                action.Cost = EditorGUILayout.FloatField(GUIContent.none, action.Cost, GUILayout.Width(labelSize));
            }
            catch { }

            GUILayout.EndHorizontal();
            GUI.skin.label.alignment = TextAnchor.MiddleLeft;

            if (_actionDropdown[action])
            {
                GUILayout.Label($"Preconditions:");
                GUILayout.BeginHorizontal(GUILayout.Width(Screen.width * 0.8f));
                GUILayout.Space(20);
                GUILayout.BeginVertical();
                foreach (var precondition in action.PreConditions)
                {
                    var operatorExtra = precondition.Operator != EOperator.Contains && precondition.Operator != EOperator.DoesNotContain ?
                        $"{precondition.Value}" :
                        $"";

                    var operatorName = $"{OperatorFriendlyNames[precondition.Operator]} {operatorExtra}";

                    if (GUILayout.Button($"<color={GetColor(precondition.Key)}><size=12>{precondition.Key} {operatorName}</size></color>", _headerStyle))
                        SelectedText = precondition.Key;
                }
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();

                GUILayout.Label($"Effects:");
                GUILayout.BeginHorizontal(GUILayout.Width(Screen.width * 0.8f));
                GUILayout.Space(20);
                GUILayout.BeginVertical();
                foreach (var effect in action.AfterEffects)
                {
                    if (GUILayout.Button($"<color={GetColor(effect.Key)}><size=12>{effect.Key}</size></color>", _headerStyle))
                        SelectedText = effect.Key;
                }
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }

        }

        GUI.backgroundColor = c;
        GUILayout.EndVertical();

        GUILayout.Space(20);
        if (agent.PlannerSettings.GenerateGoalReport)
        {
            var goals = agent.Goals;
            foreach (var goal in goals)
            {
                var planner = agent.Planner;
                if (planner.GoalRecords.ContainsKey(goal.Key))
                    _goalLeaves[goal] = JsonConvert.DeserializeObject<List<Node>>(planner.GoalRecords[goal.Key]);

                if (planner.ErrorRecords.ContainsKey(goal.Key))
                    _goalErrors[goal] = planner.ErrorRecords[goal.Key];
            }

            DrawGoals(goals, false);
        }

        if (agent.PlannerSettings.GenerateFailedPlansReport)
            DrawErrors();

        GUILayout.Space(20);

        DrawSubHeader("World States");
        GUILayout.BeginVertical("Box", GUILayout.Height(size));
        var worldStates = World.Instance.States.GetStates();
        DrawStates(worldStates, ref _worldStateInput, ref _worldStateValueInput);
        GUILayout.Label("");
        GUILayout.EndVertical();

        GUILayout.Space(20);

        DrawSubHeader("Agent States");
        GUILayout.BeginVertical("Box", GUILayout.Height(size));
        var agentStates = agent.States.GetStates();
        DrawStates(agentStates, ref _agentStateInput, ref _agentStateValueInput);
        GUILayout.EndVertical();

        Repaint();
    }

    private Dictionary<Goal, Queue<Action>> _goalActions = new Dictionary<Goal, Queue<Action>>();
    private Dictionary<Goal, List<Node>> _goalLeaves = new Dictionary<Goal, List<Node>>();

    public static Dictionary<string, float> WorldStates = new Dictionary<string, float>();
    public static Dictionary<string, bool> WorldStatesVisibility = new Dictionary<string, bool>();

    private string _stateInput;
    private float _stateValue;
    private bool _simulate = true;

    private string _agentStateInput;
    private float _agentStateValueInput;

    private string _worldStateInput;
    private float _worldStateValueInput;

    public void DrawSimulator(Agent agent)
    {
        DrawHeader("Simulator");

        GUILayout.Label("Simulate states and see what your agents will do. You can save the states and import it again later. ");
        
        GUILayout.Space(10);

        _simulate = EditorPrefs.GetBool("simulate");
        _simulate = GUILayout.Toggle(_simulate, new GUIContent("Ignore procedural checks."));
        GUILayout.Label("Add a component inheriting SimulatorDataProvider for procedural checks data", EditorStyles.helpBox);
        EditorPrefs.SetBool("simulate", _simulate);

        GUILayout.Space(10);

        var actions = agent.GetComponentsInChildren<Action>().ToList();
        var goals = agent.Goals;

        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical("Box", GUILayout.Width(Screen.width * 0.3f), GUILayout.Height(Screen.height * 0.1f));
        _stateInput = EditorGUILayout.TextField("State: ", _stateInput, GUILayout.Width(Screen.width * 0.3f));
        _stateValue = EditorGUILayout.FloatField("Value: ", _stateValue, GUILayout.Width(Screen.width * 0.3f));

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Add", GUILayout.Width(60)))
        {
            WorldStates[_stateInput] = _stateValue;
            WorldStatesVisibility[_stateInput] = true;
        }

        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        GUILayout.BeginVertical("Box", GUILayout.Width(Screen.width * 0.6f), GUILayout.Height(Screen.height * 0.1f));
        GUILayout.Label("Simulated States");

        foreach (var worldState in WorldStates.Reverse())
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(worldState.Key);
            WorldStates[worldState.Key] = EditorGUILayout.FloatField("Value: ", WorldStates[worldState.Key], GUILayout.Width(Screen.width * 0.3f));
            WorldStatesVisibility[worldState.Key] = EditorGUILayout.Toggle("", WorldStatesVisibility[worldState.Key], GUILayout.Width(20));
            if (GUILayout.Button("X", GUILayout.Width(25)))
            {
                WorldStates.Remove(worldState.Key);
                WorldStatesVisibility.Remove(worldState.Key);
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUI.changed)
            UpdateSimulatorCache();

        if (GUILayout.Button("Save"))
        {
            var path = EditorUtility.SaveFilePanel("Simulation Data", Application.dataPath, "simulation-data", "sim");

            var simulationData = new SimulationData
            {
                WorldStates = WorldStates,
                WorldStatesVisibility = WorldStatesVisibility
            };

            var json = JsonConvert.SerializeObject(simulationData, Formatting.Indented);
            File.WriteAllText(path, json);
        }

        if (GUILayout.Button("Load"))
        {
            var path = EditorUtility.OpenFilePanel("Simulation Data", Application.dataPath, "sim");
            if (string.IsNullOrEmpty(path))
                return;

            var json = File.ReadAllText(path);
            var data = JsonConvert.DeserializeObject<SimulationData>(json);

            WorldStates = data.WorldStates;
            WorldStatesVisibility = data.WorldStatesVisibility;

            UpdateSimulatorCache();
        }

        if (GUILayout.Button("Plan", GUILayout.Width(60)))
        {
            var simulatorData = agent.GetComponentInChildren<SimulatorDataProvider>();
            simulatorData?.Setup();

            CachePlan(goals, actions, agent);
            simulatorData?.Clean();
        }

        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();

        DrawGoals(goals);

        if(agent.PlannerSettings.GenerateFailedPlansReport)
            DrawErrors();
    }

    void DrawErrors()
    {
        GUILayout.Space(20);

        DrawHeader("Breakdown");
        GUILayout.BeginVertical("Box");
        foreach (var goalError in _goalErrors)
        {
            DrawSubHeader(goalError.Key.Key);
            GUILayout.Label($"{goalError.Value}");
            GUILayout.Space(30);
        }

        GUILayout.EndVertical();
    }

    void DrawStates(Dictionary<string, float> states, ref string inputState, ref float inputValue)
    {
        EditorGUIUtility.labelWidth = 60;
        GUILayout.BeginHorizontal("Box");
        {
            inputState = EditorGUILayout.TextField("State: ", inputState, GUILayout.Width(Screen.width * 0.3f));
            inputValue = EditorGUILayout.FloatField("Value: ", inputValue, GUILayout.Width(Screen.width * 0.3f));

            if (GUILayout.Button("Add", GUILayout.Width(60)))
                states.Add(inputState, inputValue);
        }

        GUILayout.EndHorizontal();

        foreach (var state in states.Reverse())
        {
            GUILayout.BeginHorizontal(GUILayout.Width(Screen.width * 0.7f));
            GUILayout.Label(state.Key);
            states[state.Key] = EditorGUILayout.FloatField("Value: ", states[state.Key], GUILayout.Width(Screen.width * 0.3f));
            if (GUILayout.Button("X", GUILayout.Width(25)))
                states.Remove(state.Key);
            GUILayout.EndHorizontal();
        }
    }

    private Dictionary<Goal, string> _goalErrors = new Dictionary<Goal, string>();

    public void CachePlan(List<Goal> goals, List<Action> actions, Agent agent)
    {
        _goalActions.Clear();
        _goalLeaves.Clear();
        _goalErrors.Clear();

        foreach (var goal in goals.OrderByDescending(x => x.Priority))
        {
            var agentStates = new Dictionary<string, float>();
            var worldStates = new Dictionary<string, float>(WorldStates);

            if (worldStates.ContainsKey(goal.Key))
                worldStates.Remove(goal.Key);

            foreach (var worldState in worldStates.Reverse())
            {
                if (WorldStatesVisibility[worldState.Key] == false)
                    worldStates.Remove(worldState.Key);
            }

            var planner = new Planner();
            var queuedActions = planner.Plan(agent, goal.Key, worldStates, actions, agentStates, _simulate);

            if(planner.ErrorRecords.ContainsKey(goal.Key))
                _goalErrors[goal] = planner.ErrorRecords[goal.Key];

            if (queuedActions != null)
                _goalActions[goal] = new Queue<Action>(queuedActions);

            planner = new Planner();
            var leaves = planner.BuildLeaves(goal.Key, worldStates, actions, agentStates, out var success, _simulate);

            if (leaves != null)
                _goalLeaves[goal] = new List<Node>(leaves);

        }
    }

    public void CacheLeaves(Goal goal, Dictionary<string, float> worldStates, List<Action> actions, Dictionary<string, float> agentStates, bool simulate)
    {
        var planner = new Planner();
        var leaves = planner.BuildLeaves(goal.Key, worldStates, actions, agentStates, out var success, simulate);

        if (leaves != null)
            _goalLeaves[goal] = new List<Node>(leaves);
    }

    public void DrawGoals(List<Goal> goals, bool drawCompletedActions = true)
    {
        var orderedGoals = goals.OrderByDescending(x => x.Priority);
        foreach (var goal in orderedGoals)
        {
            GUILayout.Space(15);

            GUILayout.BeginVertical("Box");
            DrawSubHeader($"{goal.Key} ({goal.Priority})");

            if (!_goalLeaves.ContainsKey(goal))
            {
                GUILayout.Label($"<color={LeaveColor}>No available actions</color>");
                GUILayout.EndVertical();
                continue;
            }

            if (drawCompletedActions)
            {
                if (!_goalActions.ContainsKey(goal))
                {
                    GUILayout.Label($"<color={LeaveColor}>No available actions</color>");
                    GUILayout.EndVertical();
                    continue;
                }

                var goalActions = _goalActions[goal];

                if (goalActions == null)
                {
                    GUILayout.Label($"<color={LeaveColor}>No available actions</color>");
                    GUILayout.EndVertical();
                    continue;
                }

                GUILayout.BeginHorizontal();
                foreach (var goalAction in goalActions)
                {
                    var extra = goalActions.Last() == goalAction ? "" : " -->";
                    GUILayout.Label($"{goalAction.Name}{extra}");
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }

            if(_goalLeaves.ContainsKey(goal))
                DrawLeaves(_goalLeaves[goal]);

            GUILayout.EndVertical();
        }

    }

    public void UpdateSimulatorCache()
    {
        var worldStatesJson = JsonConvert.SerializeObject(WorldStates);
        var worldStatesVisibilityJson = JsonConvert.SerializeObject(WorldStatesVisibility);

        EditorPrefs.SetString("worldStatesJson", worldStatesJson);
        EditorPrefs.SetString("worldStatesVisibilityJson", worldStatesVisibilityJson);
    }

    public string LeaveColor => UnityPro ? "#616161" : "black";

    public void DrawLeaves(List<Node> leaves)
    {

        if (leaves.Count == 0)
        {
            GUILayout.Label($"<color={LeaveColor}>No available actions</color>");
            return;
        }

        var logBuilder = new StringBuilder();

        foreach (var leaf in leaves.OrderBy(x => x.Cost))
        {
            logBuilder.AppendLine("");
            var l = leaf;

            var queue = new Stack<string>();
            while (l != null)
            {
                if(!string.IsNullOrEmpty(l.ActionName))
                    queue.Push(l.ActionName);

                l = l.Parent;
            }

            var cost = leaf.Cost.ToString("F");
            logBuilder.Append($"<color={LeaveColor}>[${cost}]--</color>");

            while (queue.Count != 0)
            {
                var action = queue.Pop();
                var extra = queue.Count == 0 ? "" : " --> ";

                logBuilder.Append($"<color={LeaveColor}>{action}{extra}</color>");
            }
        }

        GUILayout.Label(logBuilder.ToString(), _headerStyle);
    }

    public void DrawOverview(Agent agent)
    {
        var actions = agent.GetComponentsInChildren<Action>();
        var goals = agent.Goals;

        DrawHeader("Goals");
        GUILayout.Label("Select any text to highlight connecting effects.");
        GUILayout.Space(10);

        var goalList = new Queue<Goal>(goals);
        while (goalList.Count != 0)
        {
            EditorGUILayout.BeginHorizontal();
            for (int i = 0; i < 3; i++)
            {
                if (goalList.Count == 0)
                    break;

                GUILayout.Space(10);

                var goal = goalList.Dequeue();
                var validActions = new HashSet<Action>();
                foreach (var action in actions)
                {
                    var effects = new List<string>();
                    if (action.AfterEffects == null)
                        continue;

                    foreach (var effect in action.AfterEffects)
                    {
                        if (effect.Key.Equals(goal.Key))
                            validActions.Add(action);
                    }
                }

                EditorGUILayout.BeginVertical("Box", GUILayout.Width(Screen.width * 0.33f));

                var style = EditorStyles.foldoutHeader;
                style.fontSize = 14;
                style.richText = true;

                if (GUILayout.Button($"<color={GetColor(goal.Key)}>{goal.Key}</color>", style))
                    SelectedText = goal.Key;

                GUILayout.BeginHorizontal();
                GUILayout.Space(10);
                GUILayout.BeginVertical();
                foreach (var action in validActions)
                {
                    if (GUILayout.Button($"<color={GetColor(action.Name)}><size=12>{action.Name}</size></color>", _headerStyle))
                        SelectedText = action.Name;
                }
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();
        }

        GUILayout.Space(20);

        DrawHeader("Actions");
        GUILayout.Space(10);
        var actionList = new Queue<Action>(actions);

        while (actionList.Count != 0)
        {
            EditorGUILayout.BeginHorizontal();
            for (int i = 0; i < 3; i++)
            {
                if (actionList.Count == 0)
                    break;

                GUILayout.Space(10);
                var action = actionList.Dequeue();

                EditorGUILayout.BeginVertical( "Box",GUILayout.Width(Screen.width * 0.33f));

                var style = EditorStyles.foldoutHeader;
                style.fontSize = 14;
                style.richText = true;

                if (GUILayout.Button($"<color={GetColor(action.Name)}>{action.Name}</color>", style))
                    SelectedText = action.Name;

                DrawHeader("Preconditions:", HeaderColor, "12");

                GUILayout.BeginHorizontal();
                GUILayout.Space(10);
                GUILayout.BeginVertical();
                foreach (var precondition in action.PreConditions)
                {
                    var operatorExtra = precondition.Operator != EOperator.Contains && precondition.Operator != EOperator.DoesNotContain ?
                        $"{precondition.Value}" :
                        $"";

                    var operatorName = $"{OperatorFriendlyNames[precondition.Operator]} {operatorExtra}";

                    if (GUILayout.Button($"<color={GetColor(precondition.Key)}><size=12>{precondition.Key} {operatorName}</size></color>", _headerStyle))
                        SelectedText = precondition.Key;
                }

                GUILayout.EndVertical();
                GUILayout.EndHorizontal();

                DrawHeader("Effects:", HeaderColor, "12");

                GUILayout.BeginHorizontal();
                GUILayout.Space(10);
                GUILayout.BeginVertical();
                foreach (var effect in action.AfterEffects)
                {
                    if (GUILayout.Button($"<color={GetColor(effect.Key)}><size=12>{effect.Key}</size></color>", _headerStyle))
                        SelectedText = effect.Key;
                }

                GUILayout.EndVertical();
                GUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(20);
        }
    }

    public string HeaderColor => UnityPro ? "white" : "black";

    public void DrawHeader(string text, string color = "white", string size="17")
    {
        color = HeaderColor;
        EditorGUILayout.LabelField($"<color={color}><size={size}>{text}</size></color>", _headerStyle);
    }

    public void DrawSubHeader(string text)
    {
        var style = EditorStyles.foldoutHeader;
        style.fontSize = 14;
        GUILayout.Label(text, EditorStyles.foldoutHeader);
    }

    public Dictionary<EOperator, string> OperatorFriendlyNames = new Dictionary<EOperator, string>
    {
        { EOperator.Contains, "*"},
        { EOperator.DoesNotContain, "!="},
        { EOperator.Equals, "=="},
        { EOperator.GreaterThan, ">"},
        { EOperator.LessThan, "<S"},
    };

    public string GetColor(string n)
    {
        var color = UnityPro ? "#b5b5b5" : "black";
        return SelectedText != null && SelectedText.Equals(n) ? "green" : $"{color}";
    }
}