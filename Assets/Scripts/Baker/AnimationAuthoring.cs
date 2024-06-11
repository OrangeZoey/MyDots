using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class AnimationAuthoring : MonoBehaviour
{
    public float frameRate;//֡��
    public int frameMaxIndex;
    public class AnimationBaker : Baker<AnimationAuthoring>
    {
        public override void Bake(AnimationAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);//ʵ��
            AddComponent<AnimationFrameIndex>(entity); //������
            AddSharedComponent<AnimationSharedData>(entity, new AnimationSharedData() //��ӹ������
            {
                frameRate = authoring.frameRate,
                frameCount = authoring.frameMaxIndex, 
            });
        }
    }
}
