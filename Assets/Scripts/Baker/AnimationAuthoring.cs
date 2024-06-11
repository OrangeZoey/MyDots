using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class AnimationAuthoring : MonoBehaviour
{
    public float frameRate;//帧率
    public int frameMaxIndex;
    public class AnimationBaker : Baker<AnimationAuthoring>
    {
        public override void Bake(AnimationAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);//实体
            AddComponent<AnimationFrameIndex>(entity); //添加组件
            AddSharedComponent<AnimationSharedData>(entity, new AnimationSharedData() //添加共享组件
            {
                frameRate = authoring.frameRate,
                frameCount = authoring.frameMaxIndex, 
            });
        }
    }
}
