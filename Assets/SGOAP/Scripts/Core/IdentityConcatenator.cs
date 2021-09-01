namespace SGoap
{
    /// <summary>
    /// Returns a set identity from within the editor, this means it can be or not be instance specific.
    /// </summary>
    public class IdentityConcatenator : Concatenator
    {
        public Identity Identity;

        public override string Evaluate()
        {
            return $"-ID:{Identity.Id}";
        }
    }
}