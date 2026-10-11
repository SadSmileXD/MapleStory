using Cysharp.Threading.Tasks;
using System.Collections;
using UnityEngine;

public class SendBGM : MonoBehaviour
{
    public AudioClip clip;
    private  void Awake()
    {
        
        StartCoroutine(SendCoroutine());
    }
    public IEnumerator SendCoroutine()
    {
        while (!BGM.IsInitialized)
        {
            yield return null; // 다음 프레임까지 대기 (무한 루프 멈춤 방지)
        }
        BGM.PlayBGM(clip);
    }
   
}
