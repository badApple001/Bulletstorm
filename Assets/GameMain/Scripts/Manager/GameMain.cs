using UnityEngine;

[DefaultExecutionOrder( -2000 )]
public class GameMain : MonoBehaviour
{

    /// <summary>
    /// 游戏存档
    /// 
    /// 数据你尽管读, 我封装好了, 读取性能和普通字典没啥区别, 保存不要担心, 
    /// 退出游戏和切到后台的时候自动保存, 你就读写这个对象就好了
    /// 全局方式获取 DiskAgent.Load<GameData>()  就可以了, 这个获取的方式性能很高,不用担心性能问题, 我都帮你考虑好了
    /// 
    /// </summary>
    [SerializeField]
    public class GameData
    {
        public bool firstInGame = false;
        private long _exp = 0;
        public long exp
        {
            get
            {
                return _exp;
            }
            set
            {
                _exp = value;
                MsgFire.Event( GameEventName.ON_PLAYER_EXP_CHANGE, _exp );
            }
        }
    }

    private void Awake( )
    {
        UIManager.Init( GameObject.Find( "HUD/CanvasBase" ).transform );
        Quark.QuarkLauncher.Ins.OnLaunchCallback += AssetBundleInitializetion;
    }

    private void AssetBundleInitializetion( bool suc )
    {
        if ( suc )
        {
            UIManager.Create<LoginWindow>( );
            if ( !DiskAgent.Load<GameData>( ).firstInGame )
            {
                UIManager.Create<GuideWindow>( );
            }
        }
        else
        {
            Debug.LogError( "AssetBundle初始化失败" );
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
