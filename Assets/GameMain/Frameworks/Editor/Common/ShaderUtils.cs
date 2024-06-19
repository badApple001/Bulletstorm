
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ShaderUtils 
{

    [MenuItem( "Assets/🐕🐕🐕‍\U0001f9ba🐩🐶/依赖文件夹内的shaders" )]
    public static void RefreshAlwaysIncludeShaders( )
    {

        List<string> ignore = new List<string>( )
        {
            "GUI*",
        };

        List<string> shaders = new List<string>
        {
            //2D基础
            "Sprites/Default",
            "UI/Default",

        };

        string[] guids = Selection.assetGUIDs;
        string[] dirs = new string[ guids.Length ];
        for ( int i = 0; i < guids.Length; i++ )
        {
            dirs[ i ] = AssetDatabase.GUIDToAssetPath( guids[ i ] );
            EditorUtility.DisplayProgressBar( "Filter dirs", dirs[ i ], i * 1.0f / guids.Length );
        }

        var currentShaders = GetAlawasIncludedCurrentShaders( );
        for ( int i = 0; i < currentShaders.Count; i++ )
        {
            var shader = currentShaders[ i ];
            if ( !shaders.Contains( shader ) )
            {
                shaders.Add( shader );
            }
            EditorUtility.DisplayProgressBar( "Add an already dependent shader", shader, i * 1.0f / currentShaders.Count );
        }

        var mats = AssetDatabase.FindAssets( "t:Material", dirs ).Select( s => AssetDatabase.LoadAssetAtPath<Material>( AssetDatabase.GUIDToAssetPath( s ) ) );
        int count = mats.Count( );
        int index = 0;
        foreach ( var mat in mats )
        {
            if ( mat.shader != null && !string.IsNullOrEmpty( mat.shader.name ) && !shaders.Contains( mat.shader.name ) )
            {
                shaders.Add( mat.shader.name );
            }
            EditorUtility.DisplayProgressBar( "Add project mateials dependent shader", mat.shader != null ? mat.shader.name : mat.name, ++index * 1.0f / count );
        }

        var shaderFiles = AssetDatabase.FindAssets( "t:shader", dirs ).Select( s => AssetDatabase.LoadAssetAtPath<Shader>( AssetDatabase.GUIDToAssetPath( s ) ) );
        foreach ( var shaderFile in shaderFiles )
        {
            if ( !shaders.Contains( shaderFile.name ) )
            {
                shaders.Add( shaderFile.name );
            }
        }

        for ( int i = 0; i < shaders.Count; i++ )
        {
            string shader = shaders[ i ];
            bool insideIgnore = ignore.Find( s =>
            {
                if ( s.EndsWith( "*" ) )
                {
                    var _ = s.Remove( s.Length - 1 );
                    if ( shader.StartsWith( _ ) )
                    {
                        return true;
                    }

                }
                else
                {
                    if ( shader == s )
                    {
                        return true;
                    }
                }
                return false;
            } ) != null;
            if ( insideIgnore )
            {
                shaders.RemoveAt( i-- );
            }

            EditorUtility.DisplayProgressBar( "Filter out the ignore list", shader, i * 1.0f / shaders.Count );
        }


        AddShadersToAlawasIncludedShades( shaders );
        EditorUtility.ClearProgressBar( );
        Debug.Log( "shader dependency complete" );
        AssetDatabase.Refresh( );
    }

    public static List<string> GetAlawasIncludedCurrentShaders( )
    {
        var shaders = new List<string>( );
        SerializedObject graphicsSettings = new SerializedObject( AssetDatabase.LoadAllAssetsAtPath( "ProjectSettings/GraphicsSettings.asset" )[ 0 ] );
        SerializedProperty it = graphicsSettings.GetIterator( );
        SerializedProperty dataPoint;
        while ( it.NextVisible( true ) )
        {
            if ( it.name == "m_AlwaysIncludedShaders" )
            {
                if ( it.isArray )
                {
                    for ( int i = 0; i < it.arraySize; i++ )
                    {
                        dataPoint = it.GetArrayElementAtIndex( i );
                        if ( dataPoint.objectReferenceValue is Shader s )
                        {
                            shaders.Add( s.name );
                        }
                    }
                }
            }
        }
        return shaders;
    }

    public static void AddShadersToAlawasIncludedShades( List<string> newShaders )
    {

        SerializedObject graphicsSettings = new SerializedObject( AssetDatabase.LoadAllAssetsAtPath( "ProjectSettings/GraphicsSettings.asset" )[ 0 ] );
        SerializedProperty it = graphicsSettings.GetIterator( );
        SerializedProperty dataPoint;
        while ( it.NextVisible( true ) )
        {
            if ( it.name == "m_AlwaysIncludedShaders" )
            {
                it.ClearArray( );
                for ( int i = 0; i < newShaders.Count; i++ )
                {
                    it.InsertArrayElementAtIndex( i );
                    dataPoint = it.GetArrayElementAtIndex( i );
                    var shader = Shader.Find( newShaders[ i ] );
                    if ( !shader )
                    {
                        UnityEngine.Debug.LogError( "找不到shader " + newShaders[ i ] + "请检查名字是否正确" );
                        continue;
                    }
                    dataPoint.objectReferenceValue = shader;
                    graphicsSettings.ApplyModifiedProperties( );
                    EditorUtility.DisplayProgressBar( "add to alawyInclude list", newShaders[ i ], i * 1.0f / newShaders.Count );
                }
            }
        }
    }

}
