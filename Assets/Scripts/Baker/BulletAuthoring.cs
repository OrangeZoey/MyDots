using Unity.Entities;
using UnityEngine;

public class BulletAuthoring : MonoBehaviour
{
    public float moveSpeed; //移动速度
    public float destroyTime; //生命时间

    public class BulletBaker : Baker<BulletAuthoring>
    {
        public override void Bake(BulletAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<RendererSortTag>(entity); //添加排序组件
            AddComponent<BulletData>(entity, new BulletData()
            {
                destroyTimer = authoring.destroyTime
            });
            SetComponentEnabled<RendererSortTag>(entity, true);//设置状态
            AddSharedComponent<BulletSharedData>(entity, new BulletSharedData()//添加共享组件
            {
                moveSpeed = authoring.moveSpeed,
                destroyTime = authoring.destroyTime
            });
        }
    }
}