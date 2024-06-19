//==========================
// - FileName:      Assets/Frameworks/Scripts/Event/Fire.cs
// - Created:       ChenJC	
// - CreateTime:    2023-06-19 10:10:45
// - UnityVersion:  2019.4.35f1
// - Version:       1.0
// - Description:   
//==========================
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;



/**
 * 
 * class: Fire
 *      Listen&Dispatch Event for global-environment  
 * 
 * Declaration:
 *      Lightweight and efficient event distribution management class.
 *      You can listen for events via on and do not need to worry about the possibility of repeated listener.
 *      You can also use objects to remove all related listener.
 * 
 * Function: 
 *      On: Add Event Handler 
 *      Off: Remove Event Handler
 *      Event: Dispatch Event
 * 
 * 
 * Anchor: ChenJC
 * Time: 2022/10/09
 * Source: https://blog.csdn.net/qq_39162566/article/details/113106880
 * 
 */
public class MsgFire
{

    private static Dictionary<string, List<Delegate>> eventPool = new Dictionary<string, List<Delegate>>( );
    private static HashSet<Delegate> once = new HashSet<Delegate>( );

    #region Add Event Listener Methods

    public static void On( string eventID, Action func )
    {
        if ( !Has( eventID, func ) )
        {
            List<Delegate> result = null;
            if ( eventPool.TryGetValue( eventID, out result ) )
            {
                result.Add( func );
            }
            else
            {
                eventPool.Add( eventID, new List<Delegate>( ) { func } );
            }
        }
#if UNITY_EDITOR
        else
        {
            Debug.Log( $"<color=#ff0000>----------- Listener Has already exist: {eventID},{func.ToString( )}</color>" );
        }
#endif
    }

    public static void On( string eventID, Action<object> func )
    {

        if ( !Has( eventID, func ) )
        {
            List<Delegate> result = null;
            if ( eventPool.TryGetValue( eventID, out result ) )
            {
                result.Add( func );
            }
            else
            {
                eventPool.Add( eventID, new List<Delegate>( ) { func } );
            }
        }
#if UNITY_EDITOR
        else
        {
            Debug.Log( $"<color=#ff0000>----------- Listener Has already exist: {eventID},{func.ToString( )}</color>" );
        }
#endif

    }

    public static void On( string eventID, Action<object, object> func )
    {

        if ( !Has( eventID, func ) )
        {
            List<Delegate> result = null;
            if ( eventPool.TryGetValue( eventID, out result ) )
            {
                result.Add( func );
            }
            else
            {
                eventPool.Add( eventID, new List<Delegate>( ) { func } );
            }
        }
#if UNITY_EDITOR
        else
        {
            Debug.Log( $"<color=#ff0000>----------- Listener Has already exist: {eventID},{func.ToString( )}</color>" );
        }
#endif

    }

    public static void On( string eventID, Action<object, object, object> func )
    {

        if ( !Has( eventID, func ) )
        {
            List<Delegate> result = null;
            if ( eventPool.TryGetValue( eventID, out result ) )
            {
                result.Add( func );
            }
            else
            {
                eventPool.Add( eventID, new List<Delegate>( ) { func } );
            }
        }
#if UNITY_EDITOR
        else
        {
            Debug.Log( $"<color=#ff0000>----------- Listener Has already exist: {eventID},{func.ToString( )}</color>" );
        }
#endif

    }
    public static void Once( string eventID, Action func )
    {
        once.Add( func );
        On( eventID, func );
    }
    public static void Once( string eventID, Action<object> func )
    {
        once.Add( func );
        On( eventID, func );
    }
    public static void Once( string eventID, Action<object, object> func )
    {
        once.Add( func );
        On( eventID, func );
    }
    public static void Once( string eventID, Action<object, object, object> func )
    {
        once.Add( func );
        On( eventID, func );
    }
    #endregion

    #region Find Methods
    /// <summary>
    ///
    /// declara:
    /// find method by methodInfo and caller
    /// 
    /// Example:
    /// var _delegate = Find( eventID, func.Method, func.Target );
    ///
    /// </summary>
    /// <param name="eventID"></param>
    /// <param name="method"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static Delegate Find( string eventID, MethodInfo method, object target )
    {
        List<Delegate> handlers = null;
        if ( eventPool.TryGetValue( eventID, out handlers ) )
        {
            return handlers.Find( handler => { return handler.Method == method && handler.Target == target; } );
        }
        return null;
    }
    #endregion

    #region Check Methods 

    public static bool Has( string eventID, Action func )
    {

        return Find( eventID, func.Method, func.Target ) != null;
    }
    public static bool Has( string eventID, Action<object> func )
    {

        return Find( eventID, func.Method, func.Target ) != null;
    }
    public static bool Has( string eventID, Action<object, object> func )
    {
        return Find( eventID, func.Method, func.Target ) != null;
    }
    public static bool Has( string eventID, Action<object, object, object> func )
    {
        return Find( eventID, func.Method, func.Target ) != null;
    }

