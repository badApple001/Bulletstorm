using UnityEditor;
using UnityEngine;
using static Monster;

[CanEditMultipleObjects]
[CustomEditor( typeof( Monster ), true)]
public class MonsterEditor : Editor
{

    Monster _target;
    private void OnEnable( )
    {
        _target = ( Monster ) target;
    }

    public override void OnInspectorGUI( )
    {
        _target.m_territorialRange = EditorGUILayout.FloatField( "��ط�Χ", _target.m_territorialRange );
        _target.m_filedOfView = EditorGUILayout.FloatField( "��Ұ��Χ", _target.m_filedOfView );
        _target.m_moveSpeed = EditorGUILayout.FloatField( "�ƶ��ٶ�", _target.m_moveSpeed );
        _target.m_atkDistance = EditorGUILayout.FloatField( "��������", _target.m_atkDistance );
        _target.m_attackAnimationTime = EditorGUILayout.FloatField( "����ʱ��", _target.m_attackAnimationTime );
        _target.m_monsterType = ( MonsterType ) EditorGUILayout.EnumPopup( "��������", _target.m_monsterType );
        if ( _target.m_monsterType == MonsterType.Shooter )
        {
            _target.m_bulletPrefab = ( GameObject ) EditorGUILayout.ObjectField( "�ӵ�Ԥ��", _target.m_bulletPrefab, typeof( GameObject ), true );
            _target.m_muzzle = ( Transform ) EditorGUILayout.ObjectField( "����ǹ��", _target.m_muzzle, typeof( Transform ), true );
            _target.m_isTrace = EditorGUILayout.Toggle( "׷���ӵ�", _target.m_isTrace );
        }
        else
        {
            _target.m_attackHarm = EditorGUILayout.FloatField( "��ս�˺�", _target.m_attackHarm );
        }
        _target.m_deathColor = EditorGUILayout.ColorField( "������ɫ", _target.m_deathColor );

        //readonly
        GUI.enabled = false;
        _target.m_currentState = ( MonsterState ) EditorGUILayout.EnumPopup( "���״̬", _target.m_currentState );
        GUI.enabled = true;

        //_speed�����޸Ľ������������ڷ����޸�ʱ�������ࡱ���
        if ( EditorGUI.EndChangeCheck( ) )
        {
            EditorUtility.SetDirty( target );
        }
    }

}
