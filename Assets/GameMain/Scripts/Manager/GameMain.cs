using UnityEngine;

[DefaultExecutionOrder(-2000)]
public class GameMain : MonoBehaviour
{

    /// <summary>
    /// ”Œœ∑¥Êµµ
    /// </summary>
    [SerializeField]
    public class GameData {

        public bool firstInGame = false;
    }

    private void Awake( )
    {
        UIManager.Init( GameObject.Find( "HUD/CanvasBase" ).transform );
        if ( !DiskAgent.Load<GameData>().firstInGame )
        {
            UIManager.Create<LoginWindow>( );
        }
    }

    // Start is called before the first frame update
    void Start( )
    {

    }

    // Update is called once per frame
    void Update( )
    {

    }
}
