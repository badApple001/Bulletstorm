using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HarmText : MonoBehaviour
{
    [SerializeField] private Text text;
    [SerializeField] private Color physics, magic, crit, normal;
    [SerializeField] private Animator animator;
    public Queue<HarmText> objpool = null;
    public enum HarmType
    {
        physics, magic, crit, normal
    }

    public void Init( HarmType harmType, int harm )
    {
        if ( harmType == HarmType.physics )
        {
            text.color = physics;
        }
        else if ( harmType == HarmType.magic ) { text.color = magic; }
        else if ( harmType == HarmType.normal ) { text.color = normal; }
        else if ( harmType == HarmType.crit ) { text.color = crit; }

        animator.transform.localScale = Vector3.one * 0.5f * ( harmType == HarmType.crit ? 1.2f : 1 );
        animator.transform.GetChild( 0 ).localEulerAngles = Vector3.zero;
        animator.CrossFade( harmType == HarmType.crit ? "crit" : "flyHarmText", 0 );
        text.text = harm.ToString( );
        Timer.SetTimeout( 1.5f, Clear );
    }

    public void Clear( )
    {
        objpool?.Enqueue( this );
        gameObject.SetActive( false );
    }

}
