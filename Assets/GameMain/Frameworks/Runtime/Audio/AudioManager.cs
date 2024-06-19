//==========================
// - FileName:      Assets/Frameworks/Scripts/Audio/AudioManager.cs
// - Created:       ChenJC	
// - CreateTime:    2023-06-19 10:03:20
// - UnityVersion:  2019.4.35f1
// - Version:       1.0
// - Description:   
//==========================
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    //自由的音效组件
    private Queue<AudioSource> freeAudioSources = new Queue<AudioSource>( );
    //激活中的音效组件
    private List<AudioSource> activeAudioSources = new List<AudioSource>( );
    //已经缓存的音效池
    private Dictionary<string, AudioClip> audioClipCachePool = new Dictionary<string, AudioClip>( );
    //music特征
    private HashSet<AudioSource> musics = new HashSet<AudioSource>( );
    //音效音量
    private float _soundVolume = 1.0f;

    /// <summary>
    /// 同一帧内的相同音效Chan
    /// </summary>
    private HashSet<string> effectChan = new HashSet<string>( );

    /// <summary>
    /// 
    /// 设置 音效系统音量
    /// 
    /// 第一: 这个音量修改会影响后续音量的整体大小
    /// 第二: 这个音量会影响当前播放中的音量    
    ///     例如: 
    ///     a播放音量是 0.7,  b播放音量是1   
    ///     那修改这个volume = 0.8   
    ///     那最终 a的音量是 0.56 即0.7的80%
    ///     那最终 b的音量是 0.8  即1.0的80%
    /// 
    /// 可以理解为总线音量控制
    /// 
    /// </summary>
    /// <param name="volume"> 0f ~ 1f </param>
    public float soundVolume
    {
        set
        {
            float newVolume = Mathf.Clamp( value, 0, 1 );
            float scale = newVolume / _soundVolume;
            _soundVolume = newVolume;

            foreach ( var source in activeAudioSources )
            {
                if ( !musics.Contains( source ) )
                {
                    source.volume *= scale;
                }
            }
            DiskAgent.SetFloat( $"{Application.productName}_soundVolume", newVolume );
        }
        get
        {
            return _soundVolume;
        }
    }

    //音乐音量                 
    private float _musicVolume = 1.0f;

    /// <summary>
    /// 
    /// 设置所有音乐系统音量
    /// 
    /// 第一: 这个音量修改会影响后续音量的整体大小
    /// 第二: 这个音量会影响当前播放中的音量    
    ///     例如: 
    ///     a播放音量是 0.7,  b播放音量是1   
    ///     那修改这个volume = 0.8   
    ///     那最终 a的音量是 0.56 即0.7的80%
    ///     那最终 b的音量是 0.8  即1.0的80%
    /// 
    /// 可以理解为总线音量控制
    /// 
    /// </summary>
    /// <param name="volume">0f ~ 1f</param>
    public float musicVolume
    {
        set
        {
            float newVolume = Mathf.Clamp( value, 0, 1 );
            float scale = newVolume / _musicVolume;
            _musicVolume = newVolume;

            foreach ( var asource in musics )
            {
                asource.volume *= scale;
            }

            DiskAgent.SetFloat( $"{Application.productName}_musicVolume", newVolume );
        }
        get { return _musicVolume; }
    }

    //音效静音状态
    private bool _soundMute = false;

    /// <summary>
    /// 设置所有音效的静音状态
    /// </summary>
    /// <param name="isMute"></param>
    public bool soundMute
    {
        set
        {
            _soundMute = value;
            foreach ( var source in activeAudioSources )
            {
                if ( !musics.Contains( source ) )
                {
                    source.mute = _soundMute;
                }
            }

            DiskAgent.SetBool( $"{Application.productName}_soundMute", value );
        }
        get { return _soundMute; }
    }

    //音量静音状态
    private bool _musicMute = false;

    /// <summary>
    /// 设置所有Music的静音状态
    /// </summary>
    /// <param name="isMute"></param>
    public bool musicMute
    {
        set
        {
            _musicMute = value;
            foreach ( var asource in musics )
            {
                asource.mute = _musicMute;
            }

            DiskAgent.SetBool( $"{Application.productName}_musicMute", value );
        }
        get { return _musicMute; }
    }


    private static AudioManager instance = null;
    public static AudioManager GetInstance( ) {
        if( instance == null )
        {
            Log.Error( $"{nameof(AudioManager)} instance is null." );
        }
        return instance;
    }

    [SerializeField] private bool LoadOnAwake = true;
    /// <summary>
    /// 预加载音效配置
    /// </summary>
    private void Awake( )
    {
        if ( instance != null && instance != this )
        {
            DestroyImmediate( instance );
            return;
        }
        instance = this;

        soundVolume = DiskAgent.GetFloat( $"{Application.productName}_soundVolume", 1f );
        musicVolume = DiskAgent.GetFloat( $"{Application.productName}_musicVolume", 1f );
        soundMute = DiskAgent.GetBool( $"{Application.productName}_soundMute", false );
        musicMute = DiskAgent.GetBool( $"{Application.productName}_musicMute", false );

        if ( LoadOnAwake )
        {
            LoadDefaultAudioAsset( );
        }
    }

    public bool HasLoadDefaultAudioAsset { private set; get; } = false;
    public void LoadDefaultAudioAsset( )
    {
        if ( HasLoadDefaultAudioAsset )
        {
            return;
        }
        HasLoadDefaultAudioAsset = true;
        AudioSettingAsset audioSettingAsset = Resources.Load<AudioSettingAsset>( "AudioSettingAsset" );
        Add( audioSettingAsset );
    }

    /// <summary>
    /// 通过音效预设文件 加载新的音效到内存池中
    /// </summary>
    /// <param name="audioSettingAsset"></param>
    public void Add( AudioSettingAsset audioSettingAsset )
    {
        Add( audioSettingAsset.settings );
        Resources.UnloadAsset( audioSettingAsset );
    }

    /// <summary>
    /// 加载音效到内存中
    /// </summary>
    /// <param name="audioSettingItems"></param>
    public void Add( List<AudioSettingItem> audioSettingItems )
    {
#if UNITY_EDITOR
        List<string> added = new List<string>( );
#endif
        foreach ( var setting in audioSettingItems )
        {
            if ( !audioClipCachePool.ContainsKey( setting.name ) )
            {
                var ac = AssetLoader.Load<AudioClip>( setting.url );
                audioClipCachePool.Add( setting.name, ac );
#if UNITY_EDITOR
                added.Add( setting.name );
#endif
            }
        }
#if UNITY_EDITOR
        if ( added.Count > 0 )
        {
            Log.Info( $"新增音效: {string.Join( ",", added )}" );
        }
#endif
    }


    /// <summary>
    /// 释放所有音效占用
    /// </summary>
    public void UnloadAllAudioClip( )
    {
        foreach ( var source in activeAudioSources )
        {
            Destroy( source );
        }
        foreach ( var source in freeAudioSources )
        {
            Destroy( source );
        }
        foreach ( var pair in audioClipCachePool )
        {
            Destroy( pair.Value );
        }

        activeAudioSources.Clear( );
        freeAudioSources.Clear( );
        musics.Clear( );
        audioClipCachePool.Clear( );
    }

    /// <summary>
    /// 检测音效的运行状态
    /// </summary>
    public void Update( )
    {
        for ( int i = 0; i < activeAudioSources.Count; i++ )
        {
            if ( !activeAudioSources[ i ].isPlaying )
            {
                var free = activeAudioSources[ i ];
                free.gameObject.SetActive( false );
                freeAudioSources.Enqueue( free );
                activeAudioSources.RemoveAt( i-- );
                if ( musics.Contains( free ) )
                {
                    musics.Remove( free );
                }
            }
        }


        if ( Time.frameCount % 5 == 0 )
        {
            effectChan.Clear( );
        }
    }

    /// <summary>
    /// 播放音乐 默认循环播放
    /// </summary>
    public void PlayMusic( string audioname, float volume = 1.0f, bool loop = true )
    {
        if ( IsPlaying( audioname ) )
        {
            return;
        }

        //切换bgm 默认是不能存在多个bgm
        if( loop )
        {
            StopAllMusic( );
        }

        var clip = GetAudioClip( audioname );
        if ( null == clip )
        {
            return;
        }

        if ( freeAudioSources.Count == 0 )
        {
            GenAudioSouceNew( );
        }
        var audioSource = freeAudioSources.Dequeue( );
        activeAudioSources.Add( audioSource );
        audioSource.gameObject.SetActive( true );
        audioSource.volume = volume * _musicVolume;
        audioSource.mute = _soundMute;
        audioSource.loop = loop;
        audioSource.spatialBlend = 0f;
        audioSource.clip = clip;
        audioSource.Play( );
        if ( !musics.Contains( audioSource ) )
        {
            musics.Add( audioSource );
        }
    }


    /// <summary>
    /// 播放音效 默认单次播放
    /// </summary>
    /// <param name="audioname"></param>
    /// <param name="volume"></param>
    /// <param name="loop"></param>
    public void PlaySound( string audioname, float volume = 1.0f, bool loop = false )
    {

        if ( effectChan.Contains( audioname ) )
        {
            Log.Light( $"[AudioManager] PlaySound not Contain: {audioname} " );
            return;
        }
        effectChan.Add( audioname );

        var clip = GetAudioClip( audioname );
        if ( null == clip )
        {
            Log.Light( $"[AudioManager] PlaySound clip is null: {audioname} " );
            return;
        }

        if ( freeAudioSources.Count == 0 )
        {
            GenAudioSouceNew( );
        }
        var audioSource = freeAudioSources.Dequeue( );
        activeAudioSources.Add( audioSource );
        audioSource.gameObject.SetActive( true );
        audioSource.volume = volume * _soundVolume;
        audioSource.mute = _soundMute;
        audioSource.loop = loop;
        audioSource.spatialBlend = 0f;
        audioSource.clip = clip;
        audioSource.Play( );


        Log.Green( $"[AudioManager] PlaySound: {audioname} " );
    }

    /// <summary>
    /// 停掉所有audioname相关的音效
    /// 因为是可以同时存在多个相同audioname的音效的  它们都有自己的生命周期
    /// </summary>
    /// <param name="audioname"></param>
    //public void Stop( string audioname ) => activeAudioSources.FindAll( a => { return a.clip != null && a.clip.name == audioname; } ).ForEach( s => s.Stop( ) );
    public void Stop( string audioname, bool matchAll = true )
    {
        if ( audioClipCachePool.TryGetValue( audioname, out var cache ) )
        {
            if ( matchAll )
            {
                var playingAudioSources = activeAudioSources.FindAll( a => a.clip == cache );
                if ( null != playingAudioSources && playingAudioSources.Count > 0 )
                {
                    playingAudioSources.ForEach( audioSource =>
                    {
                        audioSource.Stop( );
                        audioSource.gameObject.SetActive( false );
                        activeAudioSources.Remove( audioSource );
                        freeAudioSources.Enqueue( audioSource );
                        if ( musics.Contains( audioSource ) )
                        {
                            musics.Remove( audioSource );
                        }
                    } );
                }
            }
            else
            {
                var audioSource = activeAudioSources.Find( a => a.clip == cache );
                if ( null != audioSource )
                {
                    audioSource.Stop( );
                    audioSource.gameObject.SetActive( false );
                    activeAudioSources.Remove( audioSource );
                    freeAudioSources.Enqueue( audioSource );
                    if ( musics.Contains( audioSource ) )
                    {
                        musics.Remove( audioSource );
                    }
                }
            }
        }
    }

    /// <summary>
    /// 停止所有bgm
    /// </summary>
    public void StopAllMusic( )
    {
        if( musics.Count == 0 ) {
            return;
        }
        var playingAudioSources = activeAudioSources.FindAll( a => musics.Contains( a ) );
        if ( null != playingAudioSources && playingAudioSources.Count > 0 )
        {
            playingAudioSources.ForEach( audioSource =>
            {
                audioSource.Stop( );
                audioSource.gameObject.SetActive( false );
                activeAudioSources.Remove( audioSource );
                freeAudioSources.Enqueue( audioSource );
            } );
            musics.Clear();
        }
    }

    /// <summary>
    /// 音乐/音效 是否在播放中
    /// </summary>
    public bool IsPlaying( string audioname )
    {
        if ( audioClipCachePool.TryGetValue( audioname, out var cache ) )
        {
            return activeAudioSources.Exists( a => a.clip == cache );
        }
        return false;
    }

    /// <summary>
    /// 3D世界播放一个音效  指定一个位置
    /// </summary>
    /// <param name="audioname"></param>
    /// <param name="position"></param>
    /// <param name="volume"></param>
    public void PlaySoundAtPoint( string audioname, Vector3 position, float volume = 1f, bool loop = false )
    {
        var clip = GetAudioClip( audioname );
        if ( null == clip )
        {
            return;
        }

        if ( freeAudioSources.Count == 0 )
        {
            GenAudioSouceNew( );
        }
        var audioSource = freeAudioSources.Dequeue( );
        activeAudioSources.Add( audioSource );
        audioSource.gameObject.transform.position = position;
        audioSource.gameObject.SetActive( true );
        audioSource.clip = clip;
        audioSource.volume = volume * _soundVolume;
        audioSource.mute = _soundMute;
        audioSource.loop = loop;
        audioSource.spatialBlend = 1f;
        audioSource.Play( );
    }

    public AudioClip GetAudioClip( string audioname )
    {
        if ( audioClipCachePool.TryGetValue( audioname, out AudioClip clip ) )
        {
            return clip;
        }
        return null;
    }

    private void GenAudioSouceNew( )
    {
        var obj = new GameObject( $"AudioComponent_{( activeAudioSources.Count + freeAudioSources.Count )}" );
        obj.transform.SetParent( transform, true );
        var asource = obj.AddComponent<AudioSource>( );
        asource.loop = false;
        asource.volume = 0;
        asource.playOnAwake = false;
        asource.mute = false;
        freeAudioSources.Enqueue( asource );
    }
}

