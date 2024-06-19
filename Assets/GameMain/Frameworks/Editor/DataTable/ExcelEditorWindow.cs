//==========================
// - FileName:      Assets/Frameworks/Editor/Excel/ExcelEditorWindow.cs
// - Created:       ChenJC	
// - CreateTime:    2023-06-19 10:03:20
// - UnityVersion:  2019.4.35f1
// - Version:       1.0
// - Description:   
//==========================
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ExcelEditorWindow : EditorWindow
{
    [MenuItem( "Tools/Excel工具界面", false, 10 )]
    private static void Open( )
    {
        Rect wr = new Rect( 0, 0, 500, 500 );
        ExcelEditorWindow window = ( ExcelEditorWindow ) EditorWindow.GetWindowWithRect( typeof( ExcelEditorWindow ), wr, true, "Export Excel Window" );
        window.Show( );
    }

    class ExcelEditorConfig
    {
        public string excelRootFolder;
        public string dataOutFolder;
        public string lastUpdateTime = string.Empty;
    }
    ExcelEditorConfig data;
    private string GetConfigFilePath( )
    {
        return Path.Combine( Application.persistentDataPath, nameof( ExcelEditorConfig ) );
    }
    private void OnEnable( )
    {

        if ( !File.Exists( GetConfigFilePath( ) ) )
        {
            data = new ExcelEditorConfig( );
            try
            {
                var clips = Application.dataPath.Split( '/' );
                var projname = clips[ clips.Length - 2 ];
                var plant = projname.Split( '-' )[ 0 ] + "-plan";
                var plantPath = Application.dataPath.Replace( $"{projname}/Assets", $"{plant}/" );
                data.excelRootFolder = plantPath;
            }
            catch { }
        }
        else
        {
            try
            {
                data = JsonConvert.DeserializeObject<ExcelEditorConfig>( File.ReadAllText( GetConfigFilePath( ) ) );
            }
            catch ( Exception e )
            {
                Debug.LogError( e );
                data = new ExcelEditorConfig( );
            }
        }

        if ( string.IsNullOrEmpty( data.dataOutFolder ) )
        {
            data.dataOutFolder = Path.Combine( Application.dataPath, "AB/ExcelData/" ).Replace( '\\', '/' );
        }

        data.lastUpdateTime = DateTime.Now.ToString( );
        excelRootFolder = data.excelRootFolder;
        dataOutFolder = data.dataOutFolder;
    }
    private void OnDisable( )
    {
        try
        {
            string jsonstr = JsonConvert.SerializeObject( data );
            File.WriteAllText( GetConfigFilePath( ), jsonstr );
        }
        catch ( Exception e )
        {
            Debug.LogError( e );
        }
    }

    string excelRootFolder = string.Empty;
    string dataOutFolder = string.Empty;
    List<string> excels = new List<string>( );
    Vector2 excelScrollerPos = Vector2.zero;
    //绘制窗口时调用
    private void OnGUI( )
    {
        GUILayout.Label( "选择一个批量导出的Excel目录: " );
        GUILayout.BeginHorizontal( );
        excelRootFolder = GUILayout.TextField( excelRootFolder, 128, GUILayout.MaxWidth( 400f ) );
        if ( GUILayout.Button( "选择目录" ) )
        {
            string newFolder = EditorUtility.OpenFolderPanel( "选择Excel目录", excelRootFolder, string.Empty );
            if ( !string.IsNullOrEmpty( newFolder ) && !string.IsNullOrWhiteSpace( newFolder ) )
            {
                excelRootFolder = newFolder;
                if ( Directory.Exists( excelRootFolder ) )
                {
                    data.excelRootFolder = excelRootFolder;
                }
            }
        }
        GUILayout.EndHorizontal( );

        GUILayout.Label( "选择数据输出目录: " );
        GUILayout.BeginHorizontal( );
        dataOutFolder = GUILayout.TextField( dataOutFolder, 128, GUILayout.MaxWidth( 400f ) );
        if ( GUILayout.Button( "选择目录" ) )
        {
            string newFolder = EditorUtility.OpenFolderPanel( "选择输出目录", dataOutFolder, string.Empty );
            if ( !string.IsNullOrEmpty( newFolder ) && !string.IsNullOrWhiteSpace( newFolder ) )
            {
                dataOutFolder = newFolder;
                if ( Directory.Exists( dataOutFolder ) )
                {
                    data.dataOutFolder = dataOutFolder;
                }
            }
        }
        GUILayout.EndHorizontal( );
        GUILayout.Space( 4 );
        GUILayout.BeginHorizontal( );
        if ( GUILayout.Button( "开始遍历" ) )
        {
            excels.Clear( );
            if ( Directory.Exists( excelRootFolder ) )
            {
                ConsoleUtils.Clear( );
                string[] xlsxs = Directory.GetFiles( excelRootFolder, "*.xlsx", SearchOption.TopDirectoryOnly );

                foreach ( var xlsx in xlsxs )
                {
                    if ( xlsx.Contains( "~$" ) )
                    {
                        continue;
                    }
                    excels.Add( xlsx );
                }

            }
            else
            {
                EditorUtility.DisplayDialog( "路径错误", $"不存在 {excelRootFolder}", "确认" );
                Debug.LogError( $"Excel路径错误: {excelRootFolder}" );
            }
        }
        else if ( GUILayout.Button( "批量导出" ) )
        {
            BatchExport( excelRootFolder );
        }
        else if ( GUILayout.Button( "打开目录 - Excel源" ) )
        {
            EditorUtility.RevealInFinder( excelRootFolder );
        }
        else if ( GUILayout.Button( "打开目录 - 数据源" ) )
        {
            var dataFolder = Path.Combine( Application.dataPath, "AB/ExcelData" );
            EditorUtility.RevealInFinder( dataFolder );
        }
        else if ( GUILayout.Button( "打开目录 - 数据类" ) )
        {
            var csFolder = Path.Combine( Application.dataPath, ExcelExport.xlsxClassExportPath );
            EditorUtility.RevealInFinder( csFolder );
        }
        GUILayout.EndHorizontal( );
        if ( excels.Count > 0 )
        {
            GUILayout.Space( 4 );
            excelScrollerPos = EditorGUILayout.BeginScrollView( excelScrollerPos );
            EditorGUILayout.BeginVertical( );
            for ( int i = 0; i < excels.Count; i++ )
            {
                EditorGUILayout.BeginHorizontal( );
                EditorGUILayout.TextArea( excels[ i ], GUILayout.MaxWidth( 400 ) );
                if ( GUILayout.Button( "Export" ) )
                {
                    Export( excels[ i ] );
                }
                EditorGUILayout.EndHorizontal( );
            }
            EditorGUILayout.EndVertical( );
            EditorGUILayout.EndScrollView( );
        }

    }

    private void Export( string xlsx )
    {
        ExcelExport.ConvertFromFile( xlsx, data.dataOutFolder );
    }

    private void BatchExport( string folder )
    {
        ExcelExport.ConvertFromFolder( folder, data.dataOutFolder );
    }

    private void OnInspectorUpdate( )
    {
        this.Repaint( );
    }
}
