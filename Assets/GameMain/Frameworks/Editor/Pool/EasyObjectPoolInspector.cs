//==========================
// - FileName:      Assets/Frameworks/Editor/EasyObjectPoolInspector.cs
// - Created:       ChenJC	
// - CreateTime:    2023-06-19 10:03:20
// - UnityVersion:  2019.4.35f1
// - Version:       1.0
// - Description:   
//==========================
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using Object = UnityEngine.Object;

[CanEditMultipleObjects, CustomEditor( typeof( PoolManager ) )]
public class EasyObjectPoolInspector : Editor
{

    private Material material;
    private float horizontalIncrement = 100f;
    private float verticalIncrement = 100f;
    private PoolManager _target { get { return target as PoolManager; } }
    private SerializedProperty preloadConfigs_serializedProperty;
    private Dictionary<string, List<float>> lines = new Dictionary<string, List<float>>( );
    GUIStyle detailStyle;
    GUIStyle titleStyle;
    private const string preloadSettingFile = "Assets/Resources/PoolSettingAsset.asset";
    private PoolSettingAsset _asset;

    private void OnEnable( )
    {
        // Find the "Hidden/Internal-Colored" shader, and cache it for use.
        material = new Material( Shader.Find( "Hidden/Internal-Colored" ) );
        //line infos   
        lines.Clear( );
        //label font style
        detailStyle = new GUIStyle( );
        detailStyle.font = Font.CreateDynamicFontFromOSFont( "Consolas", 14 );
        detailStyle.fontStyle = FontStyle.Normal;
        titleStyle = new GUIStyle( );
        titleStyle.font = Font.CreateDynamicFontFromOSFont( "仿宋", 16 );
        titleStyle.fontStyle = FontStyle.Bold;
        titleStyle.normal.textColor = Color.white;
        _asset = LoadAsset( );
    }

    public override void OnInspectorGUI( )
    {
        if ( _asset == null )
        {
            return;
        }

        EditorGUILayout.PropertyField( serializedObject.FindProperty( "LoadOnAwake" ) );
        EditorGUILayout.LabelField( $"配置数量: {_asset.settings.Count}" );

        EditorGUILayout.BeginHorizontal( );
        if ( GUILayout.Button( "新增预设" ) )
        {
            _asset.settings.Add( new PoolSettingItem( )
            {
                count = 1
            } );
        }

        if ( GUILayout.Button( "清空预设" ) )
        {
            if ( EditorUtility.DisplayDialog( "危险操作", "删除所有配置, 你准备好了吗?", "Yes" ) )
            {
                if ( _asset.settings.Count > 0 )
                {
                    string debugStr = "clean list:\n";
                    foreach ( PoolSettingItem item in _asset.settings )
                    {
                        debugStr += $"{item.path}|{item.count}\n";
                    }
                    Debug.Log( debugStr );
                }
                _asset.settings.Clear( );
            }
        }

        EditorGUILayout.EndHorizontal( );

        for ( int i = 0; i < _asset.settings.Count; i++ )
        {
            var setting = _asset.settings[ i ];
            EditorGUILayout.BeginHorizontal( );
            EditorGUILayout.LabelField( "Preload", GUILayout.MaxWidth( 50 ) );
            if ( GUILayout.Button( "X" ) )
            {
                _asset.settings.RemoveAt( i );
                continue;
            }

            setting.count = EditorGUILayout.IntField( setting.count, GUILayout.MaxWidth( 50 ) );
            Object obj = string.IsNullOrEmpty( setting.path ) ? null : AssetDatabase.LoadAssetAtPath<GameObject>( setting.path );
            obj = EditorGUILayout.ObjectField( obj, typeof( GameObject ), false );
            if ( null != obj )
            {
                setting.path = AssetDatabase.GetAssetPath( obj );
            }
            EditorGUILayout.EndHorizontal( );
        }

        serializedObject.ApplyModifiedProperties( );


        if ( GUI.changed && null != _asset )
        {
            SaveAsset( _asset );
        }
    }

    public static PoolSettingAsset LoadAsset( )
    {
        PoolSettingAsset _asset;
        if ( !File.Exists( preloadSettingFile ) )
        {
            if ( !Directory.Exists( Path.GetDirectoryName( preloadSettingFile ) ) )
            {
                Directory.CreateDirectory( Path.GetDirectoryName( preloadSettingFile ) );
            }

            _asset = ScriptableObject.CreateInstance<PoolSettingAsset>( );
            _asset.createdTime = DateTime.Now.ToString( "F" );
            _asset.updatedTime = _asset.createdTime;
            AssetDatabase.CreateAsset( _asset, preloadSettingFile );
        }
        else
        {
            _asset = AssetDatabase.LoadAssetAtPath<PoolSettingAsset>( preloadSettingFile );
        }
        return _asset;
    }

    public static void SaveAsset( PoolSettingAsset asset )
    {
        asset.updatedTime = DateTime.Now.ToString( "F" );
        EditorUtility.SetDirty( asset );
        AssetDatabase.SaveAssetIfDirty( asset );
    }

}