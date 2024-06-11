using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public AudioSource audioSource;
    public AudioClip shootAudioClip;
    public AudioClip hitAudioClip;

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

}
