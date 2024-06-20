using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponList : MonoBehaviour
{
    public List<GameObject> weapons = new List<GameObject>( );
    public List<GameObject> owneWeapons = new List<GameObject>( );
    private int current_weapon_index = 0;

    //private void Start( )
    //{
    //    owneWeapons[ 0 ].SetActive( true );
    //}

    public void DisableAllWeapon( )
    {
        owneWeapons.ForEach( w => w.SetActive( false ) );
    }

    public void AppendWeapon( int id )
    {
        //if( !weapons.Contains )

        if ( !owneWeapons.Exists( g => g.GetComponent<Gun>( ).ID == id ) && ( id >= 0 && id < weapons.Count ) )
        {
            owneWeapons.Add( weapons[ id ] );
            if ( owneWeapons.Count == 1 )
            {
                owneWeapons[ 0].SetActive( true );
                current_weapon_index = 0;
            }
        }
    }


    private void Update( )
    {
        SwitchGun( );
    }

    void SwitchGun( )
    {
        if ( Input.GetKeyDown( KeyCode.Q ) )
        {
            owneWeapons[ current_weapon_index ].SetActive( false );
            if ( --current_weapon_index < 0 )
            {
                current_weapon_index = owneWeapons.Count - 1;
            }
            owneWeapons[ current_weapon_index ].SetActive( true );
        }
        if ( Input.GetKeyDown( KeyCode.E ) )
        {
            owneWeapons[ current_weapon_index ].SetActive( false );
            if ( ++current_weapon_index > owneWeapons.Count - 1 )
            {
                current_weapon_index = 0;
            }
            owneWeapons[ current_weapon_index ].SetActive( true );
        }
    }
}
