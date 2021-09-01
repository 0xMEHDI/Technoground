namespace SGoap
{
    /// <summary>
    /// Pass between action evaluators in order to provide more concrete classes.
    /// </summary>
    public interface IContext
    {
        /// <summary>
        /// Cast the current context to an expected type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Get<T>() where T : class;
    }
}
