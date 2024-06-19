using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [HideInInspector]
    public Creature creature { get; private set; } = null;
    public void Awake( )
    {
        creature = GetComponent<Creature>( );
        creature.OnHpChange += OnHpChange;
        creature.OnMpChange += OnMpChange;
    }

    void OnHpChange( float cur, float max )
    {
        //通知ui
        MsgFire.Event( GameEventName.ON_PLAYER_HP_CHANGE, cur, max );
        
        //红屏
        MsgFire.Event( GameEventName.ON_PLAYER_HURT );

        //禁用武器
        if ( cur == 0 ) {

            Debug.Log( "玩家死亡" );
            GetComponent<WeaponList>( ).DisableAllWeapon( );
            UIManager.Create<GameOverWindow>( );
        }
    }

    void OnMpChange( float cur, float max )
    {
        // 通知ui
        MsgFire.Event( GameEventName.ON_PLAYER_MP_CHANGE, cur, max );
    
    
    }
    
}
