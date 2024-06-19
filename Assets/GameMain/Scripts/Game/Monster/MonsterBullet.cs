using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBullet : MonoBehaviour
{

    public float bulletRadius = 1.2f;
    public Vector2 offset = Vector2.zero;
    public float lifeTime = 2f;
    public float flySpeed = 2f;
    public float harm = 5;
    public GameObject explodePrefab;
    public LayerMask checkLayer;

    private float tm = 0;
    private Vector3 dir = Vector3.zero;
    private bool isTrace = false;
    private Transform target = null;
    private Vector3 nextpos = Vector3.zero;

    private void OnDrawGizmos( )
    {
        GizmosUtil.DrawCircle( transform.localToWorldMatrix, bulletRadius / transform.localScale.x, offset, Color.red );
    }

    public void Init( Vector3 dir, bool isTrace = false, Transform target = null )
    {
        this.dir = dir;
        this.isTrace = isTrace;
        this.target = target;
        tm = 0;
        if ( this.isTrace && this.target == null )
        {
            Debug.LogError( "追踪子弹必须要有目标" );
        }
        CalcNextPos( );
    }


    private void Update( )
    {
        tm += Time.deltaTime;
        if ( tm >= lifeTime )
        {
            ObjectPool.Instance.PushObject( gameObject );
            if ( null != explodePrefab )
            {
                var obj = ObjectPool.Instance.GetObject( explodePrefab );
                obj.transform.position = transform.position;
            }
        }

        if ( isTrace )
        {
            dir = ( target.position - transform.position ).normalized;
        }
        CalcNextPos( );
    }


    private void CalcNextPos( )
    {
        dir.z = 0;
        nextpos = transform.position + dir * Time.deltaTime * flySpeed;

        float zAngle = Mathf.Atan2( dir.y, dir.x ) * Mathf.Rad2Deg;
        transform.localEulerAngles = Vector3.forward * zAngle;
    }


    private void FixedUpdate( )
    {
        var prePos = transform.position;
        var dir = nextpos - prePos;
        var result = Physics2D.CircleCast( prePos, bulletRadius, dir.normalized, dir.magnitude, checkLayer );
        var collider = result.collider;
        if ( collider != null )
        {
            ObjectPool.Instance.PushObject( gameObject );
            if ( collider.TryGetComponent<Creature>( out var creature ) )
            {
                float realharm = harm * creature.defense;
                creature.Hp -= realharm;
                //飘字
                MsgFire.Event(
                       GameEventName.ON_FLY_HARMTEXT,
                       creature.transform.position + Vector3.down * 0.4f, isTrace ? HarmText.HarmType.magic : HarmText.HarmType.normal,
                       ( int ) realharm );
            }
        }
        transform.position = nextpos;
    }
}
