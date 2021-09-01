using UnityEngine;

namespace SGoap
{
    [CreateAssetMenu(menuName = "Games/AI/State")]
    public class StringReference : ScriptableObject
    {
        public string Value;

        public override string ToString()
        {
            return Value;
        }
    }
}