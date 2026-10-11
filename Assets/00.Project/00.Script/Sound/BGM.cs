using Cysharp.Threading.Tasks;
using UnityEngine;

public class BGM : MonoBehaviour
{
   public static  AudioSource m_AudioSource;
    public static bool IsInitialized => m_AudioSource != null;
    void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();
    }

    public  static void PlayBGM(AudioClip clip)
    {
        if (!IsInitialized)
        {
            Debug.LogError("[BGM] AudioSource가 아직 초기화되지 않았습니다.");
            return;
        }
        
        if (clip == null)
        {
            Debug.LogWarning("PlayBGM called with null AudioClip.");
            return;
        }
        m_AudioSource.Stop();
        m_AudioSource.clip = clip;
        m_AudioSource.Play();
    }
    public static async UniTask SkillBGM(AudioClip clip)
    {
        var CurrentBGM= m_AudioSource.clip;
        m_AudioSource.Stop();
        m_AudioSource.loop = false;
        m_AudioSource.clip = clip;
        m_AudioSource.Play();

        while(m_AudioSource.isPlaying)
        {
            await UniTask.Yield();
        }
        m_AudioSource.loop = true;
        m_AudioSource.clip = CurrentBGM;
        m_AudioSource.Play();

    }
}
