namespace SGoap
{
    public enum EActionStatus
    {
        /// <summary>
        /// Returning failed will trigger abort on the action.
        /// </summary>
        Failed,
        /// <summary>
        /// Action is successfully and will call PostPerform
        /// </summary>
        Success,
        /// <summary>
        /// Keep the action running.
        /// </summary>
        Running
    }
}