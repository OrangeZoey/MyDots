using Unity.Entities;

/// <summary>
/// 共享组件  主线程中运行
/// </summary>
public struct AnimationSharedData : ISharedComponentData
{
    public float frameRate;
    public int frameCount;
}