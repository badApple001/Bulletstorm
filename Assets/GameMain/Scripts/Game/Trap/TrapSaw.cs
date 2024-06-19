using System.Collections.Generic;
using UnityEngine;

public class TrapSaw : MonoBehaviour
{
    public float harmInterval = 0.1f;
    public float harm = 10f;
    List<Creature> creatures = new List<Creature>( );


    private void Start( )
    {
        Timer.SetInterval( harmInterval, ProcessHarm );
    }

    private void ProcessHarm( )
    {
        Creature creature = null;
        for ( int i = 0; i < creatures.Count; i++ )
        {
            creature = creatures[ i ];
            if ( creature == null )
            {
                creatures.RemoveAt( i-- );
                continue;
            }
            creature.Hp -= harm * creature.defense;
        }
    }

    private void OnDestroy( )
    {
        Timer.ClearTimer( ProcessHarm );
    }

    private void OnTriggerEnter2D( Collider2D collision )
    {

        if ( collision.TryGetComponent<Creature>( out var creature ) && !creatures.Contains(creature) )
        {
            creatures.Add( creature );
        }

    }

    private void OnTriggerExit2D( Collider2D collision )
    {
        if ( collision.TryGetComponent<Creature>( out var creature ) && creatures.Contains( creature ) )
        {
            creatures.Remove( creature );
        }
    }
}
