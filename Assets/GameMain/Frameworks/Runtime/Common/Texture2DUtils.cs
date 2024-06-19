//==========================
// - FileName:      Assets/Frameworks/Scripts/Utility/Texture2DUtils.cs      
// - Created:       ChenJC	
// - CreateTime:    2023-06-28-20:20:14
// - UnityVersion:  2021.3.22f1
// - Version:       1.0
// - Description:   
//==========================

using UnityEngine;

public static class Texture2DUtils
{




    public static Sprite ToSprite( this Texture2D tex )
    {
        //Vector2.one * 0.5f;
        return Sprite.Create( tex, new Rect( 0, 0, tex.width, tex.height ), Vector2.zero );
    }


}
