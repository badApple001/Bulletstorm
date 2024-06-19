using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHub : MonoBehaviour
{
    [SerializeField] private Image imgHp;

    // Start is called before the first frame update
    void Start()
    {
        MsgFire.On( GameEventName.ON_PLAYER_HP_CHANGE, OnPlayerHpChange );
        MsgFire.On( GameEventName.ON_PLAYER_MP_CHANGE, OnPlayerMpChange );
    }

    private void OnDestroy( )
    {
        MsgFire.Off( this );
    }

    private void OnPlayerHpChange( object cur, object max )
    {
        float p = ( float ) cur / ( float ) max;
        imgHp.fillAmount = p;
    }

    //TODO: 暂时没找到相同画风的蓝条, 待定
    private void OnPlayerMpChange( object cur, object max )
    {
       
    }
}
