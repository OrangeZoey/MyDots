using Unity.Entities;
using Unity.Mathematics;

public struct BulletCreateInfo : IBufferElementData
{
    public float3 position; //位置
    public quaternion rotation; //旋转
}