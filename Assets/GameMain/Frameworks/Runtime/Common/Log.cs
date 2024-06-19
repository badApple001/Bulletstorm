using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

/**
 * 
 * 
 * Log Utils 
 * 
 * General log management
 * 
 * anchor: ChenJC
 * 
 * time: 2023/2/23
 * 
 */
public class Log
{


    [Conditional( "ENABLE_LOG" )]
    public static void Info( string msg, Object param = null )
    {
        if ( null == param )
        {
            Debug.Log( msg );
        }
        else
        {
            Debug.Log( msg, param );
        }
    }


    [Conditional( "ENABLE_LOG" )]
    public static void Info( object msg, Object param = null )
    {
        if ( null == param )
        {
            Debug.Log( msg );
        }
        else
        {
            Debug.Log( msg, param );
        }
    }


    [Conditional( "ENABLE_LOG" )]
    public static void Error( string msg, Object param = null )
    {
        if ( null == param )
        {
            Debug.LogError( msg );
        }
        else
        {
            Debug.LogError( msg, param );
        }
    }


    [Conditional( "ENABLE_LOG" )]
    public static void Error( object msg )
    {
        Debug.LogError( msg );
    }



    [Conditional( "ENABLE_LOG" )]
    public static void Assert( bool condition, string msg, Object param = null )
    {
        if ( null == param )
        {
            Debug.Assert( condition, msg );
        }
        else
        {
            Debug.Assert( condition, msg, param );
        }
    }


    [Conditional( "ENABLE_LOG" )]
    public static void Warning( string msg, Object param = null )
    {
        if ( null == param )
        {
            Debug.LogWarning( msg );
        }
        else
        {
            Debug.LogWarning( msg, param );
        }
    }


    [Conditional( "ENABLE_LOG" )]
    public static void Light( string msg, Object param = null )
    {
        string content = $"<color=#ffff00>{msg}</color>";
        if ( null == param )
        {
            Debug.Log( content );
        }
        else
        {
            Debug.Log( content, param );
        }
    }


    [Conditional( "ENABLE_LOG" )]
    public static void Green( string msg, Object param = null )
    {
        string content = $"<color=#00ff00>{msg}</color>";
        if ( null == param )
        {
            Debug.Log( content );
        }
        else
        {
            Debug.Log( content, param );
        }
    }


    // 
    //[Conditional( "ENABLE_LOG" )]
    /// <summary>
    /// 系统数据 任何情况下都必须输出 请不要随意调用这个方法 仅在重要的地方调用
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="param"></param>
    public static void PINK( string msg, Object param )
    {
        string content = $"<color=#EF47D1>{msg}</color>";
        Debug.Log( content, param );
    }
    /// <summary>
    /// 系统数据 任何情况下都必须输出 请不要随意调用这个方法 仅在重要的地方调用
    /// </summary>
    /// <param name="msg"></param>
    public static void PINK( string msg )
    {
        string content = $"<color=#EF47D1>♥ ♥ ♥ {msg}</color>";
        Debug.Log( content );
    }

    [Conditional( "SDK_LOG" )]
    public static void SDK( string msg, Object param = null )
    {
        string content = $"<color=#33ff33>{msg}</color>";
        if ( null == param )
        {
            Debug.Log( content );
        }
        else
        {
            Debug.Log( content, param );
        }
    }
}
