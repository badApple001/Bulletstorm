using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketLauncher : Gun
{
    public int rocketNum = 3;
    public float rocketAngle = 15;

    protected override void Fire( )
    {
        animator.SetTrigger( "Shoot" );
        StartCoroutine( DelayFire( .2f ) );
    }

    IEnumerator DelayFire( float delay )
    {
        yield return new WaitForSeconds( delay );

        Vector2 pos = new Vector2( transform.position.x, transform.position.y );
        Vector2 dir = mousePos - pos;
        float distance = Mathf.Min( dir.magnitude, 5.78f );
        Vector2 target = pos + dir.normalized * distance;
        int median = rocketNum / 2;
        for ( int i = 0; i < rocketNum; i++ )
        {
            GameObject bullet = ObjectPool.Instance.GetObject( bulletPrefab );
            bullet.transform.position = muzzlePos.position;

            if ( rocketNum % 2 == 1 )
            {
                bullet.transform.right = Quaternion.AngleAxis( rocketAngle * ( i - median ), Vector3.forward ) * direction;
            }
            else
            {
                bullet.transform.right = Quaternion.AngleAxis( rocketAngle * ( i - median ) + rocketAngle / 2, Vector3.forward ) * direction;
            }
            bullet.GetComponent<Rocket>( ).SetTarget( target );
        }
    }
}
