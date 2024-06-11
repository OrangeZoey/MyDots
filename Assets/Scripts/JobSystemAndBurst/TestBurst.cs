using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

public class TestBurst : MonoBehaviour
{
    //¾²Ì¬Ö»¶Á
    public readonly static SharedStatic<float> StaticNum = SharedStatic<float>.GetOrCreate<KeyClass>(); //GetOrCreate<T> TÊÇkey

    public class KeyClass { }

    public static Entity entity;


    private void Awake()
    {
        StaticNum.Data = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(StaticNum.Data);

        //World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentDataRW<GameConfig>(entity);
    }
}
