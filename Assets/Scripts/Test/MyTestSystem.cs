using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public partial struct MyTestSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        Debug.Log("state.DebugName��" + state.DebugName);
    }

    public void OnUpdate(ref SystemState state)
    {
        Debug.Log("state.DebugName��" + state.World);
    }
}
