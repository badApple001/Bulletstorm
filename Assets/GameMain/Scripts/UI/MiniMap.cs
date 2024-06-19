using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMap : MonoBehaviour
{
    public Transform player;
    public Transform monsterRoot;
    public GameObject pointPref;
    List<Transform> monsters = new List<Transform>( );
    Dictionary<Transform, Transform> monsterToPoint = new Dictionary<Transform, Transform>( );


    void Start( )
    {
        for ( int i = 0; i < monsterRoot.childCount; i++ )
        {
            var child = monsterRoot.GetChild( i );
            monsters.Add( child );

            var pointObj = GameObject.Instantiate( pointPref, transform );
            pointObj.GetComponent<Image>( ).color = Color.red;
            monsterToPoint.Add( child, pointObj.transform );
        }

        Timer.SetInterval( 0.1f, UpdateMonsterPos );
    }

    void UpdateMonsterPos( )
    {
        if( player == null )
        {
            return;
        }
        var playerPos = player.position;
        Transform monster;
        for ( int i = 0; i < monsters.Count; i++ )
        {
            monster = monsters[ i ];
            if ( monster == null )
            {
                monsters.RemoveAt( i-- );
                continue;
            }
            var dir = monster.position - playerPos;
            var scale = Mathf.Clamp01( dir.magnitude / 9.6f );
            monsterToPoint[ monster ].localPosition = dir.normalized * scale * 115;
        }
    }

    private void OnDestroy( )
    {
        Timer.ClearTimer( UpdateMonsterPos );
    }
}
