using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

/// <summary>
/// 共享数据
/// </summary>
public static class SharedData
{
    //创建静态
    public static readonly SharedStatic<Entity> singtonEnitty = SharedStatic<Entity>.GetOrCreate<keyClass1>();
    public static readonly SharedStatic<GameSharedData> gameShareData = SharedStatic<GameSharedData>.GetOrCreate<GameSharedData>();
    public static readonly SharedStatic<float2> playerPos = SharedStatic<float2>.GetOrCreate<keyClass2>();

    public struct keyClass1 { }
    public struct keyClass2 { }
}

public struct GameSharedData
{
    public int deadCounter; //死亡计数
    public float spawnInterval; //刷怪间隔
    public int spawnCount; //生成数量
    public bool playHitAudio;//是否播放
    public double playHitAudioTime;//吞掉的时间
} 