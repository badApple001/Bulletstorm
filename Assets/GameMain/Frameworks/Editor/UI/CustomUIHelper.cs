using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CustomUIHelper 
{

    [MenuItem( "GameObject/🐕🐕🐕‍\U0001f9ba🐩🐶/UI/优化节点以下图片和文本的Makeabe和Raycast" )]
    public static void OptimizeRenderNodesFromSelectionObj( )
    {
        var obj = Selection.activeGameObject;
        if ( obj == null )
        {
            return;
        }
        var mgs = obj.GetComponentsInChildren<MaskableGraphic>( );
        void disableRaycast( MaskableGraphic mg )
        {
            mg.raycastTarget = false;
            Log.Info( $"disable raycastTarget {mg.transform.name}" );
        }
        void disableMakeable( MaskableGraphic mg )
        {
            mg.maskable = false;
            Log.Info( $"disable maskable {mg.transform.name}" );
        }
        foreach ( var m in mgs )
        {
            if ( m.GetComponent<Button>( ) == null )
            {
                disableRaycast( m );
                disableMakeable( m );

            }
        }
    }
}
