/**
 * 
 * class: EasyObjectPool
 * 
 * A lightweight object pool.
 * 
 * You need to create an object in the scene and then hang it.
 *
 * Support automatic capacity expansion.
 *  
 * Support recycling detection.
 * ————————————————
 * 版权声明：本文为CSDN博主「极客柒」的原创文章，遵循CC 4.0 BY-SA版权协议，转载请附上原文出处链接及本声明。
 * 原文链接：https://blog.csdn.net/qq_39162566/article/details/128290119
 * 
 */

using System.Collections.Generic;
using UnityEngine;
using PoolCore;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PoolManager : MonoBehaviour
{

    [SerializeField, Tooltip( "是否在Awake的时候进行预加载" )] private bool LoadOnAwake = true;
    private Dictionary<string, TransformPool> poolDict = new Dictionary<string, TransformPool>( );
    private static Dictionary<Transform, string> nameofDict = new Dictionary<Transform, string>( );
    private static PoolManager instance = null;
    public static PoolManager GetInstance( )
    {
        return instance;
    }


    private void Awake( )
    {
        if ( instance != null && instance != this )
        {
            GameObject.DestroyImmediate( instance );
            return;
        }
        instance = this;

        //预加载
        if ( LoadOnAwake )
        {
            PreloadInspectoryConfig( );
        }
    }

    /// <summary>
    /// 预加载面板配置
    /// </summary>
    public void PreloadInspectoryConfig( )
    {
        PoolSettingAsset settingAsset = Resources.Load<PoolSettingAsset>( "PoolSettingAsset" );
        if ( settingAsset != null && settingAsset.settings.Count > 0  )
        {
            var settings = settingAsset.settings;
            try
            {
                foreach ( var setting in settings )
                {
                    var go = AssetLoader.Load<GameObject>( setting.path );
                    Add( go, setting.count );
                }
                Resources.UnloadAsset( settingAsset );
            }
            catch( System.Exception e)
            {
                Debug.LogError( e );
            }
        }
    }

    /// <summary>
    /// 从对象池中拿一个闲置的对象
    /// </summary>
    /// <param name="key"></param>
    /// <returns>当返回null时 说明不存在这个预设的池子 你可以使用 GeneratePool 来添加一个新的池子 </returns>
    public Transform Spawn( string key )
    {

        TransformPool res = null;
        if ( poolDict.TryGetValue( key, out res ) )
        {
            Transform trans = res.Pop( );
            trans.gameObject.SetActive( true );
            return trans;
        }
#if UNITY_EDITOR
        else
        {
            Log.Error( $"PoolManager not contain: {key}" );
        }
#endif
        return null;
    }
    /// <summary>
    /// 回收一个对象到对象池中
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public bool Despawn( Transform obj )
    {
        if ( null == obj )
        {
#if UNITY_EDITOR || ENABLE_LOG
            throw new System.Exception( "Despawn obj is null" );
#else
            return false;
#endif
        }

        if ( nameofDict.TryGetValue( obj, out string name ) && poolDict.TryGetValue( name, out TransformPool res ) )
        {
            res.Push_Back( obj );
            return true;
        }
        else
        {
            //容错处理
#if UNITY_EDITOR || ENABLE_LOG
            obj.gameObject.SetActive( false );
            Debug.LogError( $"current object is not objectPool element: {obj.name}", obj.gameObject );
#else
            Destroy( obj.gameObject );
#endif
        }
        return false;
    }


    /// <summary>
    /// 将指定key的缓存池内所有的对象全部回收
    /// </summary>
    /// <param name="pool"></param>
    public void Despawn( string pool )
    {
        if ( poolDict.TryGetValue( pool, out TransformPool res ) )
        {
            res.Recycle( );
        }
#if UNITY_EDITOR || ENABLE_LOG
        else
        {
            Debug.LogError( $"exclusive pool: {pool}" );
        }
#endif
    }

    /// <summary>
    /// 延迟回收
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="delay"></param>
    public void Despawn( Transform obj, float delay )
    {
        Timer.SetTimeout( delay, ( ) => Despawn( obj ) );
    }

    /// <summary>
    /// 是否是对象池元素
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public bool Contains( Transform element )
    {
        TransformPool pool;
        if ( null != element && nameofDict.TryGetValue( element, out string name ) && poolDict.TryGetValue( name, out pool ) && pool.InSidePool( element ) )
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 回收自身所有的对象池元素 ( 包含自身 )
    /// </summary>
    /// <param name="root"></param>
    public void DespawnSelfAny<T>( Transform root ) where T : Component
    {
        T[] suspectObjects = root.GetComponentsInChildren<T>( );
        foreach ( var obj in suspectObjects )
        {
            if ( Contains( obj.transform ) )
            {
                Despawn( obj.transform );
            }
        }
    }


    /// <summary>
    /// 回收自己的子节点 如果子节点是对象池元素的话
    /// </summary>
    /// <param name="root"> 父节点 </param>
    /// <param name="includeSelf"> 本次回收是否包含父节点 </param>
    /// <param name="force"> true: 遍历所有的孩子节点  false: 仅遍历一层 </param>
    public void DespawnChildren( Transform root, bool includeSelf = false, bool force = false )
    {
        List<Transform> children = null;

        if ( force )
        {
            Transform[] suspectObjects = root.GetComponentsInChildren<Transform>( );
            children = new List<Transform>( suspectObjects );
            if ( !includeSelf ) children.Remove( root );

        }
        else
        {
            children = new List<Transform>( );
            if ( includeSelf )
            {
                children.Add( root );
            }
            foreach ( Transform child in root )
            {
                children.Add( child );
            }
        }

        foreach ( var child in children )
        {
            Despawn( child );
        }
    }

    /// <summary>
    /// 新增对象池
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="firstExpandCount"></param>
    public void Add( GameObject prefab, int firstExpandCount = 100 )
    {
        if ( prefab != null )
        {
            var key = prefab.name;
            if ( poolDict.ContainsKey( key ) )
            {
                Debug.LogError( $"Add Pool Error: pool name <{key}> already exist!" );
#if UNITY_EDITOR
                Selection.activeGameObject = prefab;
#endif
                return;
            }
            var pool = new TransformPool( prefab, transform, nameofDict );
            poolDict.Add( key, pool );
            pool.Reserve( firstExpandCount );
#if UNITY_EDITOR || ENABLE_LOG
            Debug.Log( $"<color=#00ff44>[EasyObjectPool]\t对象池创建成功: {key}\t当前闲置数量: {firstExpandCount}</color>" );
#endif
        }
        else
        {
            Debug.LogError( $"Add Pool Error: prefab is null" );
        }
    }


    /// <summary>
    /// 回收所有激活对象
    /// </summary>
    public void Recycle( )
    {
        foreach ( var pool in poolDict )
        {
            pool.Value.Recycle( );
        }
    }

}


public class ReadOnlyAttribute : PropertyAttribute
{

}

#if UNITY_EDITOR 
[CustomPropertyDrawer( typeof( ReadOnlyAttribute ) )]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight( SerializedProperty property, GUIContent label )
    {
        return EditorGUI.GetPropertyHeight( property, label, true );
    }

    public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
    {
        GUI.enabled = false;
        EditorGUI.PropertyField( position, property, label, true );
        GUI.enabled = true;
    }
}
#endif