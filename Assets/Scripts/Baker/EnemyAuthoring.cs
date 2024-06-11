using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class EnemyAuthoring : MonoBehaviour
{
    //public Vector3 scale = Vector3.one; //缩放
    public float moveSpeed = 4; //移动速度
    public class EnemyBaker : Baker<EnemyAuthoring>
    {
        public override void Bake(EnemyAuthoring authoring)
        {
            //添加实体
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            //添加组件(标签)
            AddComponent<RendererSortTag>(entity);
            //设置组件状态
            SetComponentEnabled<RendererSortTag>(entity, true);

            AddComponent<EnemyData>(entity, new EnemyData() { die = false });
            SetComponentEnabled<EnemyData>(entity, true);

            AddSharedComponent<EnemySharedData>(entity, new EnemySharedData()
            {
                moveSpeed = authoring.moveSpeed,
                scale=(Vector2)authoring.transform.localScale
            }) ;
        }
    }
}
