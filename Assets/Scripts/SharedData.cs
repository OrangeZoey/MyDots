using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Burst;
using Unity.Entities;

/// <summary>
/// 共享数据
/// </summary>
public static class SharedData
{
    //创建静态
    public static readonly SharedStatic<Entity> singtonEnitty = SharedStatic<Entity>.GetOrCreate<keyClass1>();

    public struct keyClass1 { }
}