//==========================
// - FileName:      Assets/Frameworks/Editor/AudioManagerInspector.cs      
// - Created:       ChenJC	
// - CreateTime:    2023-06-29-16:06:51
// - UnityVersion:  2021.3.22f1
// - Version:       1.0
// - Description:   
//==========================
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using static AudioManager;
using Object = UnityEngine.Object;

[CanEditMultipleObjects, CustomEditor( typeof( AudioManager ) )]
public class AudioManagerInspector : Editor
{
    private AudioManager _target { get { return target as AudioManager; } }
    private Vector2 settingScrollerPosition = Vector2.zero;
    private const string preloadSettingFile = "Assets/Resources/AudioSettingAsset.asset";
    private AudioSettingAsset _asset;
    private string lastUpdateTime = string.Empty;
    private void OnEnable( )
    {
        _asset = LoadAsset( );
        lastUpdateTime = _asset.updatedTime;
    }

    public override void OnInspectorGUI( )
    {
        base.OnInspectorGUI( );
        if ( Application.isPlaying )
        {
            EditorGUILayout.LabelField( "Runtime model - 禁止修改配置" );
            return;
        }

        if ( null != _asset )
        {

            if ( GUILayout.Button( "新增音效" ) )
            {
                _asset.settings.Insert( 0, new AudioSettingItem( )
                {
                    url = "",
                    name = ""
                } );
            }

            settingScrollerPosition = EditorGUILayout.BeginScrollView( settingScrollerPosition );
            for ( int i = 0; i < _asset.settings.Count; i++ )
            {
                GUILayout.BeginHorizontal( );
                if ( GUILayout.Button( "-" ) )
                {
                    _asset.settings.RemoveAt( i );
                    break;
                }

                var setting = _asset.settings[ i ];
                AudioClip obj = string.IsNullOrWhiteSpace( setting.url ) ? null : AssetDatabase.LoadAssetAtPath<AudioClip>( setting.url );
                obj = ( AudioClip ) EditorGUILayout.ObjectField( obj, typeof( AudioClip ), false, GUILayout.MaxWidth( 200 ) );
                setting.name = EditorGUILayout.TextField( setting.name );
                if ( GUI.changed && obj != null )
                {
                    string url = AssetDatabase.GetAssetPath( obj );
                    if ( setting.url != url )
                    {
                        setting.url = url;
                        setting.name = Path.GetFileNameWithoutExtension( setting.url );
                    }
                }
                GUILayout.EndHorizontal( );
            }
            EditorGUILayout.EndScrollView( );
            serializedObject.ApplyModifiedProperties( );
        }

        if ( GUI.changed && null != _asset )
        {
            SaveAsset( _asset );
        }
    }

    private void OnDisable( )
    {
        if ( !Application.isPlaying && null != _asset && _asset.updatedTime != lastUpdateTime )
        {
            lastUpdateTime = _asset.updatedTime;
            AssetDatabase.Refresh( );
        }
    }

