using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent( typeof( Image ) )]
public class ImageFrameAnimation : MonoBehaviour
{
    public float rate = 15;
    public Sprite[] frames;
    public bool once = false;
    private Image img;
    private float interval = 0;
    private int index = 0;

    // Start is called before the first frame update
    void Start( )
    {
        img = GetComponent<Image>( );
        interval = 1.0f / rate;
        Debug.Assert( frames != null, "至少需要放入一帧图片来驱动播放" );
        Timer.SetInterval( interval, UpdateFrame );
    }

    void UpdateFrame( )
    {
        index = ( index + 1 ) % frames.Length;
        img.sprite = frames[index];
        if( once && index == frames.Length - 1 )
        {
            Timer.ClearTimer( UpdateFrame );
        }
    }

    private void OnDestroy( )
    {
        Timer.ClearTimer( UpdateFrame );
    }


}
