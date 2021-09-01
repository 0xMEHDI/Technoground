using UnityEngine;

/// <summary>
/// A generic Bind Contract, generally you would bind to one Context
/// </summary>
/// <typeparam name="T">The Context type you this class needs</typeparam>
public interface IDataBind<T>
{
    void Bind(T context);
}
