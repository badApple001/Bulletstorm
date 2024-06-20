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
        _target.m_territorialRange = EditorGUILayout.FloatField( "领地范围", _target.m_territorialRange );
        _target.m_filedOfView = EditorGUILayout.FloatField( "视野范围", _target.m_filedOfView );
        _target.m_moveSpeed = EditorGUILayout.FloatField( "移动速度", _target.m_moveSpeed );
        _target.m_atkDistance = EditorGUILayout.FloatField( "攻击距离", _target.m_atkDistance );
        _target.m_attackAnimationTime = EditorGUILayout.FloatField( "攻击时机", _target.m_attackAnimationTime );
        _target.m_monsterType = ( MonsterType ) EditorGUILayout.EnumPopup( "怪物类型", _target.m_monsterType );
        if ( _target.m_monsterType == MonsterType.Shooter )
        {
            _target.m_bulletPrefab = ( GameObject ) EditorGUILayout.ObjectField( "子弹预设", _target.m_bulletPrefab, typeof( GameObject ), true );
            _target.m_muzzle = ( Transform ) EditorGUILayout.ObjectField( "发射枪口", _target.m_muzzle, typeof( Transform ), true );
            _target.m_isTrace = EditorGUILayout.Toggle( "追踪子弹", _target.m_isTrace );
        }
        else
        {
            _target.m_attackHarm = EditorGUILayout.FloatField( "近战伤害", _target.m_attackHarm );
        }
        _target.m_deathColor = EditorGUILayout.ColorField( "死亡颜色", _target.m_deathColor );

        //readonly
        GUI.enabled = false;
        _target.m_currentState = ( MonsterState ) EditorGUILayout.EnumPopup( "玩家状态", _target.m_currentState );
        GUI.enabled = true;

        //_speed数据修改结束监听，并在发生修改时，做“脏”标记
        if ( EditorGUI.EndChangeCheck( ) )
        {
            EditorUtility.SetDirty( target );
        }
    }

}
