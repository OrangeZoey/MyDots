using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class BulletTestAuthoring : MonoBehaviour
{
    public float Hp = 100;
    public float MoveSpeed = 1;

    public class BulletBaker : Baker<BulletTestAuthoring>
    {
        public override void Bake(BulletTestAuthoring authoring)
        {
            Entity bulletEntity = GetEntity(TransformUsageFlags.Dynamic);           
            AddComponent<MoveData>(bulletEntity, new MoveData()
            {
                MoveSpeed = authoring.MoveSpeed
            });

            //像添加组件一样添加标签
            AddComponent<MoveTag>(bulletEntity);
        }
    }
}
