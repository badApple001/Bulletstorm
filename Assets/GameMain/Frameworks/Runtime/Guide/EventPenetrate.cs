using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EventPenetrate : MonoBehaviour, ICanvasRaycastFilter
{

    //作为目标点击事件渗透区域
    private GameObject target;

    private Action clickEvent;

    public void SetPenetrateObjectAndRigsterClickCallback( GameObject tg, Action clickHandler = null )
    {
        target = tg;
        this.clickEvent = clickHandler;
    }

    public bool IsRaycastLocationValid( Vector2 sp, Camera eventCamera )
    {
        //没有目标则捕捉事件渗透
        if ( target == null )
        {
            clickEvent?.Invoke( );
            return true;
        }

        //在目标范围内做事件渗透
        bool inSideObjRect = RectTransformUtility.RectangleContainsScreenPoint( target.GetComponent<RectTransform>( ),
            sp, eventCamera );

        if ( inSideObjRect )
        {
            if ( Application.isMobilePlatform )
            {
                if ( Input.touchCount > 0 )
                {
                    for ( int i = 0; i < Input.touchCount; i++ )
                    {
                        if ( Input.touches[ i ].phase == TouchPhase.Ended )
                        {
                            clickEvent?.Invoke( );
                            break;
                        }
                    }
                }
            }
            else if ( Input.GetMouseButtonUp( 0 ) )
            {
                clickEvent?.Invoke( );
            }
        }

        return !inSideObjRect;
    }

}