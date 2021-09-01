using System.Collections.Generic;
using UnityEngine;

namespace SGoap
{
    public class Zone : MonoBehaviour
    {
        public List<Effect> OnEnter;
        public List<Effect> OnExit;

        private void OnTriggerEnter(Collider col)
        {
            var agent = col.GetComponentInParent<Agent>();
            foreach (var effect in OnEnter)
            {
                var states = effect.Space == Space.Self ? agent.States : World.Instance.States;
                GOAPUtils.SetState(effect, states);
            }
        }

        private void OnTriggerExit(Collider col)
        {
            var agent = col.GetComponentInParent<Agent>();
            foreach (var effect in OnExit)
            {
                var states = effect.Space == Space.Self ? agent.States : World.Instance.States;
                GOAPUtils.SetState(effect, states);
            }
        }
    }
}