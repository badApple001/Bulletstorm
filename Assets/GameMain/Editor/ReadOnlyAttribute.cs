/*******************************************************************************
* �汾������v1.0.0
* �� �� �ƣ�ReadOnlyAttribute
* �������ڣ�2020-04-23 21:56:16
* �������ƣ�ĩ��
* ����������ֻ������
* �޸ļ�¼��
* 
******************************************************************************/

//using UnityEditor;
//using UnityEngine;


///// <summary>
///// ֻ�����Թ�����
///// </summary>
//public class ReadOnlyAttribute : PropertyAttribute { }

///// <summary>
///// ������
///// </summary>
//[CustomPropertyDrawer( typeof( ReadOnlyAttribute ) )]
//public class ReadOnlyDrawer : PropertyDrawer
//{
//    /// <summary>
//    /// ��������ԭ�и߶�
//    /// </summary>
//    /// <param name="property"></param>
//    /// <param name="label"></param>
//    /// <returns></returns>
//    public override float GetPropertyHeight( SerializedProperty property, GUIContent label )
//    {
//        return EditorGUI.GetPropertyHeight( property, label, true );
//    }

//    /// <summary>
//    /// ֻ��
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
