using System.Collections;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHub : MonoBehaviour
{
    [SerializeField] private Image imgHp;
    [SerializeField] private Text gold, exp, hp, mp;
    [SerializeField] private RectTransform effect;
    [SerializeField] private RectTransform propsTrans;

    // Start is called before the first frame update
    void Start( )
    {
        MsgFire.On( GameEventName.ON_PLAYER_HP_CHANGE, OnPlayerHpChange );
        MsgFire.On( GameEventName.ON_PLAYER_MP_CHANGE, OnPlayerMpChange );
        MsgFire.On( GameEventName.ON_PLAYER_EXP_CHANGE, OnPlayerExpChange );
        MsgFire.On( GameEventName.ON_PLAYER_GOLD_CHANGE, OnPlayerGoldChange );
        MsgFire.On( GameEventName.ON_FLY_EXP, FlyExpEffect );
    }


    private void FlyExpEffect( object data, object expv )
    {
        Vector3 worldpos = ( Vector3 ) data;
        Vector2 screenpos = RectTransformUtility.WorldToScreenPoint( Camera.main, worldpos );
        if ( RectTransformUtility.ScreenPointToLocalPointInRectangle( ( RectTransform ) transform, screenpos, Camera.main, out var lp ) )
        {
            GameObject ef = GameObject.Instantiate( effect.gameObject, transform );
            var efrect = ef.transform as RectTransform;
            efrect.localPosition = lp;
            ef.GetComponent<TrailRenderer>( ).Clear( );


            if ( RectTransformUtility.ScreenPointToLocalPointInRectangle( ( RectTransform ) transform, exp.transform.position, Camera.main, out var lp2 ) )
            {
                StartCoroutine( Fly( efrect, lp2, (long) expv ) );
            }
        }

    }

    IEnumerator Fly( RectTransform ef, Vector3 localpos_target, long expValue )
    {

        var p0 = ef.localPosition;
        var p3 = localpos_target;

        var v = p3 - p0;
        var dir = v.normalized;
        var dis = v.magnitude;
        int f = Random.value < 0.5 ? -1 : 1;

        Vector3 p1 = p0 + Quaternion.AngleAxis( 120 * f, Vector3.forward ) * dir * dis * 0.333f;
        f *= -1;
        Vector3 p2 = p0 + Quaternion.AngleAxis( 10 * f, Vector3.forward ) * dir * dis * 0.666f;


        float tm = 0f;
        float dur = 2f;
        while ( tm < dur )
        {
            float t = tm / dur;
            Vector3 pos = BezierUtils.CalculateThreePowerBezierPoint( t, p0, p1, p2, p3 );
            ef.localPosition = pos;
            tm += Time.deltaTime;
            yield return 0;
        }

        exp.text = expValue.ToString( );
        Destroy( ef.gameObject );
    }

    //private void OnGUI( )
    //{
    //    Vector3 p0 = new Vector3(1,0,0);
    //    Vector3 p3 = new Vector3( 1, 10, 0 );


    //    var dir = ( p3 - p0 ).normalized;
    //    var dis = ( p3 - p0 ).magnitude;
    //    Vector3 p1 = p0 + Quaternion.AngleAxis( 120, Vector3.forward ) * dir * dis * 0.333f;
    //    Vector3 p2 = p0 + Quaternion.AngleAxis( -10, Vector3.forward ) * dir * dis * 0.666f;

    //    var points = BezierUtils.GetThreePowerBeizerList( p0, p1, p2, p3, 100 );
    //    for ( int i = 0; i < points.Length - 1; i++ )
    //    {
    //        Debug.DrawLine( points[ i ], points[ i + 1 ] );
    //    }
    //}

    private void OnDestroy( )
    {
        MsgFire.Off( this );
    }

    private void OnPlayerHpChange( object cur, object max )
    {
        float p = ( float ) cur / ( float ) max;
        imgHp.fillAmount = p;

        hp.text = ( ( int ) cur ).ToString( );
    }

    //TODO: 暂时没找到相同画风的蓝条, 待定
    private void OnPlayerMpChange( object cur, object max )
    {
        mp.text = ( ( int ) cur ).ToString( );
    }

    private void OnPlayerExpChange( object cur )
    {
    }

    private void OnPlayerGoldChange( object cur )
    {
        gold.text = ( ( int ) cur ).ToString( );
    }
}
