namespace SGoap
{
    /// <summary>
    /// Returns the instance ID of any Object, this means no object will ever be the same. If you want to target multiple of the same type, use IdentityEvaluator
    /// </summary>
    public class ObjectConcatenator : Concatenator
    {
        public UnityEngine.Object Target;

        public override string Evaluate()
        {
            return $"-ID:{Target.GetInstanceID()}";
        }
    }
}