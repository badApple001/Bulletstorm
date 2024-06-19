using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pit : MonoBehaviour
{
    public BoxCollider2D collider2D;
    public Transform bottom;
    Animator[] pitAnimes;
    bool open = false;
    HashSet<Collider2D> pitObjs = new HashSet<Collider2D>( );
    private void Start( )
    {
        pitAnimes = GetComponentsInChildren<Animator>( );
        Timer.SetInterval( 2f, UpdatePit );
    }

    private void UpdatePit( )
    {
        open = !open;
        if ( open )
        {
            foreach ( var anima in pitAnimes )
            {
                anima.enabled = true;
            }
            collider2D.enabled = true;
        }
        else
        {
            collider2D.enabled = false;
            foreach ( var anima in pitAnimes )
            {
                var clips = anima.GetCurrentAnimatorClipInfo( 0 );
                anima.Play( clips[ 0 ].clip.name, 0, 0f );
                anima.Update( Time.deltaTime );
                anima.enabled = false;
            }
        }
    }

    private void OnTriggerEnter2D( Collider2D collision )
    {
        if ( open && !pitObjs.Contains( collision ) && collision.TryGetComponent<Creature>( out var creature ) )
        {
            pitObjs.Add( collision );
            var mons = creature.GetComponents<MonoBehaviour>( );
            foreach ( var mon in mons )
            {
                mon.enabled = false;
            }
            if ( creature.TryGetComponent<Rigidbody2D>( out var rg ) )
            {
                rg.velocity = Vector3.zero;
            }
            if ( creature.TryGetComponent<TrailRenderer>( out var trail ) )
            {
                trail.Clear( );
                trail.enabled = false;
            }
            StartCoroutine( Drop( creature.transform ) );

            Timer.SetTimeout( 0.2f, ( ) =>
            {
                creature.Hp = 0;
            } );
        }
    }

    private IEnumerator Drop( Transform creature )
    {
        float tm = 0f;
        var srcpos = creature.position;
        var despos = bottom.position;
        while ( tm < 0.5f )
        {
            tm += Time.deltaTime;
            creature.localScale = Vector3.one * ( 1f - tm / 0.5f );
            creature.position = Vector3.Lerp( srcpos, despos, tm / 0.5f );
            creature.Rotate( Vector3.forward * Time.deltaTime );
            yield return null;
        }

        Destroy( creature.gameObject );
    }

    private void OnDestroy( )
    {
        Timer.ClearTimer<Pit>( );
    }
}
