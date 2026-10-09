using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class World_Select_UI : MonoBehaviour, 
                              IPointerEnterHandler,// 마우스 마우스가 영역에 들어올 때 (Hover In)
                              IPointerExitHandler,// 마우스가 영역에서 나갈 때 (Hover Out)
                              IPointerClickHandler,// 마우스 클릭이 완료될 때
                              IPointerDownHandler,  // 마우스 버튼을 누르는 순간
                              IPointerUpHandler  // 마우스 버튼을 떼는 순간
{
    private Image m_ui; // UI 이미지
    public Sprite m_hoverImage; //마우스가 호버 했을 때 바뀌는 이미지
    public Sprite m_exitImage;// 마우스가 호버에서 나갔을 때 바뀌는 이미지
    public Sprite m_clickImage;// 마우스 클릭이 완료되었을 때 바뀌는 이미지
    public string m_worldName;// 클릭 시 선택되는 월드 이름
    private void Awake()
    {
        m_ui=GetComponent<Image>();
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        m_ui.sprite = m_hoverImage;
    }
    public virtual void OnPointerExit(PointerEventData eventData)
    {
        m_ui.sprite=m_exitImage;
    }
    public virtual void OnPointerUp(PointerEventData eventData)
    {
        // 클릭 뗌 처리
        m_ui.sprite = m_exitImage;  
    }
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        // 클릭 누름 처리 (예: 눌린 상태 이미지 변경)
        m_ui.sprite = m_clickImage;
    }
    public virtual void OnPointerClick(PointerEventData eventData) 
    {
        m_worldName=this.gameObject.name;
        firestoreManager.Instance.SetWorldName(m_worldName);
        SceneManager.LoadSceneAsync(4);
    }
}
