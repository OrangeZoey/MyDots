using Unity.Entities;
using Unity.Mathematics;

public struct EnemySharedData : ISharedComponentData
{
    public float moveSpeed;//移动速度
    public float2 scale; //比例
}