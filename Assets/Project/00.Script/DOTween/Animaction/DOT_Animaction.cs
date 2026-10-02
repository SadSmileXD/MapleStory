using UnityEngine;
using UnityEngine.UI;

public class DOT_Animaction : MonoBehaviour
{
    [SerializeField]private Context m_Context;
    [SerializeField]private DOT_Ani m_DOT_Ani;

    private Image targetImage;
    void Start()
    {
        targetImage = GetComponent<Image>();
        m_DOT_Ani.Init(m_Context, targetImage);
        m_DOT_Ani.ONDOTweenAni();
    }

   
}
