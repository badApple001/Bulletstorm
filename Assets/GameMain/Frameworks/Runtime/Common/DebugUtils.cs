//==========================
// - FileName:      #ScriptFilePath#
// - Created:       #Author#
// - CreateTime:    #CreateTime#
// - UnityVersion:  #UnityVersion#
// - Version:       #Version#
// - Description:   
//==========================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugUtils 
{
    public static void DrawRect( Rect rect )
    {
        Debug.DrawLine( new Vector3( rect.x, rect.y ), new Vector3( rect.x + rect.width, rect.y ), Color.green );
        Debug.DrawLine( new Vector3( rect.x, rect.y ), new Vector3( rect.x, rect.y + rect.height ), Color.red );
        Debug.DrawLine( new Vector3( rect.x + rect.width, rect.y + rect.height ), new Vector3( rect.x + rect.width, rect.y ), Color.green );
        Debug.DrawLine( new Vector3( rect.x + rect.width, rect.y + rect.height ), new Vector3( rect.x, rect.y + rect.height ), Color.red );
    }
}
