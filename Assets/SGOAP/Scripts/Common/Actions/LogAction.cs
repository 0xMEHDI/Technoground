namespace SGoap
{
    using UnityEngine;

    public class LogAction : BasicAction
    {
        public override bool PrePerform()
        {
            return true;
        }

        public override bool PostPerform()
        {
            Debug.Log($"Completed Action: {Name}");
            return true;
        }
    }
}