    [MenuItem( "Assets/🐕🐕🐕‍\U0001f9ba🐩🐶/添加当前文件夹下的音效文件到 AssetManager上" )]
    public static void AddAllSoundEffectsUnderTheFolder( )
    {

        string[] guids = Selection.assetGUIDs;
        if ( guids == null || guids.Length == 0 )
        {
            Log.Info( $"{DateTime.Now.ToLocalTime( )}\tPlease select a folder or select a music file" );
            return;
        }

        List<FileInfo> fileInfos = new List<FileInfo>( );
        foreach ( var guid in guids )
        {
            string assetPath = AssetDatabase.GUIDToAssetPath( guid );
            if ( Directory.Exists( assetPath ) )
            {
                ForeachFile( assetPath, fileInfos, "*.mp3" );
                ForeachFile( assetPath, fileInfos, "*.wav" );
            }
            else if ( File.Exists( assetPath ) && ( assetPath.EndsWith( ".mp3" ) || assetPath.EndsWith( ".wav" ) ) )
            {
                fileInfos.Add( new FileInfo( assetPath ) );
            }
        }

        Log.PINK( $"Found music file {fileInfos.Count}" );

        //load assets
        var _asset = LoadAsset( );

        //filter and remove empty file
        int removeEmptyFileCount = _asset.settings.RemoveAll( asi =>
        {
            bool flag = string.IsNullOrWhiteSpace( asi.url );
            if ( flag )
            {
                Log.PINK( $"Remove empty file: {asi.name}" );
            }
            return flag;
        } );

        //removed duplicat element
        var oldSettings = _asset.settings;
        HashSet<string> ids = new HashSet<string>( );
        _asset.settings = new List<AudioSettingItem>( );
        foreach ( var setting in oldSettings )
        {
            if ( !ids.Contains( setting.name ) )
            {
                _asset.settings.Add( setting );
                ids.Add( setting.name );
            }
            else
            {
                Log.PINK( $"Remove duplicat audioItem: {setting.name}|{setting.GetHashCode( )}" );
            }
        }
        int removeDuplicatCount = oldSettings.Count - _asset.settings.Count;

        int added = 0, replace = 0;
        if ( null != _asset )
        {
            foreach ( var fileInfo in fileInfos )
            {
                var name = Path.GetFileNameWithoutExtension( fileInfo.Name );
                var someSoundEffect = _asset.settings.Find( asi =>
                {
                    return asi.name == name;
                } );

                string fullname = fileInfo.FullName.Replace( "\\", "/" );
                fullname = fullname.Replace( Application.dataPath.Remove( Application.dataPath.Length - 6 ), "" );
                if ( someSoundEffect != null )
                {
                    someSoundEffect.url = fullname;
                    ++replace;
                }
                else
                {
                    AudioSettingItem item = new AudioSettingItem( );
                    item.url = fullname;
                    item.name = name;
                    _asset.settings.Add( item );
                    ++added;
                }
            }

            SaveAsset( _asset );
            AssetDatabase.Refresh( );
        }

        Log.PINK( $"===============Done===============" );
        Log.PINK( $"筛选空文件: {removeEmptyFileCount}=============" );
        Log.PINK( $"筛选重复文件: {removeDuplicatCount}============" );
        Log.PINK( $"覆盖更新文件: {replace}===========" );
        Log.PINK( $"新增文件: {added}==============" );
    }

    public static AudioSettingAsset LoadAsset( )
    {
        AudioSettingAsset _asset;
        if ( !File.Exists( preloadSettingFile ) )
        {
            if ( !Directory.Exists( Path.GetDirectoryName( preloadSettingFile ) ) )
            {
                Directory.CreateDirectory( Path.GetDirectoryName( preloadSettingFile ) );
            }

            _asset = ScriptableObject.CreateInstance<AudioSettingAsset>( );
            _asset.createdTime = DateTime.Now.ToString( "F" );
            _asset.updatedTime = _asset.createdTime;
            AssetDatabase.CreateAsset( _asset, preloadSettingFile );
        }
        else
        {
            _asset = AssetDatabase.LoadAssetAtPath<AudioSettingAsset>( preloadSettingFile );
        }
        return _asset;
    }

    public static void SaveAsset( AudioSettingAsset asset )
    {
        asset.updatedTime = DateTime.Now.ToString( "F" );
        EditorUtility.SetDirty( asset );
        AssetDatabase.SaveAssetIfDirty( asset );
    }


    /// <summary>
    /// 遍历获取所有文件
    /// </summary>
    /// <param name="filePathByForeach"></param>
    /// <param name="result"></param>
    public static void ForeachFile( string filePathByForeach, List<FileInfo> result, string searchPattern = "*" )
    {
        try
        {
            DirectoryInfo theFolder = new DirectoryInfo( filePathByForeach );
            DirectoryInfo[] dirInfo = theFolder.GetDirectories( searchPattern );//获取所在目录的文件夹
            FileInfo[] file = theFolder.GetFiles( );//获取所在目录的文件

            if ( file.Length > 0 )
            {
                foreach ( FileInfo f in file )
                {
                    if ( !f.Name.EndsWith( ".meta" ) )
                    {
                        result.Add( f );
                    }
                }
            }

            //遍历文件夹
            foreach ( DirectoryInfo NextFolder in dirInfo )
            {
                ForeachFile( NextFolder.FullName, result, searchPattern );
            }
        }
        catch ( Exception )
        {
            throw;
        }

    }

}
