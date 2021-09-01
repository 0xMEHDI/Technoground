using System.Collections.Generic;

namespace SGoap
{
    public static class GOAPUtils
    {
        public static void SetState(Effect effect, States states)
        {
            var key = effect.Key;

            switch (effect.EffectOperator)
            {
                case EChangeOperator.Set:
                    states.SetState(key, effect.Value);
                    break;

                case EChangeOperator.Modify:
                    states.ModifyState(key, effect.Value);
                    break;

                case EChangeOperator.Remove:
                    states.RemoveState(key);
                    break;
            }
        }

        /// <summary>
        /// Return true if the action has all the requirements to start. This happens after IsAchieveable().
        /// </summary>
        public static bool IsStateMatch(State[] agentWorldStates, Dictionary<string, float> preConditions)
        {
            foreach (var agentPreCondition in agentWorldStates)
            {
                if (agentPreCondition.Operator == EOperator.DoesNotContain)
                {
                    if (preConditions.ContainsKey(agentPreCondition.Key))
                        return false;
                    else
                        continue;
                }

                if (!preConditions.ContainsKey(agentPreCondition.Key))
                    return false;

                switch (agentPreCondition.Operator)
                {
                    case EOperator.Contains:
                        // Handled by default
                        break;

                    case EOperator.LessThan:
                        var value = preConditions[agentPreCondition.Key];
                        if (agentPreCondition.Value < value)
                            return false;

                        break;

                    case EOperator.GreaterThan:
                        if (agentPreCondition.Value > preConditions[agentPreCondition.Key])
                            return false;

                        break;

                    case EOperator.Equals:
                        if (agentPreCondition.Value != preConditions[agentPreCondition.Key])
                            return false;

                        break;
                }
            }

            return true;
        }
    }
}
