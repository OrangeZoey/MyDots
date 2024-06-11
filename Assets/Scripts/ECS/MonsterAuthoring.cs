using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class MonsterAuthoring : MonoBehaviour
{
    public float Hp = 100;
    public float MoveSpeed = 1;
    public float CreateBulletInterval=1;

    public List<int>Skills= new List<int>();

   
    public class MonsterBaker : Baker<MonsterAuthoring>
    {
        public override void Bake(MonsterAuthoring authoring)
        {
            Entity monsterEntity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<MonsterData>(monsterEntity, new MonsterData()
            {
                Hp = authoring.Hp,              
            });

            //不启用组件
            SetComponentEnabled<MonsterData>(monsterEntity,true);

            AddSharedComponent<MonsterConfig>(monsterEntity, new MonsterConfig()
            {
              
                CreateBulletInterval=authoring.CreateBulletInterval,
            });
            AddComponent<MoveData>(monsterEntity, new MoveData()
            {
                MoveSpeed = authoring.MoveSpeed
            });

            AddBuffer<Skill>(monsterEntity);
            for (int i = 0; i < authoring.Skills.Count; i++)
            {
                AppendToBuffer<Skill>(monsterEntity, new Skill()
                {
                    Id = authoring.Skills[i]
                }) ;
            }
        }
    }
}
