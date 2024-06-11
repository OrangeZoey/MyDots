using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static MoveSystem;

public struct Skill : IBufferElementData
{
    public int Id;
}


public readonly partial struct MonsterAspect : IAspect
{
    public readonly RefRW<LocalTransform> localTransfrom;
    public readonly RefRW<MonsterData> monsterData;
    public readonly MonsterConfig monsterConfig;
    public readonly DynamicBuffer<Skill> skills;
}

public readonly partial struct MoveAspect : IAspect
{
    public readonly RefRW<MoveData> moveData;
    public readonly RefRW<LocalTransform> localTransfrom;
    public readonly RefRW<MoveTag> moveTag;
}


public struct MonsterData : IComponentData, IEnableableComponent
{
    public float Hp;
    public float CreateBulletTimer;//计时器
}

[ChunkSerializable]
public struct MonsterConfig : ISharedComponentData
{
    //public Entity BulletProtoType;    
    public float CreateBulletInterval;//间隔

}

public struct GameConfig : IComponentData
{
    public Entity BulletProtoType;
}


public struct MoveData : IComponentData
{
    public float MoveSpeed;
}
public struct MoveTag : IComponentData
{

}

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct MonsterSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        Entity entity = state.EntityManager.CreateEntity();
        //如果没有这个组件则不会执行Update
        state.RequireForUpdate<GameConfig>();
    }

    public void OnUpdate(ref SystemState state)
    {
        //获取单例组件
        GameConfig gameConfig = SystemAPI.GetSingleton<GameConfig>();

        //SystemAPI 只能在System中使用 否则会报错
        //筛选 遍历
        //SystemAPI.Query 查找  RefRO 只能读  RefRW 读写  WithAny 包含任何一个       
        //foreach (MonsterAspect monster in SystemAPI.Query<MonsterAspect>())
        //{
        //    //持续掉血
        //    monster.monsterData.ValueRW.Hp -= SystemAPI.Time.DeltaTime;
        //    //计时
        //    monster.monsterData.ValueRW.CreateBulletTimer -= SystemAPI.Time.DeltaTime;
        //    if (monster.monsterData.ValueRO.CreateBulletTimer <= 0)
        //    {
        //        monster.monsterData.ValueRW.CreateBulletTimer = monster.monsterConfig.CreateBulletInterval;
        //        Entity bullet = state.EntityManager.Instantiate(gameConfig.BulletProtoType);
        //        state.EntityManager.SetComponentData(bullet, new LocalTransform()
        //        {
        //            Position = monster.localTransfrom.ValueRO.Position,
        //            Scale = 0.5f
        //        });
        //    }
        //}
        //state.Dependency.Complete();//确保之前的job都完成
        //EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);

        //我们只管获取  后续不管
        EntityCommandBuffer ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
        new MonsterJob()
        {
            name = "WASD",
            delaTimer = SystemAPI.Time.DeltaTime,
            gameConfig = gameConfig,
            ecb = ecb
        }.Schedule();//隐式依赖  如果填写state.Dependency 意味着显示管理

        //state.Dependency.Complete();
        //ecb.Playback(state.EntityManager);//回放
        //ecb.Dispose();
    }

    //表示没有这些组件
    //[WithNone(typeof(GameObject))]
    [BurstCompile]
    public partial struct MonsterJob : IJobEntity
    {
        public FixedString32Bytes name;
        public float delaTimer;
        public GameConfig gameConfig;
        public EntityCommandBuffer ecb;
        public void Execute(MonsterAspect monster)
        {
            FixedString32Bytes str = $"{name}wasd";
            //Debug.Log(str);//支持这种调用，但是不能手动Tostring

            TestBurst.StaticNum.Data = 3;

            monster.monsterData.ValueRW.Hp -= delaTimer;
            monster.monsterData.ValueRW.CreateBulletTimer -= delaTimer;

            if (monster.monsterData.ValueRO.CreateBulletTimer <= 0)
            {
                monster.monsterData.ValueRW.CreateBulletTimer = monster.monsterConfig.CreateBulletInterval;
                Entity bullet = ecb.Instantiate(gameConfig.BulletProtoType);
                ecb.SetComponent(bullet, new LocalTransform()
                {
                    Position = monster.localTransfrom.ValueRO.Position,
                    Scale = 0.5f
                });
            }
        }
    }
}

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct MoveSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {

        float3 dir = new float3(0, 0, 1);

        foreach (MoveAspect move in SystemAPI.Query<MoveAspect>())
        {
            //向前走
            move.localTransfrom.ValueRW.Position += dir * SystemAPI.Time.DeltaTime * move.moveData.ValueRW.MoveSpeed;
        }


    }

    [BurstCompile]
    public partial struct MoveJob : IJobEntity
    {
        public float delaTimer;
        public float3 dir;
        public void Execute(MoveAspect move)
        {
            move.localTransfrom.ValueRW.Position += dir * delaTimer * move.moveData.ValueRW.MoveSpeed;
            //Debug.Log("JOB线程：" + System.Threading.Thread.CurrentThread.ManagedThreadId);
        }
    }

   
}