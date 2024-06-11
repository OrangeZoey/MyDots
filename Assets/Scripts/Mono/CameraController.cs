using System.Collections;
using UnityEngine;
public class CameraController : MonoBehaviour
{
    public Transform target; //玩家
    public Vector3 offset;  //偏移量
    public float smooth;   //平滑程度
    private Vector3 velocity; //速度
    public Vector2 xRange; //范围
    public Vector2 yRange;
    void Update()
    {
        if (target != null)
        {
            //用于平滑地改变一个向量的方法
            Vector3 pos = Vector3.SmoothDamp(transform.position, target.position + offset, ref velocity, Time.deltaTime * smooth);
            SetPosition(pos);
        }
    }

    /// <summary>
    /// 限制范围
    /// </summary>
    /// <param name="pos"></param>
    private void SetPosition(Vector3 pos)
    {
        pos.x = Mathf.Clamp(pos.x, xRange.x, xRange.y);
        pos.y = Mathf.Clamp(pos.y, yRange.x, yRange.y);
        pos.z = -10;
        transform.position = pos;
    }
}