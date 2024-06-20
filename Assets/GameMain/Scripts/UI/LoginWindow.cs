using BulletHell;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameMain;

public class LoginWindow : MonoBehaviour
{

    private void OnEnable( )
    {

    }
   
    public void Close( )
    {
        Time.timeScale = 1f;
        DiskAgent.Load<GameData>( ).firstInGame = true;
        UIManager.Close<LoginWindow>( );


        GameObject.FindAnyObjectByType<PlayerMovement>( ).enabled = true;
    }
}