    #endregion

    #region  Remove Event Listener Methods

    public static void Off( )
    {
        eventPool.Clear( );
    }
    public static void Off( string eventID )
    {
        eventPool.Remove( eventID );
    }
    public static void Off( object caller )
    {
        var clsName = caller.GetType( ).FullName;
        foreach ( var pair in eventPool )
        {
            for ( int i = 0; i < pair.Value.Count; i++ )
            {
                if ( pair.Value[ i ].Target != null )
                {
                    var fullname = pair.Value[ i ].Target.GetType( ).FullName;
                    var currentClsNameClip = fullname.Split( '+' );
                    if ( currentClsNameClip.Length > 0 && currentClsNameClip[ 0 ] == clsName )
                    {
                        pair.Value.RemoveAt( i-- );
                    }
                }
            }
        }
    }
    public static void Off( string eventID, Action func )
    {
        List<Delegate> result = null;
        if ( eventPool.TryGetValue( eventID, out result ) )
        {
            int index = result.FindIndex( handler => { return handler.Equals( func ); } );
            if ( -1 != index )
            {
                result.RemoveAt( index );
            }
        }

    }

    #endregion

    #region dispath event methods

    public static void Event( string eventID )
    {
        List<Delegate> handlers = null;
        if ( eventPool.TryGetValue( eventID, out handlers ) )
        {
            Delegate handler = null;
            for ( int i = 0; i < handlers.Count; i++ )
            {
                handler = handlers[ i ];
                Action func = handler as Action;
                func?.Invoke( );
                if ( once.Contains( func ) )
                {
                    handlers.RemoveAt( i-- );
                }
            }
        }
    }

    public static void Event( string eventID, object arg )
    {

        List<Delegate> handlers = null;
        if ( eventPool.TryGetValue( eventID, out handlers ) )
        {
            Delegate handler = null;
            for ( int i = 0; i < handlers.Count; i++ )
            {
                handler = handlers[ i ];
                Action<object> func = handler as Action<object>;
                func?.Invoke( arg );
                if ( once.Contains( func ) )
                {
                    handlers.RemoveAt( i-- );
                }
            }

        }
    }

    public static void Event( string eventID, object arg1, object arg2 )
    {
        List<Delegate> handlers = null;
        if ( eventPool.TryGetValue( eventID, out handlers ) )
        {
            Delegate handler = null;
            for ( int i = 0; i < handlers.Count; i++ )
            {
                handler = handlers[ i ];
                Action<object, object> func = handler as Action<object, object>;
                func?.Invoke( arg1, arg2 );
                if ( once.Contains( func ) )
                {
                    handlers.RemoveAt( i-- );
                }
            }
        }
    }

    public static void Event( string eventID, object arg1, object arg2, object arg3 )
    {
        List<Delegate> handlers = null;
        if ( eventPool.TryGetValue( eventID, out handlers ) )
        {
            Delegate handler = null;
            for ( int i = 0; i < handlers.Count; i++ )
            {
                handler = handlers[ i ];
                Action<object, object, object> func = handler as Action<object, object, object>;
                func?.Invoke( arg1, arg2, arg3 );
                if ( once.Contains( func ) )
                {
                    handlers.RemoveAt( i-- );
                }
            }
        }
    }

    #endregion


    //#region MainThreadCall

    //public struct AsyncCallMessage
    //{
    //    public string eventId;
    //    public object arg1;
    //}

    //public static Queue<AsyncCallMessage> asyncCalls = new Queue<AsyncCallMessage>( );
    //private static bool PollingEnabled = false;

    ///// <summary>
    ///// 异步派发事件, 事件将在主线程下一帧中触发, 确保线程安全
    ///// </summary>
    ///// <param name="eventId"></param>
    ///// <param name="arg"></param>
    //public static void AsyncEventForMainThread( string eventId, object arg )
    //{
    //    asyncCalls.Enqueue( new AsyncCallMessage( )
    //    {
    //        eventId = eventId,
    //        arg1 = arg
    //    } );
    //    if ( !PollingEnabled )
    //    {
    //        var timerMgr = GameObject.FindObjectOfType<Timer>( );
    //        if ( null != timerMgr )
    //        {
    //            timerMgr.mainThreadFrameHook += PollingUpdate;
    //            PollingEnabled = true;
    //        }
    //    }
    //}

    //private static void PollingUpdate( )
    //{
    //    while ( asyncCalls.Count != 0 )
    //    {
    //        AsyncCallMessage message = asyncCalls.Dequeue( );
    //        Event( message.eventId, message.arg1 );
    //    }
    //}

    //#endregion


}