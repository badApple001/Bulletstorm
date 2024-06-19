using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///
/// Timer 轻量高效定时器
/// 
/// Anchor: ChenJC
/// Time: 2022/10/09
/// Example: https://blog.csdn.net/qq_39162566/article/details/113105351
/// </summary>
public class Timer : MonoBehaviour
{

    //定时器数据类
    public class TimerTask
    {
        public ulong ID;
        public float lifeCycle;
        public float expirationTime;
        public long times;
        public Action func;

        //你可以通过此方法来获取定时器的运行进度  0 ~ 1  1.0表示即将要调用func
        //你可以通过 GetTimer( id ) 获取当前Task的Clone体 
        public float progress
        {
            get
            {
                return 1.0f - Mathf.Clamp01( ( expirationTime - Time.time ) / lifeCycle );
            }
        }

        //获取一个当前TimerTask的副本出来
        public TimerTask Clone( )
        {
            var timerTask = new TimerTask( );
            timerTask.ID = ID;
            timerTask.expirationTime = expirationTime;
            timerTask.lifeCycle = lifeCycle;
            timerTask.times = times;
            timerTask.func = func;
            return timerTask;
        }

        //释放回收当前定时器
        public void Destory( )
        {
            freeTaskCls.Enqueue( this );
        }

        //刷新
        public void Refresh( )
        {
            expirationTime = Time.time + lifeCycle;
        }
    }


    #region Member property

    protected static List<TimerTask> activeTaskCls = new List<TimerTask>( );//激活中的TimerTask对象
    protected static Queue<TimerTask> freeTaskCls = new Queue<TimerTask>( );//闲置TimerTask对象
    protected static HashSet<Action> lateChannel = new HashSet<Action>( );//确保callLate调用的唯一性
    protected static ulong timerID = 1000; //timer的唯一标识

    #endregion


    #region Enable timer methods

    //每帧结束时执行回调 : 当前帧内的多次调用仅在当前帧结束的时候执行一次
    public static void CallerLate( Action func )
    {
        if ( !lateChannel.Contains( func ) )
        {
            lateChannel.Add( func );
            SetTimeout( 0f, func );
        }
    }


    //delay秒后 执行一次回调
    public static ulong SetTimeout( float delay, Action func )
    {
        return SetInterval( delay, func, false, 1 );
    }

    /// <summary>
    /// 周期性定时器 间隔一段时间调用一次
    /// </summary>
    /// <param name="interval"> 间隔时长: 秒</param>
    /// <param name="func"> 调用的方法回调 </param>
    /// <param name="immediate"> 是否立即执行一次 </param>
    /// <param name="times"> 调用的次数: 默认永久循环 当值<=0时会一直更新调用 当值>0时 循环指定次数后 停止调用 </param>
    /// <returns></returns>
    public static ulong SetInterval( float interval, Action func, bool immediate = false, int times = 0 )
    {
        //从free池中 获取一个闲置的TimerTask对象
        var timer = GetFreeTimerTask( );
        timer.lifeCycle = interval;
        timer.Refresh( );
        timer.func = func;
        timer.times = times;
        timer.ID = ++timerID;

        //立即执行一次
        if ( immediate )
        {
            --timer.times;
            func?.Invoke( );
            if ( timer.times == 0 )
            {

                timer.Destory( );
            }
            else
            {
                //添加到激活池中
                activeTaskCls.Add( timer );
            }
        }
        else
        {
            //添加到激活池中
            activeTaskCls.Add( timer );
        }

        return timerID;
    }

    #endregion


    #region Get Timer methods

    /// <summary>
    /// 通过Tag获取定时器对象
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    public static TimerTask GetTimer( ulong ID )
    {
        return activeTaskCls.Find( ( TimerTask t ) =>
        {
            return t.ID == ID;
        } )?.Clone( );
    }

    /// <summary>
    /// 通过方法获取定时器对象
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static List<TimerTask> GetTimer( Action func )
    {
        return activeTaskCls.FindAll( t =>
        {
            return t.func == func;
        } );
    }

    /// <summary>
    /// 通过对用对象获取定时器对象
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static List<TimerTask> GetTimer( object target )
    {
        return activeTaskCls.FindAll( t =>
        {
            return t.func.Target == target;
        } );
    }
    #endregion


    #region Clean Timer methods

    /// <summary>
    /// 通过ID 清理定时器
    /// </summary>
    /// <param name="ID">定时器标签</param>
    /// <returns></returns>
    public static void ClearTimer( ulong ID )
    {
        int index = activeTaskCls.FindIndex( ( TimerTask t ) =>
        {
            return t.ID == ID;
        } );

        if ( index != -1 )
        {
            var timerTask = activeTaskCls[ index ];
            ClearTimers( new List<TimerTask>( ) { timerTask } );
        }
    }


