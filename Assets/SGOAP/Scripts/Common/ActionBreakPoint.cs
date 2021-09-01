using System;
using UnityEngine;

namespace SGoap
{
    public class ActionBreakPoint : MonoBehaviour
    {
#if UNITY_EDITOR
        private Action Action;

        //[EnumFlags]
        public EActionBreakPointType BreakOn;

        private void Awake()
        {
            Action = GetComponent<Action>();
            Action.OnPerform += OnPerform;
            Action.OnPrePerform += OnPrePerform;
            Action.OnPostPerform += OnPostPerform;
            Action.OnPerformFailed += OnPerformFailed;
        }

        private void OnPerform()
        {
            if(BreakOn.HasFlag(EActionBreakPointType.Perform))
                Break("Perform");
        }

        private void OnPrePerform()
        {
            if (BreakOn.HasFlag(EActionBreakPointType.PrePerform))
                Break("Pre Perform");
        }

        private void OnPostPerform()
        {
            if (BreakOn.HasFlag(EActionBreakPointType.PostPerform))
                Break("Post Perform");
        }

        private void OnPerformFailed()
        {
            if (BreakOn.HasFlag(EActionBreakPointType.Failed))
                Break("Failed");
        }

        public void Break(string message)
        {
            Debug.Log($"[Break Point] {message}");
            Debug.Break();
        }
#endif

        [Flags]
        public enum EActionBreakPointType 
        {
            PrePerform = 1 << 0,
            Perform = 1 << 1,
            PostPerform = 1 << 2,
            Failed = 1 << 3,
        }
    }
}