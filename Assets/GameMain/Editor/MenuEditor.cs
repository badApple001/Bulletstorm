using System.IO;
using UnityEditor;
using UnityEngine;
using static GameMain;

public class MenuEditor : Editor
{

    [MenuItem( "Tools/Clean Local Gamedata" )]
    public static void CleanLocalGameData( )
    {
        //File.Delete( Path.Combine( Application.persistentDataPath, nameof( GameData ) + ".json" ) );
        Directory.Delete( Application.persistentDataPath, true );
        Log.PINK( "Clean Local Gamedata complete" );
    }

}
