//==========================
// - FileName:      Assets/Frameworks/Editor/ScriptingDefineSymbolsUtils.cs
// - Created:       ChenJC	
// - CreateTime:    2023-06-19 10:03:20
// - UnityVersion:  2019.4.35f1
// - Version:       1.0
// - Description:   
//==========================
using System.Collections.Generic;
using UnityEditor;

public class ScriptingDefineSymbolsUtils
{
    /// <summary>
    /// 是否包含宏
    /// </summary>
    /// <param name="define"></param>
    /// <returns></returns>
    public static bool HasDefines( string define )
    {
        var targetGruop = EditorUserBuildSettings.selectedBuildTargetGroup;
        string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup( targetGruop );
        var groups = defines.Split( ';' );
        return new List<string>( groups ).Contains( define );
    }

    /// <summary>
    /// 移除宏
    /// </summary>
    /// <param name="define"></param>
    public static void RemoveDefines( string define )
    {
        if ( HasDefines( define ) )
        {
            var targetGruop = EditorUserBuildSettings.selectedBuildTargetGroup;
            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup( targetGruop );
            var groups = defines.Split( ';' );
            var groupList = new List<string>( groups );
            groupList.Remove( define );
            string defineString = string.Join( ";", groupList );
            PlayerSettings.SetScriptingDefineSymbolsForGroup( targetGruop, defineString );
        }
    }

    /// <summary>
    /// 添加宏
    /// </summary>
    /// <param name="define"></param>
    public static void AddDefines( string define )
    {
        if ( !HasDefines( define ) )
        {
            var targetGruop = EditorUserBuildSettings.selectedBuildTargetGroup;
            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup( targetGruop );
            string defineString = defines.EndsWith( ";" ) ? $"{defines}{define}" : $"{defines};{define}";
            PlayerSettings.SetScriptingDefineSymbolsForGroup( targetGruop, defineString );
            Log.PINK( $"Add Define: {define}" );
        }
    }

    public static void  CleanDefines( )
    {
        var targetGruop = EditorUserBuildSettings.selectedBuildTargetGroup;
        PlayerSettings.SetScriptingDefineSymbolsForGroup( targetGruop, "");
    }
}
