using UnityEngine;
using System.Collections;
using UnityEditor.Callbacks;
using UnityEditor;
using System.IO;
using System;
public class PostProcessBuild
{
    [PostProcessBuildAttribute( )]
    public static void OnPostprocessBuild( BuildTarget target, string pathToBuiltProject )
    {
        if ( target == BuildTarget.Android )
            PostProcessAndroidBuild( pathToBuiltProject );
    }

    public static void PostProcessAndroidBuild( string pathToBuiltProject )
    {
        UnityEditor.ScriptingImplementation backend = UnityEditor.PlayerSettings.GetScriptingBackend( UnityEditor.BuildTargetGroup.Android );

        if ( backend == UnityEditor.ScriptingImplementation.IL2CPP )
        {
            CopyARMSymbols( "armeabi-v7a" );
            CopyARMSymbols( "arm64-v8a" );
        }
    }


    private static void CopyARMSymbols( string target )
    {
        const string libpath = "/Temp/StagingArea/libs/";
        string sourcePath = Directory.GetCurrentDirectory( ) + libpath + $"{target}/";
        string desPath = Directory.GetCurrentDirectory( ) + $"/Build/android/unityLibrary/symbols/{target}";
        if ( !Directory.Exists( desPath ) )
        {
            Directory.CreateDirectory( desPath );
        }
        CopyDirectory( sourcePath, desPath );
    }

    private static void CopyDirectory( string sourcePath, string targetPath )
    {

        if ( System.IO.Directory.Exists( sourcePath ) )
        {
            string[] files = System.IO.Directory.GetFiles( sourcePath );

            // Copy the files and overwrite destination files if they already exist.
            foreach ( string s in files )
            {
                // Use static Path methods to extract only the file name from the path.
                var fileName = System.IO.Path.GetFileName( s );
                var destFile = System.IO.Path.Combine( targetPath, fileName );
                Debug.Log( $"fileName: {fileName}, destFile: {destFile}" );
                System.IO.File.Copy( s, destFile, true );
            }
        }
        else
        {
            Console.WriteLine( "Source path does not exist!" );
        }
    }

}