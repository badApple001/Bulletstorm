using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public Transform target;
    public float followThreshold = 0.5f;
    public float followSpeed = 1f;
    public Vector2 minPos = new Vector2( -5, -5 );
    public Vector2 maxPos = new Vector2( 5, 5 );


    private void LateUpdate( )
    {
        if ( target != null )
        {
            Follow( );
        }
    }

    private void Follow( )
    {
        Vector3 dif = target.position + target.right * 3 - transform.position;
        dif.z = 0;
        if ( dif.magnitude > followThreshold )
        {
            Vector3 nextpos = transform.position + dif * Time.deltaTime * followSpeed;
            nextpos.x = Mathf.Clamp( nextpos.x, minPos.x, maxPos.x );
            nextpos.y = Mathf.Clamp( nextpos.y, minPos.y, maxPos.y );
            transform.position = nextpos;
        }
    }
}
