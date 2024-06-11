using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using Unity.Collections;
using System;

public class TestJobSystem : MonoBehaviour
{
    //Run() �����߳�ִ��
    //Schedule() ��һ�������߳�ִ��ȫ������ 
    //ScheduleParallel() ��x�������߳��� ��ɸ��Ե����񣬲���

    private MyJob job;
    JobHandle jobHandle;
    private NativeArray<int> nums;//Unity.Collections;������� ���������ָ������� ��Ȼ�ǽṹ�嵫�Ǵ�ֵ����������仯 ���������ڲ��߱���ṹ������������ֵ������
    void Start()
    {
        //����ԭ������洢����  (�����С��������ѡ�ָ������η���������ڴ棬Temp��ʾ��ʱ���飩)
        //NativeArray<int>nums= new NativeArray<int>(10,Allocator.Temp);

        //��ȡ��ǰ����ִ�е��̵߳��й��߳�ID
        Debug.Log("���̣߳�" + System.Threading.Thread.CurrentThread.ManagedThreadId);

        nums = new NativeArray<int>(100, Allocator.Persistent);//�־�����
        job = new MyJob() { nums = nums };
        jobHandle = job.ScheduleParallel(100, 10, default);//һ�������10������ ÿ�����ڲ�ͬ�����߳�ִ��
        jobHandle.Complete();//ϣ������������job

        //�������Ҫ��
        //for (int i = 0; i < nums.Length; i++)
        //{
        //    Debug.Log(nums[i]);
        //}

        //nums.Dispose(); //��������� ���ٻᱨ��

        //Invoke("MyTest", 3);
    }

    public void Update()
    {
        //��������
        if(jobHandle.IsCompleted && jobHandle!=default)
        {
            jobHandle.Complete();
            for (int i = 0; i < nums.Length; i++)
            {
                Debug.Log(nums[i]);
            }

            nums.Dispose(); //��������� ���ٻᱨ��
            jobHandle = default;//default �� IsCompleted Ϊtrue
        }
    }

    public void MyTest()
    {
        for (int i = 0; i < nums.Length; i++)
        {
            Debug.Log(nums[i]);
        }

        nums.Dispose(); //��������� ���ٻᱨ��
    }


}

public struct MyJob : IJobFor
{
    public NativeArray<int> nums;

    //Jobʵ�ʻ�ִ�еĺ���   
    public void Execute(int index)
    {
        nums[index] = index;

        //index �ڼ���
        //Debug.Log("JOB�̣߳�" + System.Threading.Thread.CurrentThread.ManagedThreadId+"_Index��"+index);
    }
}

