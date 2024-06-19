//==========================
// - FileName:      Assets/Frameworks/Editor/ConsoleUtils.cs
// - Created:       ChenJC	
// - CreateTime:    2023-06-19 10:03:20
// - UnityVersion:  2019.4.35f1
// - Version:       1.0
// - Description:   
//==========================
﻿using System.Reflection;
using UnityEditor;

public class ConsoleUtils
{
    public static void Clear()
    {
        Assembly assembly = Assembly.GetAssembly( typeof( SceneView ) );
        System.Type type = assembly.GetType( "UnityEditor.LogEntries" );
        MethodInfo method = type.GetMethod( "Clear" );
        method.Invoke( new object(), null );
    }

}
