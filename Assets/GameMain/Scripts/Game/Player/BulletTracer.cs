using System.Collections;
using UnityEngine;

public class BulletTracer : MonoBehaviour
{
    public float fadeSpeed;
    private LineRenderer line;
    private float alpha;
    [SerializeField] private GameObject explosionPrefab;

    private void Awake( )
    {
        line = GetComponent<LineRenderer>( );
        alpha = line.endColor.a;
    }

    private void OnEnable( )
    {
        line.endColor = new Color( line.endColor.r, line.endColor.g, line.endColor.b, alpha );
        StartCoroutine( Fade( ) );
    }

    IEnumerator Fade( )
    {
        while ( line.endColor.a > 0 )
        {
            line.endColor = new Color( line.endColor.r, line.endColor.g, line.endColor.b, line.endColor.a - fadeSpeed );
            yield return new WaitForFixedUpdate( );
        }

        GameObject exp = ObjectPool.Instance.GetObject( explosionPrefab );
        exp.transform.position = line.GetPosition( 1 );
        ObjectPool.Instance.PushObject( gameObject );
    }
}
