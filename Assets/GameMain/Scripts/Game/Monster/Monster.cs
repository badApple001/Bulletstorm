using Pathfinding;
using System;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent( typeof( Creature ) )]
public class Monster : MonoBehaviour, IPlayerPositionChangeEvent
{

    public enum MonsterState
    {
        Idle, //����״̬
        Hit,//������
        FollowTarget, //׷�ٵ���״̬
        Attack, //����״̬
        Patrol, //Ѳ��״̬
        GoHome,//�ص�������ĵ�
        Death,//����״̬
    }

    public enum MonsterType
    {
        //սʿ / ��ս
        Warrior,

        //���� / ��ʦ
        Shooter,
    }


    [HideInInspector] public Vector3 m_territorialCenter = Vector3.zero;
    /*[Header( "��ط�Χ" )] */
    public float m_territorialRange = 4f;
    /*[Header( "��Ұ��Χ" )] */
    public float m_filedOfView = 3.93f;
    /*[Header( "�����˺�" )] */
    public float m_attackHarm = 2f;
    public float m_attackAnimationTime = 0.6f;
    /*[Header( "�ƶ��ٶ�" )] */
    public float m_moveSpeed = 2f;
    /*[Header( "��������" )] */
    public float m_atkDistance = 1f;
    /*[Header( "������ɫ" )] */
    public Color m_deathColor = Color.white;
    /*[Header( "��������" )] */
    public MonsterType m_monsterType = MonsterType.Warrior;
    /*[Header( "�ӵ�Ԥ��" )] */
    public GameObject m_bulletPrefab;
    public Transform m_muzzle = null;//���ֵ�ǹ��λ��
    public bool m_isTrace = false;//�Ƿ��Ǹ����ӵ�
    /*[Header( "���״̬" )]*/
    /*[ReadOnly][Tooltip( "����" )]*/
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

        //������Ұ��Χ
        GizmosUtil.DrawCircle( transform.localToWorldMatrix, m_filedOfView / scale, Vector2.zero, Color.red );
        
        //������ط�Χ
        if ( !Application.isPlaying )
        {
            GizmosUtil.DrawCircle( transform.localToWorldMatrix, m_territorialRange / scale, Vector2.zero, Color.blue );
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

        //������ҵ�Trans�ڵ� ��һ��
        if ( null == g_playerControl )
        {
            g_playerControl = GameObject.FindObjectOfType<PlayerControl>( );
        }
        playerTrans = g_playerControl.transform;

        //���������¼�
        creature.OnHpChange = OnHpChange;
        creature.OnMpChange = OnMpChange;

        //ע�ᶯ���¼�
        ReigisteAnimationEvent( );
    }

    //����ʵ��̫����,�������¾;����ô������󶨺���, ����һ������
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
                    if ( dir.magnitude < m_filedOfView )
                    {
                        SetState( MonsterState.FollowTarget );
                    }
                }
                break;
            case MonsterState.FollowTarget:

                //����Ѱ·λ�������ͬ��ˮƽ�ߵĲ��
                //�ٸ�����, ��ҵ�λ�� x:0,y:0
                //��������Ǵ�����߹���,������λ�þ��� x:-0.4,y:0.4
                //��֮ x:0.4,y:0.4
                //yҲ��0.4��ɶ��˼��? ��Ϊ��ҵĽŵ�����Ҫ����0.4, ��������һ�������Խŵ�Ϊԭ��������,������ͳһˮƽ����
                ai.destination = playerTrans.position;
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

        var dir = playerTrans.position - transform.position;
        transform.localScale = new Vector3( dir.x > 0 ? -flipX : flipX, flipY, 1 );
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

        //�����Ǵ��ֵ���ȡ��, ���õ�������
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
            Timer.SetTimeout( 0.1f, ai.SearchPath );
        }
    }

    public void OnAttack( )
    {

        var dir = playerTrans.position - transform.position;

        //��ս������ʽ
        if ( m_monsterType == MonsterType.Warrior )
        {
            if ( dir.magnitude <= m_atkDistance )
            {
                //����demo��ʱ��, �����ټӵ�����
                int critRate = UnityEngine.Random.value < 0.1f ? 2 : 1;
                float realHarm = m_attackHarm * critRate * g_playerControl.creature.defense;
                g_playerControl.creature.Hp -= realHarm;

                //Ʈ��
                MsgFire.Event(
                       GameEventName.ON_FLY_HARMTEXT,
                       transform.position, critRate > 1 ? HarmText.HarmType.crit : HarmText.HarmType.normal,
                       ( int ) realHarm );
            }
        }
        else
        {

            //Զ�̵ķ����ӵ�
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
