using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterCreation : ImgClick
{
    private Image chargenerator;
    public Sprite hover;
    public Sprite nomal;
    public Sprite press;
    public int DelayTime = 0;
    private void Awake()
    {
        chargenerator = GetComponent<Image>();
    }
    public override void OnPointerClick(PointerEventData eventData)
    {
        chargenerator.sprite = press;
        UniTask.Delay(DelayTime).ContinueWith(() =>
        {
            chargenerator.sprite = nomal;
        });
        SceneManager.LoadScene("06_Charator_generator2");

    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        chargenerator.sprite = hover;


         
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        chargenerator.sprite = nomal;
       
    }
}
