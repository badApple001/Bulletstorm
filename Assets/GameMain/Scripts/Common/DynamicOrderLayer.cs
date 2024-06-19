using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicOrderLayer : MonoBehaviour
{
    [SerializeField] Transform monsterRoot;
    [SerializeField] Transform player;
    List<Transform> entitys = new List<Transform>( );
    Dictionary<Transform, SpriteRenderer> entity2sr = new Dictionary<Transform, SpriteRenderer>( );
    private void Start( )
    {
        var srs = monsterRoot.GetComponentsInChildren<SpriteRenderer>( );
        foreach ( var sr in srs )
        {
            entitys.Add( sr.transform );
            entity2sr[ sr.transform ] = sr;
        }
        Timer.SetInterval( 0.1f, UpdateLayer );
    }

    private void UpdateLayer( )
    {
        var checkEntitys = FindEntity( );
        var pos = player.position;
        if ( checkEntitys.Count > 0 )
        {
            for ( int i = 0; i < checkEntitys.Count; i++ )
            {
                var entity = checkEntitys[ i ];
                if ( Mathf.Abs( entity.position.y + 0.4f - pos.y ) > 0.1f )
                {
                    entity2sr[ entity ].sortingOrder = 4;
                }
                else
                {
                    entity2sr[ entity ].sortingOrder = 6;
                }
            }
        }
    }

    List<Transform> needSortEntity = new List<Transform>( );
    private List<Transform> FindEntity( )
    {
        needSortEntity.Clear( );

        var pos = player.position;
        for ( int i = 0; i < entitys.Count; i++ )
        {
            if ( Vector3.Distance( entitys[ i ].position, pos ) < 2 )
            {
                needSortEntity.Add( entitys[ i ] );
            }
        }
        return needSortEntity;
    }
}
