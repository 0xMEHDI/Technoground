using System;
using UnityEngine;

namespace SGoap
{
    [Serializable]
    public class Effect : State
    {
        public EChangeOperator EffectOperator;
        public Space Space;
    }

    public enum EStateType
    {
        Ref,
        Text,
        Code
    }

    public abstract class Concatenator : MonoBehaviour
    {
        public abstract string Evaluate();
    }

    [Serializable]
    public class ActionEffect : Effect
    {
        public Action Action;
    }

    [Serializable]
    public class PerformEffect : ActionEffect
    {
        public float Rate;
    }

    public enum Space
    {
        Self,
        World
    }
}