using System;

namespace SGoap
{
    /// <summary>
    /// A state data often used by the editor, to visualize the key and valu
    /// </summary>
    [Serializable]
    public class State
    {
        public StringReference KeyReference;
        public Concatenator Concatenator;
        public EStateType StateType;

        public float Value;
        public string StringValue;

        public EOperator Operator;

        public string Key
        {
            get
            {
                switch (StateType)
                {
                    case EStateType.Ref:
                        return KeyReference.Value;

                    case EStateType.Code:
                        return Concatenator.Evaluate();

                    case EStateType.Text:
                        return StringValue;

                    default:
                        return "NOT SET";
                }
            }
        }
    }

    public enum EOperator
    {
        Contains,
        Equals,
        LessThan,
        GreaterThan,
        DoesNotContain,
    }
}