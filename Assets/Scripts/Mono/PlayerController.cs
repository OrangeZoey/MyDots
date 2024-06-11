using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEditor;
using UnityEngine;

public enum PlayerState
{
    Idle, Move
}

public class PlayerController : MonoBehaviour
{
    public Animator Animator;
    public float MoveSpeed; //移动速度
    public Vector2 MoveRangeX;//范围
    public Vector2 MoveRangeY;
    public Transform GunRoot;//枪械跟物体
    public int Lv;//等级
    public int BulletQuantity { get => Lv; } //子弹数量 
    public float AttackCD { get => Mathf.Clamp(1f / Lv * 1.5f, 0.1f, 1f); } //CD

    public int LV
    {
        get => Lv;
        set
        {
            Lv = value;
            // 因为等级导致的初始化数据
            SharedData.gameShareData.Data.spawnInterval = 10f / Lv * SpawnMonsterIntervalMultiply;
            SharedData.gameShareData.Data.spawnCount = (int)(Lv * 5 * SpawnMonsterQuantityMultiply);
        }
    }

    public float SpawnMonsterIntervalMultiply = 1;//怪物间隔
    public float SpawnMonsterQuantityMultiply = 1;//怪物数量
    private PlayerState playerState;
    public PlayerState PlayerState
    {
        get { return playerState; }
        set
        {
            playerState = value;
            switch (playerState)
            {
                case PlayerState.Idle:
                    PlayAnimation("Idle");
                    break;
                case PlayerState.Move:
                    PlayAnimation("Move");
                    break;
            }
        }
    }

    private void Awake()
    {
        LV = Lv; // 为了初始化
        CheckPositionRange();//一开始就同步一次玩家位置，如果不同步敌人的坐标会为负
    }

    private void Start()
    {
        PlayerState = PlayerState.Idle;//默认为待机
    }
    private void Update()
    {
        CheckAttack();

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        switch (playerState)
        {
            case PlayerState.Idle:
                if (h != 0 || v != 0) PlayerState = PlayerState.Move;
                break;
            case PlayerState.Move:
                if (h == 0 && v == 0)
                {
                    PlayerState = PlayerState.Idle;
                    return;
                }
                //移动
                transform.Translate(MoveSpeed * Time.deltaTime * new Vector3(h, v, 0));
                //范围
                CheckPositionRange();
                //方向
                if (h > 0) transform.localScale = Vector3.one;
                else if (h < 0) transform.localScale = new Vector3(-1, 1, 1);
                break;
        }
    }

    /// <summary>
    /// 限制玩家范围
    /// </summary>
    private void CheckPositionRange()
    {
       
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, MoveRangeX.x, MoveRangeX.y);
        pos.y = Mathf.Clamp(pos.y, MoveRangeY.x, MoveRangeY.y);
        pos.z = pos.y;
        transform.position = pos;
        SharedData.playerPos.Data = (Vector2)transform.position; //共享数据中保存玩家位置数据
    }

    /// <summary>
    /// 播放动画
    /// </summary>
    /// <param name="animationName"></param>
    public void PlayAnimation(string animationName)
    {
        //在固定时间内平滑过渡到一个新的动画状态。  animationName动画名称 0间隔时间
        Animator.CrossFadeInFixedTime(animationName, 0);
    }

    private float attackCDTimer;
    private void CheckAttack()
    {
        //屏幕转世界
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        GunRoot.up = (Vector2)mousePos - (Vector2)transform.position;

        //计时器
        attackCDTimer -= Time.deltaTime;
        if (attackCDTimer <= 0 && Input.GetMouseButton(0))
        {
            //攻击 发射子弹
            Attack();
            attackCDTimer = AttackCD;
        }
    }

    private void Attack()
    {
        AudioManager.Instance.PlayShootAudio();
        // 生成子弹信息
        DynamicBuffer<BulletCreateInfo> buffer = World.DefaultGameObjectInjectionWorld.EntityManager.GetBuffer<BulletCreateInfo>(SharedData.singtonEnitty.Data);
        buffer.Add(new BulletCreateInfo()
        {
            position = GunRoot.position,
            rotation = GunRoot.rotation,
        });
        //角度
        float angleStep = Mathf.Clamp(360 / BulletQuantity, 0, 5F);
        for (int i = 1; i < BulletQuantity / 2; i++)
        {
            //两边的子弹
            buffer.Add(new BulletCreateInfo()
            {
                position = GunRoot.position,
                rotation = GunRoot.rotation * Quaternion.Euler(0, 0, angleStep * i),
            });
            buffer.Add(new BulletCreateInfo()
            {
                position = GunRoot.position,
                rotation = GunRoot.rotation * Quaternion.Euler(0, 0, -angleStep * i),
            });
        }
    }
}
