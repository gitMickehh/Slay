using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObjectPooled
{
    GameObjectPool Pool { get; set; }

    void ReturnToPool();
}

