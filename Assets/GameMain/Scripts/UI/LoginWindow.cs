using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameMain;

public class LoginWindow : MonoBehaviour
{
    
    public void Close( )
    {
        DiskAgent.Load<GameData>( ).firstInGame = true;
        UIManager.Close<LoginWindow>( );
    }
}
