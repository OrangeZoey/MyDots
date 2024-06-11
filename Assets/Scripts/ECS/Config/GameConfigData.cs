using Unity.Entities;

public struct GameConfigData : IComponentData
{
    public Entity bulletPortotype; //子弹
    public Entity enemyPortotype;  //敌人
}