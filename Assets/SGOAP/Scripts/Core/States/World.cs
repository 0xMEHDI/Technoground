using System.Collections.Generic;

namespace SGoap
{
    public sealed class World
    {
        private static States _worldStates = new States();
        public static World Instance { get; } = new World();

        public States States => _worldStates;
        public Dictionary<string, float> StateMap => States.GetStates();

        public static void ModifyState(string key, float value) => Instance.States.ModifyState(key, value);
        public static void AddState(string key, float value) => Instance.States.AddState(key, value);
        public static void RemoveState(string key) => Instance.States.RemoveState(key);
        public static void SetState(string key, float value) => Instance.States.SetState(key, value);
    }
}