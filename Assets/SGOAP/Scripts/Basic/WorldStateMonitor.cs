using System.Collections.Generic;
using UnityEngine;

namespace SGoap
{
    public class WorldStateMonitor : MonoBehaviour
    {
        [Header("Pre-load the world with states")] [EffectAndValue]
        public List<State> States;

        private void Awake()
        {
            foreach (var state in States)
                World.Instance.States.AddState(state.Key, state.Value);
        }
    }
}