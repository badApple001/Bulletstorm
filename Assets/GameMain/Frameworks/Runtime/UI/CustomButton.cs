using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;
using System.Collections.Generic;
using System;

[RequireComponent( typeof( Image ) )]
public class CustomButton : Button, IPointerDownHandler, IPointerUpHandler
{

    public bool scalable = true;
    public Vector3 scaleMultiplyBy = new Vector3( 0.95f, 0.95f, 0.95f );
    private Vector3 normalScale = Vector3.one, downScale;

    public string sound = "click";
    public string dbtEvent = string.Empty;
    public bool penetrateEvent = false;
    public static Action<string> clickEventHandler;


    protected override void OnEnable( )
    {
        base.OnEnable( );
        normalScale = transform.localScale;
        downScale = new Vector3( scaleMultiplyBy.x * normalScale.x, scaleMultiplyBy.y * normalScale.y, scaleMultiplyBy.z * normalScale.z );
    }


    public override void OnPointerDown( PointerEventData eventData )
    {
        base.OnPointerDown( eventData );

        if ( scalable && interactable )
            transform.localScale = downScale;

        if ( penetrateEvent )
        {
            PassEvent( eventData, ExecuteEvents.pointerDownHandler );
        }
    }

    public override void OnPointerEnter( PointerEventData eventData )
    {
        base.OnPointerEnter( eventData );

        if ( penetrateEvent )
        {
            PassEvent( eventData, ExecuteEvents.pointerEnterHandler );
        }
    }

    public override void OnPointerExit( PointerEventData eventData )
    {
        base.OnPointerExit( eventData );

        if ( penetrateEvent )
        {
            PassEvent( eventData, ExecuteEvents.pointerExitHandler );
        }
    }

    public override void OnPointerUp( PointerEventData eventData )
    {
        base.OnPointerUp( eventData );

        if ( scalable && interactable )
            transform.localScale = normalScale;

        if ( penetrateEvent )
        {
            PassEvent( eventData, ExecuteEvents.pointerUpHandler );
        }
    }

    public override void OnPointerClick( PointerEventData eventData )
    {
        base.OnPointerClick( eventData );

        if ( !string.IsNullOrEmpty( sound ) )
        {
            AudioManager.GetInstance( ).PlaySound( sound );
        }

        if ( !string.IsNullOrEmpty( dbtEvent ) )
        {
            clickEventHandler?.Invoke( dbtEvent );
        }

        if ( penetrateEvent )
        {
            PassEvent( eventData, ExecuteEvents.pointerClickHandler );
        }
    }


    public void PassEvent<T>( PointerEventData data, ExecuteEvents.EventFunction<T> function )
   where T : IEventSystemHandler
    {
        List<RaycastResult> results = new List<RaycastResult>( );
        EventSystem.current.RaycastAll( data, results );
        GameObject current = data.pointerCurrentRaycast.gameObject;
        for ( int i = 0; i < results.Count; i++ )
        {
            if ( current != results[ i ].gameObject )
            {
                ExecuteEvents.Execute( results[ i ].gameObject, data, function );
            }
        }
    }

}