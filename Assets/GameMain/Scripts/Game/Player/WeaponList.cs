using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponList : MonoBehaviour
{

    public List<GameObject> weapons;
    private int current_weapon_index = 0;

    private void Start( )
    {
        weapons[ 0 ].SetActive( true );
    }

    public void DisableAllWeapon( )
    {
        weapons.ForEach( w => w.SetActive( false ) );
    }

    private void Update( )
    {
        SwitchGun();
    }

    void SwitchGun( )
    {
        if ( Input.GetKeyDown( KeyCode.Q ) )
        {
            weapons[ current_weapon_index ].SetActive( false );
            if ( --current_weapon_index < 0 )
            {
                current_weapon_index = weapons.Count - 1;
            }
            weapons[ current_weapon_index ].SetActive( true );
        }
        if ( Input.GetKeyDown( KeyCode.E ) )
        {
            weapons[ current_weapon_index ].SetActive( false );
            if ( ++current_weapon_index > weapons.Count - 1 )
            {
                current_weapon_index = 0;
            }
            weapons[ current_weapon_index ].SetActive( true );
        }
    }
}
