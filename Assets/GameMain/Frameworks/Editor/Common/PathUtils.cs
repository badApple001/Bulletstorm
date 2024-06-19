//==========================
// - FileName:      Assets/Frameworks/Scripts/Utility/PathUtils.cs
// - Created:       ChenJC	
// - CreateTime:    2023-06-19 10:10:45
// - UnityVersion:  2019.4.35f1
// - Version:       1.0
// - Description:   
//==========================
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PathUtils
{
    public static void DeleteOldAndCreateNewFolder( string path )
    {
        if ( Directory.Exists( path ) )
        {
            Directory.Delete( path, true );
        }
        Directory.CreateDirectory( path );
    }

    public static void CreateFolder( params string[] paths )
    {
        foreach ( string path in paths )
        {
            if ( !Directory.Exists( path ) )
            {
                Directory.CreateDirectory( path );
            }
        }
    }
}
