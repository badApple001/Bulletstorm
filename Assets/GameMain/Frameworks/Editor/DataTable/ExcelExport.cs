//==========================
// - FileName:      Assets/Frameworks/Editor/Excel/ExcelExport.cs
// - Created:       ChenJC	
// - CreateTime:    2023-06-19 10:03:20
// - UnityVersion:  2019.4.35f1
// - Version:       1.0
// - Description:   
//==========================
using UnityEditor;
using UnityEngine;
using System.IO;
using EasyTool;

public class ExcelExport
{

    public const string xlsxClassExportPath = "Scripts/XlsxExport";

    public static void ConvertFromFolder( string folder, string datapath )
    {
        float stime = Time.time;
        var xlsxs = Directory.GetFiles( folder, "*.xlsx" );
        foreach ( var xlsx in xlsxs )
        {
            if( xlsx.Contains( "~$" ) )
            {
                continue;
            }
            ConvertFromFile( xlsx, datapath, false );
        }
        AssetDatabase.Refresh( );
        Debug.Log( $"转换Excel完成 {Time.time - stime}秒" );
    }

    public static void ConvertFromFile( string xlsx, string dataPath, bool outUseTime = true )
    {
        float stime = Time.time;
        var dataFolder = dataPath;
        var csFolder = Path.Combine( Application.dataPath, xlsxClassExportPath );
        PathUtils.CreateFolder( dataFolder, csFolder );
        string err = ExcelTool.Convert( xlsx, dataFolder, csFolder );
        if ( !string.IsNullOrEmpty( err ) )
        {
            Debug.LogError( err );
        }
        if ( outUseTime )
        {
            AssetDatabase.Refresh( );
            Debug.Log( $"转换Excel完成 {Time.time - stime}秒" );
        }
    }

}
