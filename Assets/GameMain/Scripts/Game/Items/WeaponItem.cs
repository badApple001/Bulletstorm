using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponItem : InteractiveItem
{

    //武器会发光, 发光的间隔
    public float growInterval = 0.2f;
    public Material growMaterial, defaultMaterial;
    private float growtm = 0;
    private bool flow = false;
    SpriteRenderer spriteRenderer;
    private void Start( )
    {
        spriteRenderer = transform.GetChild( 0 ).GetComponent<SpriteRenderer>( );
        growMaterial = spriteRenderer.material;
    }

    private void Update( )
    {
        growtm += Time.deltaTime;
        if ( growtm >= growInterval )
        {
            growtm -= growInterval;
            flow = !flow;
            spriteRenderer.material = flow ? growMaterial : defaultMaterial;
        }
    }


    private void OnTriggerEnter2D( Collider2D collision )
    {

        switch ( itemType )
        {
            case ItemType.Consumables:
                break;
            case ItemType.Weapon:
                if ( collision.TryGetComponent<WeaponList>( out var weaponList ) )
                {
                    Destroy( gameObject );
                    weaponList.AppendWeapon( itemID );
                }
                break;
            case ItemType.GoldCoin:
                break;
            case ItemType.SpecialGoods:
                break;
            default:
                break;
        }
    }
}
