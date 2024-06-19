using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using static Timer;
using Object = UnityEngine.Object;

[CanEditMultipleObjects, CustomEditor( typeof( Timer ) )]
public class TimerInspector : Editor
{
    FieldInfo activeTaskClsInfo;
    private void OnEnable( )
    {
        activeTaskClsInfo = typeof( Timer ).GetField( "activeTaskCls", BindingFlags.Static | BindingFlags.NonPublic );
    }

    public override void OnInspectorGUI( )
    {
        base.OnInspectorGUI( );

        if ( !Application.isPlaying )
        {
            return;
        }
        object v = activeTaskClsInfo.GetValue( null );
        var timerTasks = v as List<TimerTask>;
        EditorGUILayout.LabelField( $"TotalTimerCount: {timerTasks.Count}" );
        for ( int i = 0; i < timerTasks.Count; i++ )
        {
            var task = timerTasks[ i ];
            if ( null != task.func )
            {
                string caller = task.func.Target == null ? task.func.Method.DeclaringType.FullName : task.func.Target.GetType( ).FullName.Split( '+' )[ 0 ];

                EditorGUILayout.BeginHorizontal( );
                if ( EditorGUILayout.LinkButton( "Jump" ) )
                {
                    Jump2ScriptLinesByClsName( caller, task.func );
                }
                EditorGUILayout.LabelField( $"[{i + 1}] {caller}->{task.func.Method.Name}" );
                EditorGUILayout.EndHorizontal( );
            }
            else
            {
                EditorGUILayout.BeginHorizontal( );
                if ( EditorGUILayout.LinkButton( "Jump" ) )
                {
                    EditorUtility.DisplayDialog( "Error", "The invoking mode of the timer is incorrect!", "Confirm" );
                }
                EditorGUILayout.LabelField( $"[{i + 1}] null" );
                EditorGUILayout.EndHorizontal( );
            }
        }
    }



    public void Jump2ScriptLinesByClsName( string className, Action func )
    {
        string[] clssAssetGuids = AssetDatabase.FindAssets( className );
        if ( clssAssetGuids.Length > 0 )
        {

            var scripts = Array.FindAll<string>( clssAssetGuids, guid =>
            {
                string path = AssetDatabase.GUIDToAssetPath( guid );

                if ( path.EndsWith( ".cs" ) )
                {
                    string classFlag = $" class {className}";
                    string[] classs = Array.FindAll<string>( File.ReadAllLines( path ), l =>
                    {
                        return l.Contains( classFlag );
                    } );

                    return Array.Find<string>( classs, s =>
                    {
                        int _ = s.IndexOf( classFlag ) + classFlag.Length;
                        if ( _ < s.Length && ( _ + 1 >= s.Length || !Char.IsLetter( s[ _ + 1 ] ) ) )
                        {
                            return true;
                        }

                        return false;
                    } ) != null;
                }

                return false;
            } );
            if ( scripts.Length > 1 )
            {
                //脚本名与class名越相似权重越靠前
                Array.Sort( scripts, ( a, b ) =>
                {
                    string ap = AssetDatabase.GUIDToAssetPath( a );
                    string bp = AssetDatabase.GUIDToAssetPath( b );
                    ap = Path.GetFileNameWithoutExtension( ap );
                    bp = Path.GetFileNameWithoutExtension( bp );

                    if ( ap == className )
                    {
                        return -1;
                    }
                    else if ( bp == className )
                    {
                        return 1;
                    }
                    else if ( ap.ToLower( ) == className.ToLower( ) )
                    {
                        return -1;
                    }
                    else if ( bp.ToLower( ) == className.ToLower( ) )
                    {
                        return 1;
                    }
                    else if ( ap.StartsWith( className ) )
                    {
                        return -1;
                    }
                    else if ( bp.StartsWith( className ) )
                    {
                        return 1;
                    }
                    else if ( ap.ToLower( ).StartsWith( className.ToLower( ) ) )
                    {
                        return -1;
                    }
                    else if ( bp.ToLower( ).StartsWith( className.ToLower( ) ) )
                    {
                        return 1;
                    }
                    return 0;
                } );
            }
            var script = scripts.Length > 0 ? scripts[ 0 ] : null;

            Object obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>( AssetDatabase.GUIDToAssetPath( string.IsNullOrEmpty( script ) ? clssAssetGuids[ 0 ] : script ) );
            if ( obj is TextAsset textAsset )
            {
                string[] lines = textAsset.text.Split( new string[] { "\r\n", "\n", "\r" }, System.StringSplitOptions.None );
                var listLine = new List<string>( lines );
                string methodName = func.Method.Name;
                List<string> paramNames = new List<string>( );

                //is lambda
                bool islambda = false;
                if ( methodName.Contains( "<" ) && methodName.Contains( ">" ) && methodName.Contains( "_" ) )
                {
                    islambda = true;
                    methodName = methodName.Substring( 1, methodName.IndexOf( '>' ) - 1 );
                    var fileds = func.Target.GetType( ).GetFields( );
                    if ( fileds.Length > 1 )
                    {
                        for ( int j = 1; j < fileds.Length; j++ )
                        {
                            paramNames.Add( fileds[ j ].FieldType.Name );
                        }
                    }
                }

                var totalParams = func.Method.GetParameters( );
                if ( totalParams.Length > 0 )
                {
                    foreach ( var param in totalParams )
                    {
                        paramNames.Add( param.Name );
                    }
                }

                string returnparam = func.Method.ReturnTypeCustomAttributes.ToString( );
                int lineNumber = listLine.FindIndex( line =>
                {
                    if ( string.IsNullOrEmpty( line ) || !line.Contains( "(" ) || line.Contains( "//" ) )
                    {
                        return false;
                    }

                    //overload method filter
                    int methodNameIndex = line.IndexOf( methodName );
                    if ( methodNameIndex < 0 )
                    {
                        return false;
                    }
                    char c = line[ methodNameIndex + methodName.Length ];
                    if ( Char.IsLetter( c ) )
                    {
                        return false;
                    }

                    //return params
                    if ( !islambda && !line.ToLower( ).Contains( returnparam.ToLower( ) ) )
                    {
                        return false;
                    }


                    if ( paramNames.Count > 0 )
                    {
                        int leftIndex = line.IndexOf( "(" );

                        for ( int k = 0; k < paramNames.Count; k++ )
                        {
                            if ( line.IndexOf( paramNames[ k ] ) < leftIndex )
                            {
                                return false;
                            }
                        }

                        //is overload method ?
                        string paramDomain = line.Substring( leftIndex, line.IndexOf( ')' ) + 1 - leftIndex );
                        int paramCount = paramDomain.Split( ',' ).Length;
                        if ( paramCount != paramNames.Count )
                        {
                            return false;
                        }
                    }

                    return true;
                } );
                lineNumber = Mathf.Max( lineNumber, 0 );
                AssetDatabase.OpenAsset( obj, lineNumber + 1 );
            }
        }

    }

}
