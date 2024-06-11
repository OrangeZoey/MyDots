using Unity.Entities;
using Unity.Mathematics;

public struct BulletSharedData : ISharedComponentData
{
    public float moveSpeed;
    public float destroyTime;
    public float2 colliderOffset;//碰撞器的偏移
    public float3 colliderHalfExtents;//碰撞器的一半
}