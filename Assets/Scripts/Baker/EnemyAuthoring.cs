using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class EnemyAuthoring : MonoBehaviour
{
    //public Vector3 scale = Vector3.one; //����
    public float moveSpeed = 4; //�ƶ��ٶ�
    public class EnemyBaker : Baker<EnemyAuthoring>
    {
        public override void Bake(EnemyAuthoring authoring)
        {
            //���ʵ��
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            //������(��ǩ)
            AddComponent<RendererSortTag>(entity);
            //�������״̬
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
