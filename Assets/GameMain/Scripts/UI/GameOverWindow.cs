using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverWindow : MonoBehaviour
{

    [SerializeField] private Image player;
    [SerializeField] private Sprite[] frames;

    // Start is called before the first frame update
    void Start( )
    {
        int index = 0;
        int count = frames.Length;
        Timer.SetInterval( 1f / 12, ( ) =>
        {
            index = ( index + 1 ) % count;
            player.sprite = frames[index];
        } );
    }

    public void OnClickReplay( )
    {
        Debug.Log( "Replay" );
        UIManager.Close<GameOverWindow>( );
        ObjectPool.Instance.Clear();
        Timer.ClearTimers( );
        SceneManager.LoadScene( 0 );
    }

    private void OnDestroy( )
    {
        //Timer.ClearTimer( this );
        Timer.ClearTimer<GameOverWindow>( );
    }
}
