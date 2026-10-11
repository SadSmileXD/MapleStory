using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Cysharp.Threading.Tasks;
public class SoundPoolManager : Singleton<SoundPoolManager>
{
    private Queue<AudioSource> audioSourceQueue = new Queue<AudioSource>();
    [SerializeField] private AudioSource audioSourcePrefab;
    protected override void Awake()
    {
        base.Awake();
        initPool();
    }
     private void initPool()
     {
        for (int i = 0; i < 10; i++)
        {
            var audioSource = Instantiate(audioSourcePrefab, this.gameObject.transform);
            audioSource.gameObject.SetActive(false);
            audioSourceQueue.Enqueue(audioSource);
        }
     }

    private AudioSource GetAudioSource()
    {
        if (audioSourceQueue.Count > 0)
        {
            AudioSource audioSource = audioSourceQueue.Dequeue();
            audioSource.gameObject.SetActive(true);
            return audioSource;
        }
        else
        {
            AudioSource audioSource = Instantiate(audioSourcePrefab, this.gameObject.transform);
            audioSource.gameObject.SetActive(true);
            return audioSource;
        }
    }

    private void ReturnAudioSource(AudioSource  audioSource)
    {
       
            if (audioSource == null) return;
            if (audioSourceQueue.Contains(audioSource)) return; // 중복 반납 예방
            audioSource.gameObject.SetActive(false);
            audioSourceQueue.Enqueue(audioSource);
 
    }
    public async UniTask Play(AudioClip clip)
    {
        if(clip == null)
        {
            Debug.LogWarning("SoundManager: Play called with null AudioClip.");
            return;
        }
        var audioSource = GetAudioSource();
        audioSource.clip = clip;
        audioSource.Play();
        while (audioSource.isPlaying)
        {
            await UniTask.Yield();
        }
        ReturnAudioSource(audioSource);
        return ;
    }
}
