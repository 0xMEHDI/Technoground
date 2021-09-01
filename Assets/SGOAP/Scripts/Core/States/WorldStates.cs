using System.Collections.Generic;

namespace SGoap
{
    /// <summary>
    /// A wrapper around a dictionary of string states
    /// </summary>
    public class States
    {
        private readonly Dictionary<string, float> _states = new Dictionary<string, float>();

        public bool HasState(string key) => _states.ContainsKey(key);

        public void AddState(string key, float value)
        {
            if(!_states.ContainsKey(key))
                _states.Add(key, value);
        }

        public void RemoveState(string key)
        {
            _states.Remove(key);
        }

        public void ModifyState(string key, float value)
        {
            if (_states.ContainsKey(key))
            {
                _states[key] += value;
                if (_states[key] <= 0)
                    RemoveState(key);
            }
            else if(value > 0)
                AddState(key, value);
        }

        public void SetState(string key, float value)
        {
            _states[key] = value;
        }

        public float GetValue(string key)
        {
            return _states[key];
        }

        public Dictionary<string, float> GetStates() => _states;

        public void Clear()
        {
            _states.Clear();
        }
    }
}