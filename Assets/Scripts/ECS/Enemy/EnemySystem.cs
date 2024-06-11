using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct EnemySystem : ISystem
{
    public struct key1 { }
    public struct key2 { }
    public struct key3 { }
    public readonly static SharedStatic<int> createdCount = SharedStatic<int>.GetOrCreate<key1>(); //创建过的
    public readonly static SharedStatic<int> createCount = SharedStatic<int>.GetOrCreate<key2>();  //需要创建的
    public readonly static SharedStatic<Random> random = SharedStatic<Random>.GetOrCreate<key3>();
    public float spawnEnemyTimer; //计时器
    public const int maxEnemys = 10000;  //最大数量

    public void OnCreate(ref SystemState state)
    {
        //默认值
        createdCount.Data = 0;
        createCount.Data = 0;
        random.Data = new Random((uint)System.DateTime.Now.GetHashCode());
        SharedData.gameShareData.Data.deadCounter = 0;
    }
    public void OnUpdate(ref SystemState state)
    {
        spawnEnemyTimer -= SystemAPI.Time.DeltaTime; //计时
        if (spawnEnemyTimer <= 0)
        {
            //时间到 改变数值
            spawnEnemyTimer = SharedData.gameShareData.Data.spawnInterval;
            createCount.Data += SharedData.gameShareData.Data.spawnCount;
        }
        EntityCommandBuffer.ParallelWriter ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
        float2 playerPos = SharedData.playerPos.Data;
        new EnemyJob()
        {
            time = SystemAPI.Time.ElapsedTime, //相当于 Time.time
            deltaTime = SystemAPI.Time.DeltaTime,
            playerPos = playerPos,
            ecb = ecb,
        }.ScheduleParallel();
        state.CompleteDependency();

        //如果还有要创建的  并且已经创建的没有超过最大值
        if (createCount.Data > 0 && createdCount.Data < maxEnemys)
        {
            NativeArray<Entity> newEnemys = new NativeArray<Entity>(createCount.Data, Allocator.Temp);
            //实例化
            ecb.Instantiate(int.MinValue, SystemAPI.GetSingleton<GameConfigData>().enemyPortotype, newEnemys);
            for (int i = 0; i < newEnemys.Length && createdCount.Data < maxEnemys; i++)
            {
                createdCount.Data += 1;
                //随机位置
                float2 offset = random.Data.NextFloat2Direction() * random.Data.NextFloat(5f, 10);
                //设置组件
                ecb.SetComponent<LocalTransform>(newEnemys[i].Index, newEnemys[i], new LocalTransform()
                {
                    Position = new float3(playerPos.x + offset.x, playerPos.y + offset.y, 0),
                    Rotation = quaternion.identity,
                    Scale = 1,
                });
            }
            createCount.Data = 0; //创建完变为0
            newEnemys.Dispose();
        }

    }

    [WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
    [BurstCompile]
    public partial struct EnemyJob : IJobEntity
    {
        public float deltaTime;
        public double time;
        public float2 playerPos; //玩家位置
        public EntityCommandBuffer.ParallelWriter ecb;
        private void Execute(EnabledRefRW<EnemyData> enableState, EnabledRefRW<RendererSortTag> rendererSortEnableState, EnabledRefRW<AnimationFrameIndex> aniamtionEnableState, ref EnemyData enemyData, in EnemySharedData enemySharedData, ref LocalTransform localTransform, ref LocalToWorld localToWorld)
        {
            if (enableState.ValueRO == false)
            {
                //大于0  需要生成
                if (createCount.Data > 0)
                {
                    createCount.Data -= 1;
                    //下一个的方向
                    float2 offset = random.Data.NextFloat2Direction() * random.Data.NextFloat(5f, 10);
                    localTransform.Position = new float3(playerPos.x + offset.x, playerPos.y + offset.y, 0);
                    //设置状态
                    enableState.ValueRW = true;
                    rendererSortEnableState.ValueRW = true;
                    aniamtionEnableState.ValueRW = true;
                    localTransform.Scale = 1; 
                }
                return;
            }

            if (enemyData.die)
            {
                SharedData.gameShareData.Data.playHitAudio = true;
                SharedData.gameShareData.Data.deadCounter += 1;
                SharedData.gameShareData.Data.playHitAudioTime +=time;

                //设置状态
                enemyData.die = false;
                enableState.ValueRW = false;
                rendererSortEnableState.ValueRW = false;
                aniamtionEnableState.ValueRW = false;
                localTransform.Scale = 0;
                return;
            }

            //得到一个前进的方向
            float2 dir = math.normalize(playerPos - new float2(localTransform.Position.x, localTransform.Position.y));
            localTransform.Position += deltaTime * enemySharedData.moveSpeed * new float3(dir.x, dir.y, 0);
            localToWorld.Value.c0.x = localTransform.Position.x < playerPos.x ? -enemySharedData.scale.x : enemySharedData.scale.x;
            localToWorld.Value.c1.y = enemySharedData.scale.y;
        }
    }
}