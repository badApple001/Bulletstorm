using Pathfinding;
using System;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent( typeof( Creature ) )]
public class Monster : MonoBehaviour, IPlayerPositionChangeEvent
{

    public enum MonsterState
    {
        Idle, //空闲状态
        Hit,//被攻击
        FollowTarget, //追踪敌人状态
        Attack, //攻击状态
        Patrol, //巡逻状态
        GoHome,//回到领地中心点
        Death,//死亡状态
    }
    public enum MonsterType
    {
        //战士 / 近战
        Warrior,

        //射手 / 法师
        Shooter,
    }


    [HideInInspector] public Vector3 m_territorialCenter = Vector3.zero;
    /*[Header( "领地范围" )] */
    public float m_territorialRange = 4f;
    /*[Header( "视野范围" )] */
    public float m_filedOfView = 3.93f;
    /*[Header( "攻击伤害" )] */
    public float m_attackHarm = 2f;
    public float m_attackAnimationTime = 0.6f;
    /*[Header( "移动速度" )] */
    public float m_moveSpeed = 2f;
    /*[Header( "攻击距离" )] */
    public float m_atkDistance = 1f;
    /*[Header( "死亡颜色" )] */
    public Color m_deathColor = Color.white;
    /*[Header( "怪物类型" )] */
    public MonsterType m_monsterType = MonsterType.Warrior;
    /*[Header( "子弹预设" )] */
    public GameObject m_bulletPrefab;
    public Transform m_muzzle = null;//射手的枪口位置
    public bool m_isTrace = false;//是否是跟踪子弹
    /*[Header( "玩家状态" )]*/
    /*[ReadOnly][Tooltip( "调试" )]*/
    public MonsterState m_currentState = MonsterState.Idle;



    private static PlayerControl g_playerControl;
    private Creature creature;
    private Transform playerTrans;
    private Dictionary<MonsterState, Action<MonsterState>> stateCallback = new Dictionary<MonsterState, Action<MonsterState>>( );
    private Animator animator;
    private IAstarAI ai;
    private float flipY, flipX;

    private void Awake( )
    {
        stateCallback[ MonsterState.Idle ] = OnEnterIdleState;
        stateCallback[ MonsterState.FollowTarget ] = OnEnterFollowTargetState;
        stateCallback[ MonsterState.Attack ] = OnEnterAttackState;
        stateCallback[ MonsterState.Patrol ] = OnEnterPatrolState;
        stateCallback[ MonsterState.Death ] = OnEnterDeathState;
        stateCallback[ MonsterState.GoHome ] = OnEnterGoHomeState;
        stateCallback[ MonsterState.Hit ] = OnEnterHitState;

        //GameWatcher.ListenPlayerPositionEvent( this );
    }

    private void OnDrawGizmos( )
    {
        if ( transform == null ) return;

        float scale = transform.localScale.x;

        //绘制视野范围
        GizmosUtil.DrawCircle( transform.localToWorldMatrix, m_filedOfView / scale, Vector2.zero, Color.red );
        //绘制领地范围
        if ( !Application.isPlaying )
        {
            GizmosUtil.DrawCircle( transform.localToWorldMatrix, m_territorialRange / scale, Vector2.zero, Color.yellow );
        }
    }

    public void OnPlayerPositionChange( Vector3 newPos )
    {
        //    var dir = newPos - transform.position;
        //    switch ( currentState )
        //    {
        //        case MonsterState.Patrol:
        //        case MonsterState.Idle:
        //            {
        //                Debug.Log( dir.magnitude );
        //                if ( dir.magnitude <= filedOfView && ( transform.position - territorialCenter ).magnitude < territorialRange )
        //                {
        //                    SetState( MonsterState.FollowTarget );
        //                }
        //            }
        //            break;
        //        case MonsterState.Attack:

        //            break;
        //    }
    }


    private void Start( )
    {
        animator = transform.GetChild( 0 ).GetComponent<Animator>( );
        ai = GetComponent<IAstarAI>( );
        creature = GetComponent<Creature>( );
        m_territorialCenter = transform.position;

        ai.maxSpeed = m_moveSpeed;
        flipX = transform.lossyScale.x;
        flipY = transform.lossyScale.y;

        //缓存玩家的Trans节点 独一份
        if ( null == g_playerControl )
        {
            g_playerControl = GameObject.FindObjectOfType<PlayerControl>( );
        }
        playerTrans = g_playerControl.transform;

        //监听生物事件
        creature.OnHpChange = OnHpChange;
        creature.OnMpChange = OnMpChange;

        //注册动画事件
        ReigisteAnimationEvent( );
    }

    //动画实在太多了,所以在下就决定用代码来绑定好了, 这样一劳永逸
    private void ReigisteAnimationEvent( )
    {
        AnimatorUtils.AddAnimationEvent( animator, "Attack", "Fire", m_attackAnimationTime, "OnAttack" );
        transform.GetChild( 0 ).GetComponent<AnimateEventHandler>( ).Listen( "OnAttack", OnAttack );
    }

    private void OnMpChange( float cur, float max )
    {

    }

    private void OnHpChange( float cur, float max )
    {

        if ( cur == 0 )
        {
            SetState( MonsterState.Death );
        }
        else
        {

            animator.CrossFade( "Hit", 0.2f );
            SetState( MonsterState.Hit );
        }
    }

