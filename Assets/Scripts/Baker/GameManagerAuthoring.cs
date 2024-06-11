using Unity.Entities;
using UnityEngine;

public class GameManagerAuthoring : MonoBehaviour
{
    public GameObject bulletPrefab; //子弹预制体
    public GameObject enemyPrefab; //敌人预制体
    public class GameManagerBaker : Baker<GameManagerAuthoring>
    {
        public override void Bake(GameManagerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);//原型
            GameConfigData configData = new GameConfigData();//数据
            configData.bulletPortotype = GetEntity(authoring.bulletPrefab, TransformUsageFlags.Dynamic);
            configData.enemyPortotype = GetEntity(authoring.enemyPrefab, TransformUsageFlags.Dynamic);
            AddComponent<GameConfigData>(entity, configData);//添加组件
        }
    }
}