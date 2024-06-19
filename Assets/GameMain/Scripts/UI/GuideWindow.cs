using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideWindow : MonoBehaviour
{
    
    public void Close( )
    {
        UIManager.Close<GuideWindow>( );
    }

}
