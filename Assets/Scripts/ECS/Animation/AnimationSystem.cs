using Unity.Burst;
using Unity.Entities;

public partial struct AnimationSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        new AnimationJob()
        {
            delaTime = state.WorldUnmanaged.Time.DeltaTime
        }.ScheduleParallel();
    }
    [BurstCompile]
    public partial struct AnimationJob : IJobEntity
    {
        public float delaTime; //时间

        /// <summary>
        /// 执行函数
        /// </summary>
        /// <param name="animationData"></param>
        /// <param name="frameIndex"></param>
        private void Execute(in AnimationSharedData animationData, ref AnimationFrameIndex frameIndex)
        {
            //随着时间改变
            float newIndex = frameIndex.value + delaTime * animationData.frameRate;

            //判断
            while (newIndex > animationData.frameCount)
            {
                newIndex -= animationData.frameCount;
            }
            frameIndex.value = newIndex;
        }
    }
}