    private void Update( )
    {
        var dir = playerTrans == null ? Vector3.zero : playerTrans.position - transform.position;
        switch ( m_currentState )
        {
            case MonsterState.Idle:
                {
                    if ( dir.magnitude <= m_filedOfView )
                    {
                        SetState( MonsterState.FollowTarget );
                    }
                }
                break;
            case MonsterState.FollowTarget:

                //敌人寻路位置在玩家同条水平线的侧边
                //举个例子, 玩家的位置 x:0,y:0
                //如果敌人是从左边走过来,那它的位置就是 x:-0.4,y:0.4
                //反之 x:0.4,y:0.4
                //y也减0.4是啥意思呢? 因为玩家的脚底是需要减少0.4, 其它怪物一样保持以脚底为原点算坐标,保持在统一水平线上
                ai.destination = playerTrans.position + dir.normalized * -m_atkDistance;
                if ( ai.reachedDestination || dir.magnitude <= m_atkDistance )
                {
                    SetState( MonsterState.Attack );
                }
                else
                {
                    transform.localScale = new Vector3( ai.velocity.x > 0 ? -flipX : flipX, flipY, 1 );

                    if ( dir.magnitude > m_filedOfView )
                    {
                        SetState( MonsterState.GoHome );
                    }
                }
                break;
            case MonsterState.GoHome:
                if ( ai.reachedDestination )
                {
                    SetState( MonsterState.Idle );
                }
                else
                {
                    transform.localScale = new Vector3( ai.velocity.x > 0 ? -flipX : flipX, flipY, 1 );

                }
                break;
            case MonsterState.Attack:
                {
                    if ( dir.magnitude > m_atkDistance )
                    {
                        SetState( MonsterState.FollowTarget );
                    }
                }
                break;
            default:
                break;
        }
    }

    public void SetState( MonsterState newState )
    {
        if ( newState != m_currentState )
        {
            var oldState = m_currentState;
            m_currentState = newState;
            stateCallback[ newState ]( oldState );
        }
    }

    protected virtual void OnEnterIdleState( MonsterState oldState )
    {
        //animator.SetBool( "move", false );
        //animator.SetBool( "atk", false );
        animator.CrossFade( "Idle", 0.2f );
    }

    protected virtual void OnEnterHitState( MonsterState oldState )
    {
        Timer.SetTimeout( 0.2f, LateCallFindTarget );
    }
    private void LateCallFindTarget( )
    {
        if ( m_currentState != MonsterState.Death )
        {
            SetState( MonsterState.FollowTarget );
        }
    }

    protected virtual void OnEnterFollowTargetState( MonsterState oldState )
    {
        //animator.SetBool( "atk", false );
        //animator.SetBool( "move", true );
        animator.CrossFade( "Move", 0.2f );
        StartSearchPath( );
    }
    protected virtual void OnEnterAttackState( MonsterState oldState )
    {
        //animator.SetBool( "move", false );
        //animator.SetBool( "atk", true );
        animator.CrossFade( "Attack", 0.2f );
        ai.isStopped = true;
    }
    protected virtual void OnEnterPatrolState( MonsterState oldState )
    {
        //animator.SetBool( "move", true );
        animator.CrossFade( "Move", 0.2f );
        StartSearchPath( );
    }

    protected virtual void OnEnterDeathState( MonsterState oldState )
    {
        //animator.SetBool( "die", true );
        animator.CrossFade( oldState != MonsterState.Idle ? "MoveDie" : "Die", 0.2f );
        ai.isStopped = true;
        GetComponent<Collider2D>( ).enabled = false;
        Timer.SetTimeout( 1f, SetDeathColor );
    }
    private void SetDeathColor( )
    {
        if ( TryGetComponent<SpriteRenderer>( out var sr ) )
        {
            sr.color = m_deathColor;
        }

        //这里是从字典里取哈, 不用担心性能
        DiskAgent.Load<GameMain.GameData>( ).exp += creature.GetExp( );
        MsgFire.Event( GameEventName.ON_FLY_EXP, transform.position, DiskAgent.Load<GameMain.GameData>( ).exp );
    }

    protected virtual void OnEnterGoHomeState( MonsterState oldState )
    {
        //animator.SetBool( "move", true );
        animator.CrossFade( "Move", 0.2f );
        ai.destination = m_territorialCenter;
        StartSearchPath( );
    }

    private void StartSearchPath( )
    {
        if ( playerTrans != null )
        {
            ai.isStopped = false;
            Timer.SetTimeout( 0.12f, ai.SearchPath );
        }
    }

    public void OnAttack( )
    {


        var dir = playerTrans.position - transform.position;

        //近战处理方式
        if ( m_monsterType == MonsterType.Warrior )
        {
            if ( dir.magnitude <= m_atkDistance )
            {
                //测试demo临时加, 后续再加到表里
                int critRate = UnityEngine.Random.value < 0.1f ? 2 : 1;
                float realHarm = m_attackHarm * critRate * g_playerControl.creature.defense;
                g_playerControl.creature.Hp -= realHarm;
                //飘字
                MsgFire.Event(
                       GameEventName.ON_FLY_HARMTEXT,
                       transform.position, critRate > 1 ? HarmText.HarmType.crit : HarmText.HarmType.normal,
                       ( int ) realHarm );
            }
        }
        else
        {
            //远程的发射子弹
            var obj = ObjectPool.Instance.GetObject( m_bulletPrefab );
            obj.transform.position = m_muzzle.position;
            obj.GetComponent<MonsterBullet>( ).Init( dir.normalized );
        }
    }


    private void OnDestroy( )
    {
        Timer.ClearTimer( this );
        AnimatorUtils.CleanAllEvent( animator );
    }
}
