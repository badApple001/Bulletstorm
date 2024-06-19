using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// UIForm扩展接口
/// 扩展后支持 UIManager.Create<T>( object arg ) 方法
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IUIForm<T>
{
    void OnMessage( T data );
}

/// <summary>
/// UIManager 管理UI模块 
/// 支持自动释放 内存管理 
/// 支持Create接口 缓存处理
/// 支持Close接口 延迟惰性回收
/// 支持Find接口 通过类型获取
/// 支持Destory接口 立即释放
/// </summary>
public class UIManager
{
    private static Dictionary<Type, UIHandler> cacheForms = new Dictionary<Type, UIHandler>( );
    private static float default_ui_destruction_delay_time = 60 * 5; //关闭后多久自动销毁 小游戏建议直接设置 float.MaxValue
    private static string ui_assetbundle_path;
    public static Camera uiCamera { get; private set; } //提供一个全局的UI相机引用
    public static Transform uiRoot { get; private set; } //提供一个全局的UI root节点


    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="ui_root"> UI的ROOT节点 </param>
    /// <param name="ui_assetbundle_path"> UI预设目录 </param>
    /// <param name="default_ui_destruction_delay_time"> UI默认关闭后多少秒进行销毁 </param>
    public static void Init( Transform ui_root, string ui_assetbundle_path = "Assets/AssetBundleResources/UI", float default_ui_destruction_delay_time = 5 * 60f )
    {
        UIManager.uiRoot = ui_root;
        UIManager.ui_assetbundle_path = ui_assetbundle_path;
        UIManager.default_ui_destruction_delay_time = default_ui_destruction_delay_time;

        if ( ui_root.TryGetComponent<Canvas>( out var canvas ) && canvas.worldCamera != null )
        {
            uiCamera = canvas.worldCamera;
        }
        else
        {
            canvas = ui_root.GetComponentInParent<Canvas>( );
            if ( canvas != null && canvas.worldCamera != null )
            {
                uiCamera = canvas.worldCamera;
                uiRoot = canvas.transform;
            }
            else if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                uiCamera = null;
                uiRoot = canvas.transform;
            }
            else
            {
                var allCanvas = GameObject.FindObjectsOfType<Canvas>( );
                if ( allCanvas.Length > 0 )
                {
                    Canvas hasCameraCanvas = Array.Find<Canvas>( allCanvas, c =>
                    {
                        return c.worldCamera != null;
                    } );

                    if ( null != hasCameraCanvas )
                    {
                        uiCamera = hasCameraCanvas.worldCamera;
                        uiRoot = hasCameraCanvas.transform;
                        return;
                    }

                    //确保uiRoot指向一个Canvas节点
                    uiRoot = allCanvas[ 0 ].transform;
                }

                var cameras = GameObject.FindObjectsOfType<Camera>( );
                if ( cameras.Length > 1 )
                {
                    Array.Sort( cameras, ( Camera a, Camera b ) =>
                    {
                        return b.depth.CompareTo( a.depth );
                    } );
                }

                Debug.Assert( cameras.Length > 0, "[UIManager] must be at least one camera in the scene!" );
                uiCamera = cameras[ 0 ];
            }
        }
    }

    public enum UIState
    {
        Open,
        Close,
        UndefinedOrDestoryed,
    }

    private class UIHandler
    {
        public Component p;
        private float expirationTime;
        public UIState state;
        public float destructionTime;

        public UIHandler( )
        {
            destructionTime = default_ui_destruction_delay_time;
        }

        public void Refresh( )
        {
            expirationTime = destructionTime + Time.time;
        }
        public bool HasExpire( )
        {
            return Time.time > expirationTime;
        }
    }

    /// <summary>
    /// 打开或创建一个界面对象
    /// 
    /// 如果对象没有被释放 则打开缓存中的实例 并将对象窗口置前
    /// 
    /// </summary>
    /// <typeparam name="T"> T的类型应该要和本地存储的UI预设名称保持一致 </typeparam>
    /// <returns></returns>
    public static T Create<T>( ) where T : Component
    {
        Type t = typeof( T );
        if ( !cacheForms.TryGetValue( t, out UIHandler handler ) )
        {

            cacheForms.Add( t, handler = new UIHandler( ) );
            handler.state = UIState.UndefinedOrDestoryed;
        }

        if ( handler.state == UIState.Open )
        {
            return ( T ) handler.p;
        }

        if ( handler.state == UIState.UndefinedOrDestoryed )
        {
            var instance = GameObject.Instantiate( AssetLoader.Load<GameObject>( $"{ui_assetbundle_path}/{typeof( T ).Name}.prefab" ), uiRoot );
            T component = instance.GetComponent<T>( );
            handler.p = component;
        }
        else
        {
            handler.p.gameObject.SetActive( true );
            handler.p.transform.SetAsLastSibling( );
        }

        handler.Refresh( );
        handler.state = UIState.Open;
        return ( T ) handler.p;
    }

    /// <summary>
    /// 使用带参数的Create方法 需要扩展接口 IUIForm
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="arg"></param>
    /// <returns></returns>
    public static T Create<T, W>( W arg ) where T : Component, IUIForm<W>
    {
        Type t = typeof( T );
        if ( !cacheForms.TryGetValue( t, out UIHandler handler ) )
        {

            cacheForms.Add( t, handler = new UIHandler( ) );
            handler.state = UIState.UndefinedOrDestoryed;
        }

        if ( handler.state == UIState.Open )
        {
            return ( T ) handler.p;
        }

        if ( handler.state == UIState.UndefinedOrDestoryed )
        {
            var instance = GameObject.Instantiate( AssetLoader.Load<GameObject>( $"{ui_assetbundle_path}/{typeof( T ).Name}.prefab" ), uiRoot );
            T component = instance.GetComponent<T>( );
            handler.p = component;
        }
        else
        {
            handler.p.gameObject.SetActive( true );
            handler.p.transform.SetAsLastSibling( );
        }

        //t.GetMethod( "UpdateData", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public )?.Invoke( handler.p, new object[] { arg } );
        handler.p.SendMessage( "OnMessage", arg, SendMessageOptions.DontRequireReceiver );
        handler.Refresh( );
        handler.state = UIState.Open;
        return ( T ) handler.p;
    }

    /// <summary>
    /// 查找一个窗口类 如果对象被释放或者从未被创建 返回null
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T Find<T>( ) where T : Component
    {
        if ( cacheForms.TryGetValue( typeof( T ), out UIHandler v ) )
        {
            v.Refresh( );
            return ( T ) v.p;
        }
        return null;
    }

    /// <summary>
    /// 获取窗口的状态
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static UIState GetUIState<T>( ) where T : Component
    {
        if ( cacheForms.TryGetValue( typeof( T ), out UIHandler v ) )
        {
            v.Refresh( );
            return v.state;
        }
        return UIState.UndefinedOrDestoryed;
    }


    /// <summary>
    /// 关闭窗口 默认后台缓存一段时间 在此期间 仅设置Active状态为False
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="destructionDelay">关闭后多久进行回收处理</param>
    public static void Close<T>( float destructionDelay = -1f ) where T : Component
    {
        if ( cacheForms.TryGetValue( typeof( T ), out UIHandler v ) )
        {
            if ( v.state == UIState.Close || v.state == UIState.UndefinedOrDestoryed )
            {
                return;
            }
            v.destructionTime = destructionDelay < 0f ? default_ui_destruction_delay_time : destructionDelay;
            v.Refresh( );
            v.state = UIState.Close;
            v.p?.gameObject.SetActive( false );
            EnableUpdate( );
        }
    }


    /// <summary>
    /// 立即释放一个窗口  当前帧只做状态修改 并隐藏对象  下一帧将回收 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static void Destory<T>( ) where T : Component
    {
        if ( cacheForms.TryGetValue( typeof( T ), out UIHandler v ) )
        {
            if ( v.state == UIState.UndefinedOrDestoryed )
            {
                return;
            }
            v.Refresh( );
            v.state = UIState.Close;
            v.p?.gameObject.SetActive( false );
            if ( !destoryQueue.Contains( v ) )
            {
                destoryQueue.Enqueue( v );
                EnableUpdate( );
            }
        }
    }


    private static Queue<UIHandler> destoryQueue = new Queue<UIHandler>( );
    private static ulong updateUITimerID = 0;
    private static void EnableUpdate( )
    {
        if ( updateUITimerID != 0 )
        {
            return;
        }
        updateUITimerID = Timer.SetInterval( 1e-1f, UpdateUIState );
    }

    private static void UpdateUIState( )
    {
        //每帧最多销毁一个 分帧处理销毁任务
        //while ( destoryQueue.Count > 0 )
        if ( destoryQueue.Count > 0 )
        {
            UIHandler hander = destoryQueue.Dequeue( );
            if ( hander.state != UIState.Open )
            {
                hander.state = UIState.UndefinedOrDestoryed;
                if ( null != hander.p && hander.p.gameObject )
                {
                    GameObject.Destroy( hander.p.gameObject );
                    hander.p = null;
                }
            }
        }

        bool closeState = false;
        foreach ( var handler in cacheForms.Values )
        {
            if ( handler.state == UIState.Close && handler.HasExpire( ) )
            {
                destoryQueue.Enqueue( handler );
                closeState = true;
            }
        }

        if ( !closeState && destoryQueue.Count == 0 )
        {
            Timer.ClearTimer( updateUITimerID );
            updateUITimerID = 0;
        }
    }

}
