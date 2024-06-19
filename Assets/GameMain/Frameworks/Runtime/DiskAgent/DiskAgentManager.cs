using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiskAgentManager : MonoBehaviour
{

    private void OnApplicationFocus( bool focus )
    {
        if ( !focus )
        {
            try
            {
                DiskAgent.ToDiskAll( );
            }
            catch ( System.Exception e )
            {
                Debug.LogException( e );
            }
        }
    }

    private void OnApplicationPause( bool pause )
    {
        if ( pause )
        {
            try
            {
                DiskAgent.ToDiskAll( );
            }
            catch ( System.Exception e )
            {
                Debug.LogException( e );
            }
        }
    }

    private void OnApplicationQuit( )
    {
        try
        {
            DiskAgent.ToDiskAll( );
        }
        catch ( System.Exception e )
        {
            Debug.LogException( e );
        }
    }

}
