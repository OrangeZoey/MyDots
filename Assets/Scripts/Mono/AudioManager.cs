using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public AudioSource audioSource;
    public AudioClip shootAudioClip;
    public AudioClip hitAudioClip;

    public float playHitAudioInterval = 0.2f;//播放间隔 0.2f
    //public float playHitAudioTime = 0.1f;//有效时间
    public float lastPlayHitAudioTime;//最后时间

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// 射击
    /// </summary>
    public void PlayShootAudio()
    {
        audioSource.PlayOneShot(shootAudioClip);
    }

    /// <summary>
    /// 被击打
    /// </summary>
    public void PlayHitAudio()
    {
        audioSource.PlayOneShot(hitAudioClip);
    }

    private void Update()
    {
        //当前需要播放  
        //到了播放的间隔
        //隔一帧在播放
        if (SharedData.gameShareData.Data.playHitAudio && Time.time - lastPlayHitAudioTime > playHitAudioInterval && Time.time - SharedData.gameShareData.Data.playHitAudioTime < Time.deltaTime)
        {
            lastPlayHitAudioTime= Time.time;
            PlayHitAudio();
            SharedData.gameShareData.Data.playHitAudio = false;
        }
    }

}
