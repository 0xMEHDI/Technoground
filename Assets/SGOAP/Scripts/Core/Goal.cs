using System;

namespace SGoap
{
    [Serializable]
    public class Goal : State
    {
        public int Priority;
        public bool Once;
    }
}