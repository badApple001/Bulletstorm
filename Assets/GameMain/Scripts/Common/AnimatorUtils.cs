using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorUtils
{


    /// <summary>
    /// ��Ӷ����¼�
    /// </summary>
    /// <param name="_animator"></param>
    /// <param name="_clipName">��������</param>
    /// <param name="_eventFunctionName">�¼���������</param>
    /// <param name="_time">����¼�ʱ�䡣��λ����</param>
    public static void AddAnimationEvent( Animator _animator, string _clipName, string _eventFunctionName, float _time, string _eventParams = null )
    {
        AnimationClip[] _clips = _animator.runtimeAnimatorController.animationClips;
        for ( int i = 0; i < _clips.Length; i++ )
        {
            if ( _clips[ i ].name == _clipName )
            {
                AnimationEvent _event = new AnimationEvent( );
                _event.functionName = _eventFunctionName;
                _event.time = _time;
                if( _eventParams != null )
                {
                    _event.stringParameter = _eventParams;
                }
                _clips[ i ].AddEvent( _event );
                break;
            }
        }
        _animator.Rebind( );
    }


    /// <summary>
    /// ��������¼�
    /// </summary>
    public static void CleanAllEvent( Animator animator )
    {

        var clips = animator.runtimeAnimatorController.animationClips;
        for ( int i = 0; i < clips.Length; i++ )
        {
            clips[ i ].events = default( AnimationEvent[] );
        }
        Debug.Log( "��������¼�" );
    }
   
}
