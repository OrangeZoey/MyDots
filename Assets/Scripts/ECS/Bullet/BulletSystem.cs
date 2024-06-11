using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

public partial struct BulletSystem : ISystem
{
    //创建子弹的数量
    public readonly static SharedStatic<int> createBulletCount = SharedStatic<int>.GetOrCreate<BulletSystem>();
    public void OnCreate(ref SystemState state)
    {
        createBulletCount.Data = 0;
        SharedData.singtonEnitty.Data = state.EntityManager.CreateEntity(typeof(BulletCreateInfo));
    }
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer.ParallelWriter ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

        DynamicBuffer<BulletCreateInfo> bulletCreateInfoBuffer = SystemAPI.GetSingletonBuffer<BulletCreateInfo>();
        createBulletCount.Data = bulletCreateInfoBuffer.Length;
        new BulletJob()
        {
            enemyLayer = 1 << 6,
            ecb = ecb,
            deltaTime = SystemAPI.Time.DeltaTime,
            bulletCreateInfoBuffer = bulletCreateInfoBuffer,
            collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld,
            //enemyLookUp = SystemAPI.GetComponentLookup<EnemyData>()
        }.ScheduleParallel();

        state.CompleteDependency();//确保完成

        if (createBulletCount.Data > 0)         // 补充不足的部分
        {
            //新的子弹   Temp临时的
            NativeArray<Entity> newBullets = new NativeArray<Entity>(createBulletCount.Data, Allocator.Temp);
            //实例化
            ecb.Instantiate(int.MinValue, SystemAPI.GetSingleton<GameConfigData>().bulletPortotype, newBullets);
            //state.EntityManager.Instantiate(SystemAPI.GetSingleton<GameConfigData>().bulletPortotype, newBullets);
            for (int i = 0; i < newBullets.Length; i++)
            {
                BulletCreateInfo info = bulletCreateInfoBuffer[i];
                //设置组件数据
                ecb.SetComponent<LocalTransform>(newBullets[i].Index, newBullets[i], new LocalTransform()
                {
                    Position = info.position,
                    Rotation = info.rotation,
                    Scale = 1
                });
            }
            newBullets.Dispose();         
        }
        //清除掉 不然下次还在
        bulletCreateInfoBuffer.Clear();
      
    }

    [WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)] //忽略组件的启用状态
    [BurstCompile]
    public partial struct BulletJob : IJobEntity
    {
        public uint enemyLayer;//敌人的位掩码
        public EntityCommandBuffer.ParallelWriter ecb; //并行写入
        public float deltaTime;  //计时器      
        [ReadOnly] public DynamicBuffer<BulletCreateInfo> bulletCreateInfoBuffer; //子弹的生成信息
        [ReadOnly] public CollisionWorld collisionWorld; //碰撞世界
        //[ReadOnly] public ComponentLookup<EnemyData> enemyLookUp; //组建的lookup    lookup用来查找数据的

        /// <summary>
        /// 执行函数
        /// </summary>
        /// <param name="bulletData"></param>
        /// <param name="bulletSharedData"></param>
        /// <param name="entity"></param>
        /// <param name="localTransform"></param>
        ///                                         状态                                              排序状态
        public void Execute(EnabledRefRW<BulletData> bulletEnableState, EnabledRefRW<RendererSortTag> sortEnableState, ref BulletData bulletData, in BulletSharedData bulletSharedData, in Entity entity, ref LocalTransform localTransform)
        {

            // 当前子弹是非激活状态，同时需要创建子弹
            if (bulletEnableState.ValueRO == false)
            {
                if (createBulletCount.Data > 0)
                {
                    int index = createBulletCount.Data -= 1;
                    //改变激活状态
                    bulletEnableState.ValueRW = true;
                    sortEnableState.ValueRW = true;
                    //赋值信息
                    localTransform.Position = bulletCreateInfoBuffer[index].position;
                    localTransform.Rotation = bulletCreateInfoBuffer[index].rotation;
                    localTransform.Scale = 1;
                    //死亡计时复原
                    bulletData.destroyTimer = bulletSharedData.destroyTime;
                }
                return;
            }


            // 位置移动
            localTransform.Position += bulletSharedData.moveSpeed * deltaTime * localTransform.Up();
            // 销毁计时
            bulletData.destroyTimer -= deltaTime;
            if (bulletData.destroyTimer <= 0)
            {
                bulletEnableState.ValueRW = false;
                sortEnableState.ValueRW = false;
                localTransform.Scale = 0; //设置为0 就看不见了
                return;
            }

            // 伤害检测
            NativeList<DistanceHit> hits = new NativeList<DistanceHit>(Allocator.Temp);//范围信息
            CollisionFilter filter = new CollisionFilter() //过滤器 所有都可以碰撞
            {
                BelongsTo = ~0u,
                CollidesWith = enemyLayer, // all 1s, so all layers, collide with everything
                GroupIndex = 0
            };

            //范围检测                          位置 旋转 大小 信息 过滤器
            if (collisionWorld.OverlapBox(localTransform.Position, localTransform.Rotation, bulletSharedData.colliderHalfExtents, ref hits, filter))
            {
                for (int i = 0; i < hits.Length; i++)
                {
                    Entity temp = hits[i].Entity;
                    //判断碰撞到的有无 EnemyData
                    //if (enemyLookUp.HasComponent(temp))
                    //{
                        //销毁
                        bulletData.destroyTimer = 0;
                        //设置数据
                        ecb.SetComponent<EnemyData>(temp.Index, temp, new EnemyData()
                        {
                            die = true,
                        });
                    //}
                }
            }
            hits.Dispose();
        }
    }
}