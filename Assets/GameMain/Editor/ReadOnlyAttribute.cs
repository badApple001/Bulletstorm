/*******************************************************************************
* 版本声明：v1.0.0
* 类 名 称：ReadOnlyAttribute
* 创建日期：2020-04-23 21:56:16
* 作者名称：末零
* 功能描述：只读特性
* 修改记录：
* 
******************************************************************************/

//using UnityEditor;
//using UnityEngine;


///// <summary>
///// 只读特性功能类
///// </summary>
//public class ReadOnlyAttribute : PropertyAttribute { }

///// <summary>
///// 面板绘制
///// </summary>
//[CustomPropertyDrawer( typeof( ReadOnlyAttribute ) )]
//public class ReadOnlyDrawer : PropertyDrawer
//{
//    /// <summary>
//    /// 用来保持原有高度
//    /// </summary>
//    /// <param name="property"></param>
//    /// <param name="label"></param>
//    /// <returns></returns>
//    public override float GetPropertyHeight( SerializedProperty property, GUIContent label )
//    {
//        return EditorGUI.GetPropertyHeight( property, label, true );
//    }

//    /// <summary>
//    /// 只读
//    /// </summary>
//    /// <param name="position"></param>
//    /// <param name="property"></param>
//    /// <param name="label"></param>
//    public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
//    {
//        GUI.enabled = false;
//        EditorGUI.PropertyField( position, property, label, true );
//        GUI.enabled = true;
//    }
//}
