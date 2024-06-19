using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor( typeof( CustomButton ) )]
public class CustomButtonEditor : UnityEditor.UI.ButtonEditor
{
    CustomButton _button;

    protected override void OnEnable( )
    {
        base.OnEnable( );
        _button = ( CustomButton ) target;
    }

    public override void OnInspectorGUI( )
    {
        base.OnInspectorGUI( );
        EditorGUILayout.Space( );
        serializedObject.Update( );

        EditorGUILayout.PropertyField( this.serializedObject.FindProperty( "penetrateEvent" ) );
        var prop_ScaleAnim = this.serializedObject.FindProperty( "scalable" );
        EditorGUILayout.PropertyField( prop_ScaleAnim );
        if ( prop_ScaleAnim.boolValue )
        {
            EditorGUILayout.PropertyField( this.serializedObject.FindProperty( "scaleMultiplyBy" ) );
        }

        EditorGUILayout.PropertyField( this.serializedObject.FindProperty( "sound" ) );
        EditorGUILayout.PropertyField( this.serializedObject.FindProperty( "dbtEvent" ) );

        
        //var prop_Sound = this.serializedObject.FindProperty( "sound" );
        //EditorGUILayout.PropertyField( prop_Sound );
        //if ( prop_Sound.boolValue )
        //{
        //    EditorGUILayout.PropertyField( this.serializedObject.FindProperty( "clickEffect" ) );
        //}
        serializedObject.ApplyModifiedProperties( );
    }


    [MenuItem( "GameObject/🐕🐕🐕‍\U0001f9ba🐩🐶/UI/"+ nameof( CustomButton ), false, 0 )]
    public static void GenGButton( )
    {

        if ( Selection.activeGameObject == null )
        {
            return;
        }


        var gbtnobj = new GameObject( nameof( _button ) );
        gbtnobj.transform.SetParent( Selection.activeGameObject.transform, false );
        gbtnobj.AddComponent<CustomButton>( );
    }


    [MenuItem( "GameObject/🐕🐕🐕‍\U0001f9ba🐩🐶/UI/替换Button -> CustomButton<包括事件>", false, 0 )]
    public static void ReplaceUGUIButton2GButton( )
    {
        if ( null != Selection.activeGameObject )
        {
            var buttons = Selection.activeGameObject.GetComponentsInChildren<Button>( );
            foreach ( var button in buttons )
            {
                if ( button.GetType( ) != typeof( CustomButton ) )
                {
                    var @event = button.onClick;
                    if ( @event != null )
                    {
                        var obj = button.gameObject;
                        GameObject.DestroyImmediate( button );
                        var gbutton = obj.AddComponent<CustomButton>( );
                        gbutton.onClick = @event;
                        Log.PINK( $"Replace Button {obj.name} Succeed" );
                    }
                }
            }
        }

    }

}