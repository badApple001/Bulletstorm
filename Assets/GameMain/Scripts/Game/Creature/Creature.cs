using System;
using UnityEngine;

public class Creature : MonoBehaviour
{
    [SerializeField] private float maxHp = 10000, maxMp = 10000;
    public float defense = 0.5f;
    public int lv = 1;
    private float hp = 0, mp = 0;
 
    public Action<float, float> OnHpChange;
    public Action<float, float> OnMpChange;

    private void Awake( ) => Init( );
    private void Init( )
    {
        hp = maxHp;
        mp = maxMp;
    }

    public float Hp
    {
        get { return hp; }
        set
        {
            hp = Mathf.Clamp( value, 0, maxHp );
            OnHpChange?.Invoke( hp, maxMp );
        }
    }

    public float Mp
    {
        get { return mp; }
        set
        {
            mp = Mathf.Clamp( value, 0, maxMp );
            OnMpChange?.Invoke( mp, maxMp );
        }
    }

    public int GetExp( )
    {

        //此公式应该要遵循策划的数值, 这里我临时用一个公式
        return 100 * lv;
    }
}
