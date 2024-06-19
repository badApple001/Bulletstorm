using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class GizmosUtil
{

    public static void DrawCircle( Matrix4x4 matrix, float r, Vector2 offset, Color color, float thetaUnit = 0.001f )
    {

        // 设置矩阵
        Matrix4x4 defaultMatrix = Gizmos.matrix;
        Gizmos.matrix = matrix;

        // 设置颜色
        Color defaultColor = Gizmos.color;
        Gizmos.color = color;

        // 绘制圆环
        Vector3 beginPoint = new Vector3( r * Mathf.Cos( 0 ), r * Mathf.Sin( 0 ), 0 );
        Vector3 firstPoint = beginPoint;
        for ( float theta = thetaUnit; theta < 2 * Mathf.PI; theta += thetaUnit )
        {
            float x = offset.x + r * Mathf.Cos( theta );
            float y = offset.y + r * Mathf.Sin( theta );
            Vector3 endPoint = new Vector3( x, y, 0 );
            Gizmos.DrawLine( beginPoint, endPoint );
            beginPoint = endPoint;
        }

        // 绘制最后一条线段
        Gizmos.DrawLine( firstPoint, beginPoint );

        // 恢复默认颜色
        Gizmos.color = defaultColor;
        Gizmos.matrix = defaultMatrix;
    }
}
