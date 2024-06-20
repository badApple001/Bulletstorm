using UnityEngine;

[DefaultExecutionOrder( -2000 )]
public class GameMain : MonoBehaviour
{

    /// <summary>
    /// ��Ϸ�浵
    /// 
    /// �����㾡�ܶ�, �ҷ�װ����, ��ȡ���ܺ���ͨ�ֵ�ûɶ����, ���治Ҫ����, 
    /// �˳���Ϸ���е���̨��ʱ���Զ�����, ��Ͷ�д�������ͺ���
    /// ȫ�ַ�ʽ��ȡ DiskAgent.Load<GameData>()  �Ϳ�����, �����ȡ�ķ�ʽ���ܸܺ�,���õ�����������, �Ҷ����㿼�Ǻ���
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
            Debug.LogError( "AssetBundle��ʼ��ʧ��" );
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
