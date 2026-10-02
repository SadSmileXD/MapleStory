using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
[CreateAssetMenu(fileName = "DOT_Intro_Data", menuName = "Data/DOT_Intro_Data")]
public class DOT_intro_ani: DOT_Ani
{
    private DOT_Intro_Data m_Data;
    private bool init_flag;
    private Image targetImage;
    [SerializeField] private float frameDuration = 0.1f;
    private Sequence crossfadeSequence;
    override public void Init(Context InitData, Image image )
    {
        m_Data = InitData as DOT_Intro_Data;
        targetImage = image;
        if (m_Data == null)
        {
            init_flag= false;
            Debug.LogError("InitData is not of type DOT_Intro_Data");
            return;
        }
        init_flag = true;
       
    }
    override public void ONDOTweenAni()
    {
        if (init_flag == false) return;


        crossfadeSequence.Kill(); //진행 중인 애니메이션이 없겠지만 혹시나 해서 중단
        crossfadeSequence = DOTween.Sequence();
        for (int i = 0; i < m_Data.sprites.Length; i++)
        {
            int spriteIndex = i;
            // 1. 이미지 표시 유지 대기
            crossfadeSequence.AppendInterval(frameDuration);
            crossfadeSequence.AppendCallback(() =>
            {
                targetImage.sprite = m_Data.sprites[spriteIndex];
            });
           
        }
        crossfadeSequence.OnComplete(() =>
        {
            // 이동할 씬 이름 또는 인덱스 지정
            SceneManager.LoadScene("02_SelectChannel");
            
        });
    }
}
