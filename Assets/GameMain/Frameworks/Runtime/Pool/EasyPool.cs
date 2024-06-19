using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 通用对象池 非UnityTransform管理池  如果你想使用优化更好 更完善的池子 
/// 请用 EasyObjectPool 本框架已兼容 
/// 官网文档 https://blog.csdn.net/qq_39162566/article/details/128290119
/// </summary>
/// <typeparam name="T"></typeparam>
public class EasyPool<T> where T : class, new()
{

    private Queue<T> freeClsQueue = new Queue<T>( );//队列
    private HashSet<T> insideFreeSet = new HashSet<T>( );//增加一个HashSet来提升查找性能 因为Queue本质也是一个List 查找太低效了
    private Action<T> InitHandler, ReleaseHandler;
    private PoolSpawnFunctorType<T> GenerateHandler;
    public delegate E PoolSpawnFunctorType<E>( );
    private float lastExtendTime = 0; //上一次扩容的时间
    private UInt64 capacity = 1;//容量

    /// <summary>
    /// 创建一个简单的动态扩容对象池 
    /// </summary>
    /// <param name="prefillCount"> 首次扩容的数量 </param>
    /// <param name="onInit"> 对象拿出来初始化时的操作 </param>
    /// <param name="onRelease"> 对象回收时应该干嘛  </param>
    /// <param name="howGenerate"> 怎么生成 默认就是 new一个 你可以自定义这个方法来重写对象的实例方式  </param>
    public EasyPool( UInt32 prefillCount = 0, Action<T> onInit = null, Action<T> onRelease = null, PoolSpawnFunctorType<T> howGenerate = null )
    {
        //注册回收和初始化
        InitHandler = onInit;
        ReleaseHandler = onRelease;
        GenerateHandler = howGenerate;

        //首次扩容
        Extend( prefillCount );
    }

    /// <summary>
    /// 释放并回收一个对象
    /// </summary>
    /// <param name="clsInstance"></param>
    public void Free( T clsInstance )
    {
        Log.Assert( null != clsInstance, "The freed object must not be null" );
        if ( !insideFreeSet.Contains( clsInstance ) )
        {
            ReleaseHandler?.Invoke( clsInstance );
            freeClsQueue.Enqueue( clsInstance );
            insideFreeSet.Add( clsInstance );
        }
    }

    /// <summary>
    /// 获取一个自由对象 当内存不足时 自动扩容
    /// </summary>
    /// <returns></returns>
    public T Get( )
    {
        if ( freeClsQueue.Count > 0 )
        {
            var cls = freeClsQueue.Dequeue( );
            if ( insideFreeSet.Contains( cls ) )
            {
                insideFreeSet.Remove( cls );
            }
            InitHandler?.Invoke( cls );
            return cls;
        }
        AutoExtend( );
        return Get( );
    }

    /// <summary>
    /// 扩容 默认不用 
    /// 如果你觉得可能会出现溢出现象 可以提前预扩容 避免动态扩容可能存在的卡顿问题
    /// </summary>
    /// <param name="extendCount"></param>
    public void Extend( UInt32 extendCount )
    {
        capacity += extendCount;
        for ( UInt32 i = 0; i < extendCount; i++ )
        {
            Free( GenerateHandler != null ? GenerateHandler( ) : new T( ) );
        }
    }

    private void AutoExtend( )
    {
        if ( Time.time - lastExtendTime > 1e-2f )
        {
            Extend( 1 );
        }
        else
        {
            Extend( ( UInt32 ) capacity );
        }
        lastExtendTime = Time.time;
    }

    /// <summary>
    /// 清理对象池
    /// </summary>
    /// <param name="foreach_release"> 是否对空闲对象进行回收 </param>
    /// <param name="custom_release_handle"> 回收处理的方法 - 自定义    默认使用 ReleaseHandler </param>
    public void Clear( bool foreach_release = false, Action<T> custom_release_handle = null )
    {
        if ( foreach_release )
        {
            if ( custom_release_handle == null )
            {
                custom_release_handle = ReleaseHandler;
            }

            if ( custom_release_handle != null )
            {
                foreach ( var obj in freeClsQueue )
                {
                    custom_release_handle( obj );
                }
            }
        }

        freeClsQueue.Clear( );
        insideFreeSet.Clear( );
    }
}
