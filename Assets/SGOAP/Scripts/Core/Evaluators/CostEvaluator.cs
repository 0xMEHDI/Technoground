using UnityEngine;

namespace SGoap
{
    /// <summary>
    /// Evaluates the cost, this is a way to get dynamic cost for each actions.
    /// </summary>
    public abstract class CostEvaluator : MonoBehaviour
    {
        [TextArea]
        public string Notes;

        public abstract float Evaluate(IContext context);
    }
}
