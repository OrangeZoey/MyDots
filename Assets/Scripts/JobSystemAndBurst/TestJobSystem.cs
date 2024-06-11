using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using Unity.Collections;
using System;

public class TestJobSystem : MonoBehaviour
{
    //Run() 在主线程执行
    //Schedule() 在一个工作线程执行全部任务 
    //ScheduleParallel() 在x个工作线程中 完成各自的任务，并行

    private MyJob job;
    JobHandle jobHandle;
    private NativeArray<int> nums;//Unity.Collections;里的容器 都是相对于指针操作器 虽然是结构体但是传值后如果发生变化 特殊性在于不具备像结构体那样拷贝赋值的特性
    void Start()
    {
        //创建原生数组存储整数  (数组大小，分配器选项（指定了如何分配数组的内存，Temp表示临时数组）)
        //NativeArray<int>nums= new NativeArray<int>(10,Allocator.Temp);

        //获取当前正在执行的线程的托管线程ID
        Debug.Log("主线程：" + System.Threading.Thread.CurrentThread.ManagedThreadId);

        nums = new NativeArray<int>(100, Allocator.Persistent);//持久数组
        job = new MyJob() { nums = nums };
        jobHandle = job.ScheduleParallel(100, 10, default);//一个组完成10个任务 每个组在不同工作线程执行
        jobHandle.Complete();//希望优先完成这个job

        //这种情况要等
        //for (int i = 0; i < nums.Length; i++)
        //{
        //    Debug.Log(nums[i]);
        //}

        //nums.Dispose(); //如果在运作 销毁会报错

        //Invoke("MyTest", 3);
    }

    public void Update()
    {
        //如果完成了
        if(jobHandle.IsCompleted && jobHandle!=default)
        {
            jobHandle.Complete();
            for (int i = 0; i < nums.Length; i++)
            {
                Debug.Log(nums[i]);
            }

            nums.Dispose(); //如果在运作 销毁会报错
            jobHandle = default;//default 的 IsCompleted 为true
        }
    }

    public void MyTest()
    {
        for (int i = 0; i < nums.Length; i++)
        {
            Debug.Log(nums[i]);
        }

        nums.Dispose(); //如果在运作 销毁会报错
    }


}

public struct MyJob : IJobFor
{
    public NativeArray<int> nums;

    //Job实际会执行的函数   
    public void Execute(int index)
    {
        nums[index] = index;

        //index 第几个
        //Debug.Log("JOB线程：" + System.Threading.Thread.CurrentThread.ManagedThreadId+"_Index："+index);
    }
}