    /// <summary>
    /// 通过类型来ClearTimer
    /// @ps: 移除同类型的所有成员方法定时器  包含( lambda 和 其它类实体 )
    /// </summary>
    /// <param name="clsType"></param>
    public static void ClearTimer<T>( )
    {
        var type = typeof( T );
        var clsName = type.FullName;

        var allMatchTask = activeTaskCls.FindAll( t =>
        {
            if ( null != t.func && null != t.func.Target )
            {
                var fullname = t.func.Target.GetType( ).FullName;
                var currentClsNameClip = fullname.Split( '+' );
                if ( currentClsNameClip.Length > 0 )
                {
                    if ( currentClsNameClip[ 0 ] == clsName )
                    {
                        return true;
                    }
                }
            }
            return false;
        } );

        ClearTimers( allMatchTask );
    }

    /// <summary>
    /// 通过方法 清理定时器
    /// </summary>
    /// <param name="func">处理方法</param>
    /// <returns></returns>
    public static void ClearTimer( Action func )
    {
        var allMatchTask = activeTaskCls.FindAll( t => t.func == func );
        ClearTimers( allMatchTask );
    }


    /// <summary>
    /// 清理当前类的所有方法
    /// 
    /// 支持清理类的成员方法相关定时器清理 也包含有限lambda的释放
    /// 
    /// 支持的lambda：
    /// 
    /// 
    /// 
    //class Mtest
    //{
    //    string str_n = "123";
    //    public Mtest()
    //    {
    //        test(); //此方法内的闭包调用支持
    //        test2(); //此方法内的闭包调用不支持
    //        Timer.SetInterval( 1f,UpdatePerSecond ); //成员方法支持
    //    }
    //    //这个方法内的闭包支持
    //    private void test()
    //    {
    //        Timer.SetTimeout( 1.0f, () =>
    //        {
    //            //在lambda内部定义的变量
    //            string t = "12313213";
    //            //在lambda内部访问和修改类成员变量行为
    //            str_n = t;
    //        } );
    //    }
    //    //类成员支持
    //    private void UpdatePerSecond()
    //    {
    //        Debug.Log( Time.realtimeSinceStartup );
    //    }
    //    //这个方法内的闭包不支持
    //    private void test2()
    //    {
    //        //在lambda外定义的变量
    //        string t = "12313213";
    //        Timer.SetTimeout( 1.0f, () =>
    //        {
    //            //在lambda内部访问lambda外部的变量行为会让当前闭包的 Target变成一个新的类
    //            t = "1231";
    //            //在lambda内部访问和修改类成员变量行为
    //            str_n = t;
    //        } );
    //    }
    //    //清理这个类的所有定时器调度
    //    private void clearTime()
    //    {
    //        Timer.ClearTimer( this );
    //    }
    //}
    /// 
    /// 
    /// </summary>
    /// <param name="func">处理方法</param>
    /// <returns></returns>
    public static void ClearTimer( object target )
    {
        var allMatchTask = activeTaskCls.FindAll( t => t.func.Target == target );
        ClearTimers( allMatchTask );
    }


    /// <summary>
    /// 清理所有定时器
    /// </summary>
    public static void ClearTimers( )
    {
        lateChannel.Clear( );
        activeTaskCls.ForEach( timer => freeTaskCls.Enqueue( timer ) );
        activeTaskCls.Clear( );
    }


    /// <summary>
    /// 批量清理定时器
    /// </summary>
    /// <param name="allMatchTask"></param>
    public static void ClearTimers( List<TimerTask> allMatchTask )
    {
        allMatchTask?.ForEach( task =>
        {
            if ( lateChannel.Count != 0 && lateChannel.Contains( task.func ) )
            {
                lateChannel.Remove( task.func );
            }
            activeTaskCls.Remove( task );
            freeTaskCls.Enqueue( task );
        } );
    }

    #endregion


    #region System methods

    [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSceneLoad )]
    static void Init( )
    {
        activeTaskCls.Clear( );
        freeTaskCls.Clear( );
        lateChannel.Clear( );
        timerID = 1000;
        DontDestroyOnLoad( new GameObject( "TimerManager", typeof( Timer ) ) );
    }

    private void Awake( )
    {
        StartCoroutine( TimeElapse( ) );
    }

   
    private IEnumerator TimeElapse( )
    {
        TimerTask t = null;
        while ( true )
        {
            if ( activeTaskCls.Count > 0 )
            {
                float time = Time.time;
                for ( int i = 0; i < activeTaskCls.Count; ++i )
                {
                    t = activeTaskCls[ i ];

                    if ( t.expirationTime <= time )
                    {
                        if ( t.times == 1 )
                        {
                            activeTaskCls.RemoveAt( i-- );
                            t.Destory( );

                            if ( lateChannel.Count != 0 && lateChannel.Contains( t.func ) )
                            {
                                lateChannel.Remove( t.func );
                            }
                        }

                        t.Refresh( );
                        --t.times;
                        t.func( );
                    }
                }
            }
            yield return 0;
        }
    }

  
    protected static TimerTask GetFreeTimerTask( )
    {
        if ( freeTaskCls.Count > 0 )
        {
            return freeTaskCls.Dequeue( );
        }
        return new TimerTask( );
    }

    #endregion

}





