using Unity.Entities;
using Unity.Rendering;

[MaterialProperty("_Index")] //要修改的属性名称
public struct AnimationFrameIndex : IComponentData,IEnableableComponent
{
    public float value;
}
