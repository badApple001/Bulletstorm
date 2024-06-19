using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenEffect : MonoBehaviour
{
    [SerializeField] private Image hurtImg;
    [SerializeField] private float interval = 0.1f;
    [SerializeField] private float flashAlpha = 0.1f;
    [SerializeField] private HarmText harmTextPref;
    Queue<HarmText> harmTextObjPool = new Queue<HarmText>();
    bool flashing = false;

    // Start is called before the first frame update
    void Start( )
    {
        MsgFire.On( GameEventName.ON_PLAYER_HURT, OnPlayerHurt );
        MsgFire.On( GameEventName.ON_FLY_HARMTEXT, OnFlyHarmText );
    }


    private void OnFlyHarmText( object pos, object type, object harm )
    {
        HarmText harmText;
        if ( harmTextObjPool.Count > 0 )
        {
            harmText = harmTextObjPool.Dequeue( );
            harmText.gameObject.SetActive( true );
        }
        else
        {
             harmText = GameObject.Instantiate( harmTextPref );
        }
        harmText.transform.position = (Vector3)pos;
        harmText.objpool = harmTextObjPool;
        harmText.Init( ( HarmText.HarmType ) type, ( int ) harm );
    }

    private void OnPlayerHurt( )
    {
        if ( !flashing )
        {
            flashing = true;
            StartCoroutine( ScreenFlash( ) );
        }
    }

    IEnumerator ScreenFlash( )
    {
        hurtImg.color = new Color( 1f, 1f, 1f, flashAlpha );
        yield return new WaitForSeconds( interval );
        hurtImg.color = new Color( 1f, 1f, 1f, 0f );
        flashing = false;
    }

    private void OnDestroy( )
    {
        MsgFire.Off( this );
    }
}
