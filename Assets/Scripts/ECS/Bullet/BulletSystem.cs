using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

public partial struct BulletSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        SharedData.singtonEnitty.Data = state.EntityManager.CreateEntity(typeof(BulletCreateInfo));
    }
    public void OnUpdate(ref SystemState state)
    {
        DynamicBuffer<BulletCreateInfo> bulletCreateInfoBuffer = SystemAPI.GetSingletonBuffer<BulletCreateInfo>();
        if (bulletCreateInfoBuffer.Length == 0) return;
        NativeArray<Entity> newBullets = new NativeArray<Entity>(bulletCreateInfoBuffer.Length, Allocator.Temp);
        state.EntityManager.Instantiate(SystemAPI.GetSingleton<GameConfigData>().bulletPortotype, newBullets);
        for (int i = 0; i < bulletCreateInfoBuffer.Length; i++)
        {
            BulletCreateInfo info = bulletCreateInfoBuffer[i];
            state.EntityManager.SetComponentData<LocalTransform>(newBullets[i], new LocalTransform()
            {
                Position = info.position,
                Rotation = info.rotation,
                Scale = 1
            });
        }
        bulletCreateInfoBuffer.Clear();
        newBullets.Dispose();
    }
}