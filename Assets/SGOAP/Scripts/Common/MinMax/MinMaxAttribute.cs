using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class MinMaxAttribute : PropertyAttribute
{
    public float Min = 0;
    public float Max = 1;

    public MinMaxAttribute(int min, int max)
    {
        Min = min;
        Max = max;
    }
}

[Serializable]
public class RangeValue
{
    public float Min = 0;
    public float Max = 1;

    public float Value;

    public RangeValue(float min, float max)
    {
        Min = min;
        Max = max;
    }

    public void SetRandomValue()
    {
        Value = Random.Range(Min, Max);
    }

    public float GetRandomValue()
    {
        return Random.Range(Min, Max);
    }

    public bool WithinRange(float value)
    {
        return value >= Min && value <= Max;
    }
}