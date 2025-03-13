using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableReference<T> : ScriptableObject
{
    public T Value;
    public Action onChangedAction;

    public virtual void SetValueWithAlert(T value)
    {
        Value = value;
        onChangedAction?.Invoke();
    }
}
