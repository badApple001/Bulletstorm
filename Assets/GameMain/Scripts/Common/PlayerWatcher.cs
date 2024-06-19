using BulletHell;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerPositionChangeEvent
{
    void OnPlayerPositionChange( Vector3 newPos );
}



public class PlayerWatcher
{
    private static List<IPlayerPositionChangeEvent> listeners = new List<IPlayerPositionChangeEvent>( );

    public static void ListenPlayerPositionEvent( IPlayerPositionChangeEvent listener )
    {
        if ( !listeners.Contains( listener ) )
        {
            listeners.Add( listener );
        }
    }

    public static void UnListenPlayerPositionEvent( IPlayerPositionChangeEvent listener )
    {
        if ( listeners.Contains( listener ) )
        {
            listeners.Remove( listener );
        }
    }

    public static void DispathPlayerPositionEvent( Vector3 newPos )
    {
        for ( int i = 0; i < listeners.Count; i++ )
        {
            if ( listeners[ i ] != null )
            {
                listeners[ i ].OnPlayerPositionChange( newPos );
            }
        }
    }



}
