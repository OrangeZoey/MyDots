using Unity.Entities;
using Unity.Mathematics;

public struct BulletSharedData : ISharedComponentData
{
    public float moveSpeed;
    public float destroyTime;
}