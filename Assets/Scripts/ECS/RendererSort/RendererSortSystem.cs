using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

public partial struct RendererSortSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        new RendererSortJob() { }.ScheduleParallel();
    }
    [BurstCompile]
    public partial struct RendererSortJob : IJobEntity
    {
        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="sortTag">标签</param>
        /// <param name="localTransform">坐标</param>
        private void Execute(in RendererSortTag sortTag, ref LocalTransform localTransform)
        {
            localTransform.Position.z = localTransform.Position.y;
        }
    }
}