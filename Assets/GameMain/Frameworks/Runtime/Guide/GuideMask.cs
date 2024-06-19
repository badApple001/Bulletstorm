using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GuideMask : MonoBehaviour
{
    private RawImage _rawImage; //遮罩图片
    private RectTransform _rectTrans;
    Material _materia;
    Canvas _canvas;
    private EventPenetrate ev;
    GameObject guide;

    bool ShowTween = false;
    float CurRadNum;


    public static bool GuideEnabled = false;

    private static GuideMask instance;
    public static GuideMask GetInstance( )
    {
        if ( instance == null )
        {
            var pref = Resources.Load<GameObject>( "GuideCanvas" );
            GameObject guideCanvas = Instantiate( pref );
            instance = guideCanvas.GetComponent<GuideMask>( );
        }
        return instance;
    }

    void Awake( )
    {
        Camera[] cams = GameObject.FindObjectsOfType<Camera>( );
        Camera uicam = cams[ 0 ];
        float depth = -1;
        foreach ( Camera cam in cams )
        {
            if ( cam.depth > depth )
            {
                depth = cam.depth;
                uicam = cam;
            }
        }

        var canvas = _canvas = GetComponent<Canvas>( );
        canvas.worldCamera = uicam;
    }


    /// <summary>
    /// 全屏Mask
    /// </summary>
    /// <param name="Clickcallback"></param>
    public void SetFullMask( UnityAction Clickcallback, string htmlColorValue = null )
    {
        ShowGuideMask( ( ) =>
        {
            if ( guide != null )
            {
                if ( null != htmlColorValue && ColorUtility.TryParseHtmlString( htmlColorValue, out Color c ) )
                {
                    _rawImage.color = c;
                }
                else
                {
                    _rawImage.color = new Color( 0, 0, 0, 1 );
                }
                guide.GetComponent<Button>( ).onClick.RemoveAllListeners( );
                guide.GetComponent<Button>( ).onClick.AddListener( delegate
                {
                    guide.GetComponent<Button>( ).onClick.RemoveAllListeners( );
                    Clickcallback?.Invoke( );
                } );
            }
        } );
    }


    private GameObject _focusObj = null;
    public void SetOverlayGuide( GameObject focusObj, Action callback )
    {
        if ( focusObj == null )
        {
            return;
        }

        //if ( !focusObj.TryGetComponent( out GraphicRaycaster raycaster ) )
        //{
        //    raycaster = focusObj.AddComponent<GraphicRaycaster>( );
        //}
        if ( !focusObj.TryGetComponent( out Canvas canvas ) )
        {
            canvas = focusObj.AddComponent<Canvas>( );
        }
       
        _focusObj = focusObj;
        canvas.pixelPerfect = true;
        canvas.overrideSorting = true;
        canvas.sortingOrder = 0x7fff;
        canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1 | AdditionalCanvasShaderChannels.TexCoord2;

        ShowGuideMask( ( ) =>
        {
            if ( guide != null )
            {
                this.callback = callback;
                ev.SetPenetrateObjectAndRigsterClickCallback( focusObj, OnClickCallback );
            }
        } );

    }

    /// <summary>
    /// 创建圆形点击区域
    /// </summary>
    /// <param name="target">目标位置</param>
    /// <param name="offeset">大小偏移量</param>
    /// <param name="CallBack">点击的回调</param>
    public void SetCircleMaskFromGameObject( GameObject target, float sizeOffset = 0f, Action callback = null )
    {
        if ( target != null )
        {
            RectTransform rec = target.GetComponent<RectTransform>( );
            CreateCircleMask( GetTargetCenter( rec ), GetTargetRad( rec ) + sizeOffset, target, callback );
        }
    }


    private void CreateCircleMask( Vector3 pos, float rad, GameObject target, Action callback )
    {
        ShowGuideMask( ( ) =>
        {
            ShowTween = true;
            this.callback = callback;
            ev.SetPenetrateObjectAndRigsterClickCallback( target, OnClickCallback );
            _rectTrans.sizeDelta = Vector2.zero;
            _materia.SetFloat( "_MaskType", 0f );//给材质设置显示类型，0为圆形，1为矩形
            CurRadNum = rad;
            _materia.SetVector( "_Origin", new Vector4( pos.x, pos.y, rad + 1000, 20 ) );//设置中心点
        } );
    }

    private Action callback = null;
    private void OnClickCallback( )
    {
        callback?.Invoke( );

        CloseGuideMask( );
    }

    /// <summary>
    /// 创建矩形点击区域
    /// </summary>
    /// <param name="obj">目标位置</param>
    /// <param name="CallBack">回调</param>
    public void CreateRectangleMask( GameObject target, Action callback, float scale = 1f )
    {
        if ( target != null )
        {
            RectTransform rec = target.GetComponent<RectTransform>( );
            Vector3[] _corners = new Vector3[ 4 ];
            rec.GetWorldCorners( _corners );
            Vector2 pos1 = WorldToCanvasPos( _corners[ 0 ] );//选取左下角
            Vector2 pos2 = WorldToCanvasPos( _corners[ 2 ] );//选取右上角

            //Vector2 center = ( pos1 + pos2 ) / 2;//矩形中心点
            //Vector2 leftTop = ( pos1 - center ).magnitude * scale * ( pos1 - center ).normalized  + center;
            //Vector2 rightBottom = ( pos2 - center ).magnitude * scale * ( pos2 - center ).normalized + center;

            CreateRectangleMaskRect( pos1, pos2, target, callback );
        }
        else
        {
            CreateRectangleMaskRect( new Vector2( 0, 0 ), new Vector2( 0, 0 ), target, callback );
        }
    }

    /// <summary>
    /// 创建矩形点击区域
    /// </summary>
    /// <param name="pos">矩形的屏幕位置</param>
    /// <param name="pos1">左下角位置</param>
    /// <param name="pos2">右上角位置</param>
    /// <param name="CallBack">回调</param>
    private void CreateRectangleMaskRect( Vector3 pos1, Vector3 pos2, GameObject target, Action callback )
    {
        ShowGuideMask( ( ) =>
        {
            //ShowTween = true;
            this.callback = callback;
            ev.SetPenetrateObjectAndRigsterClickCallback( target, OnClickCallback );
            _rectTrans.sizeDelta = Vector2.zero;
            _materia.SetFloat( "_MaskType", 1.0f );
            _materia.SetVector( "_Origin", new Vector4( pos1.x, pos1.y, pos2.x, pos2.y ) );
        } );
    }


    private void ShowGuideMask( Action callback )
    {
        ShowTween = false;
        gameObject.SetActive( true );
        if ( _rectTrans == null )
        {
            guide = transform.GetChild( 0 ).gameObject;
            _rectTrans = guide.GetComponent<RectTransform>( );
            _rawImage = guide.GetComponent<RawImage>( );
            _rawImage.color = new Color( 1, 1, 1, 1 );
            _materia = _rawImage.material;
            ev = guide.GetComponent<EventPenetrate>( );
            callback( );
        }
        else
        {
            callback( );
        }
    }


    /// <summary>
    /// 关闭引导遮罩
    /// </summary>
    public void CloseGuideMask( )
    {
        if ( guide != null )
        {
            _rawImage = null;
            _rectTrans = null;
            if ( _materia != null )
            {
                _materia.SetFloat( "_MaskType", 1.0f );
                _materia.SetVector( "_Origin", new Vector4( 0, 0, 0, 0 ) );
            }
        }
        gameObject.SetActive( false );
        SetTargetNil( );
        callback = null;


        if ( null != _focusObj )
        {
            Destroy( _focusObj.GetComponent<Canvas>( ) );
            _focusObj.GetComponent<Button>( )?.onClick.Invoke( );
            _focusObj = null;
        }

    }

    /// <summary>
    /// 将可点击区域设置为空
    /// </summary>
    private void SetTargetNil( )
    {
        if ( ev != null )
        {
            ev.SetPenetrateObjectAndRigsterClickCallback( null );
        }
    }

    public void Update( )
    {
        if ( ShowTween )
        {
            if ( _materia != null )
            {
                Vector4 mateV4 = _materia.GetVector( "_Origin" );
                if ( mateV4.z <= CurRadNum + 25 )
                {
                    ShowTween = false;
                    return;
                }
                mateV4.z = Mathf.Lerp( mateV4.z, CurRadNum, 15f * Time.deltaTime );

                _materia.SetVector( "_Origin", mateV4 );

            }
        }
    }

    /// <summary>
    /// 获取对象RectTransform的半径
    /// </summary>
    /// <param name="rect"></param>
    /// <returns></returns>
    public float GetTargetRad( RectTransform rect )
    {
        Vector3[] _corners = new Vector3[ 4 ];
        rect.GetWorldCorners( _corners );
        //计算最终高亮显示区域的半径       
        float _radius = Vector2.Distance( WorldToCanvasPos( _corners[ 0 ] ),
                     WorldToCanvasPos( _corners[ 2 ] ) ) / 2f;
        return _radius;
    }

    /// <summary>
    /// 获取对象RectTransform的中心点
    /// </summary>
    /// <param name="rect"></param>
    /// <returns></returns>
    public Vector2 GetTargetCenter( RectTransform rect )
    {
        Vector3[] _corners = new Vector3[ 4 ];
        rect.GetWorldCorners( _corners );

        float x = _corners[ 0 ].x + ( ( _corners[ 3 ].x - _corners[ 0 ].x ) / 2f );
        float y = _corners[ 0 ].y + ( ( _corners[ 1 ].y - _corners[ 0 ].y ) / 2f );
        Vector3 centerWorld = new Vector3( x, y, 0 );
        Vector2 center = WorldToCanvasPos( centerWorld );
        return center;
    }
    /// <summary>
    /// 获取对象RectTransform的中心点
    /// </summary>
    /// <param name="rect"></param>
    /// <returns></returns>
    public Vector2 GetTargetCenter( RectTransform rect, float _x, float _y )
    {
        Vector3[] _corners = new Vector3[ 4 ];
        rect.GetWorldCorners( _corners );//获得对象的四个角坐标

        float x = _corners[ 0 ].x + ( ( _corners[ 3 ].x - _corners[ 0 ].x ) / 2f );
        float y = _corners[ 0 ].y + ( ( _corners[ 1 ].y - _corners[ 0 ].y ) / 2f );
        Vector3 centerWorld = new Vector3( x + _x, y + _y, 0 );
        Vector2 center = WorldToCanvasPos( centerWorld );
        return center;
    }


    /// <summary>
    /// 世界坐标向画布坐标转换
    /// </summary>
    /// <param name="world">世界坐标</param>
    /// <returns>返回画布上的二维坐标</returns>
    private Vector2 WorldToCanvasPos( Vector3 world )
    {
        if ( null == _canvas ) _canvas = gameObject.GetComponent<Canvas>( );
        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle( _canvas.transform as RectTransform,
            world, _canvas.GetComponent<Camera>( ), out position );
        return position;
    }


    private void OnEnable( )
    {
        GuideEnabled = true;
    }

    private void OnDisable( )
    {
        GuideEnabled = false;
    }

}
