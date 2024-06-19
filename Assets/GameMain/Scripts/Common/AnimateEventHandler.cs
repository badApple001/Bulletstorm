using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateEventHandler : MonoBehaviour
{
    Dictionary<string,Action> eventDict = new Dictionary<string,Action>();
    public void Fire( string eventName )
    {
        if( eventDict.TryGetValue( eventName, out Action action ) )
        {
            action();
        }
    }

    public void Listen( string eventName, Action action )
    {

        eventDict[eventName] = action;
    }

    public void Unlisten( string eventName )
    {
        eventDict.Remove( eventName );
    }

}
