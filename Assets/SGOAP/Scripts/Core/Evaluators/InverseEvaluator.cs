namespace SGoap
{
    public class InverseEvaluator : UsableEvaluator
    {
        public UsableEvaluator UsableEvaluator;

        public override bool Evaluate(IContext context)
        {
            return !UsableEvaluator.Evaluate(context);
        }
    }
}