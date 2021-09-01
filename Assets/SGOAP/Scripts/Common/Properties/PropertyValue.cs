using System;

[Serializable]
public class PropertyValue
{
    public string Name;
}

public class PropertyValue<T> : PropertyValue where T : IComparable
{
    public T Value;
}