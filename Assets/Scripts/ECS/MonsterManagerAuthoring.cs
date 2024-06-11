using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class MonsterManagerAuthoring : MonoBehaviour
{
    public GameObject BulletPrefab;

    public class MonsterManagerBaker : Baker<MonsterManagerAuthoring>
    {
        public override void Bake(MonsterManagerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent<GameConfig>(entity, new GameConfig()
            {
                BulletProtoType = GetEntity(authoring.BulletPrefab, TransformUsageFlags.Dynamic),
            });
            
        }
    }
}
