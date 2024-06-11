using Unity.Entities;

public struct BulletData : IComponentData,IEnableableComponent
{
    public float destroyTimer;//生命时长
}