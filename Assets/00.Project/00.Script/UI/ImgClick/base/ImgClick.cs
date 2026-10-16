using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class ImgClick : MonoBehaviour, IPointerEnterHandler,// 마우스 마우스가 영역에 들어올 때 (Hover In)
                              IPointerExitHandler,// 마우스가 영역에서 나갈 때 (Hover Out)
                              IPointerClickHandler,// 마우스 클릭이 완료될 때
                              IPointerDownHandler,  // 마우스 버튼을 누르는 순간
                              IPointerUpHandler  // 마우스 버튼을 떼는 순간
{
    /// <summary>
    /// 동일한 UI 요소 위에서 포인터 Press(누름)와 Release(뗌)가 모두 완료되었을 때 호출됩니다.
    /// <para>주요 활용: 버튼 클릭 이벤트, 아이템 선택, 팝업 열기 등 완료된 액션 처리</para>
    /// </summary>
    /// <param name="eventData">포인터의 위치, 클릭 횟수, 클릭한 마우스 버튼 정보 등을 담고 있는 이벤트 데이터</param>
    public virtual void OnPointerClick(PointerEventData eventData)
    {
         
    }

    /// <summary>
    /// 포인터를 UI 요소 영역 안에서 누르는 순간(Down) 즉시 호출됩니다.
    /// <para>주요 활용: 눌렀을 때 즉시 반응해야 하는 꾹 누르기(Hold), 드래그 시작 판정, 눌림 비주얼 연출</para>
    /// </summary>
    /// <param name="eventData">포인터의 위치, 클릭 횟수, 클릭한 마우스 버튼 정보 등을 담고 있는 이벤트 데이터</param>
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        
    }

    /// <summary>
    /// 포인터(마우스 커서 등)가 UI 요소의 Raycast 영역 내부로 진입했을 때 호출됩니다.
    /// <para>주요 활용: 마우스 호버(Hover) 하이라이트 연출, 툴팁 표시, 커서 모양 변경</para>
    /// </summary>
    /// <param name="eventData">포인터의 현재 위치 및 진입 시점의 입력 정보</param>
    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        
    }

    /// <summary>
    /// 포인터가 UI 요소의 Raycast 영역 밖으로 벗어났을 때 호출됩니다.
    /// <para>주요 활용: 마우스 호버 연출 해제, 표시 중인 툴팁 숨기기</para>
    /// </summary>
    /// <param name="eventData">포인터의 현재 위치 및 이탈 시점의 입력 정보</param>
    public virtual void OnPointerExit(PointerEventData eventData)
    {
         
    }

    /// <summary>
    /// 눌려 있던 포인터를 떼는 순간(Up) 호출됩니다. (UI 영역 안팎 상관없이 누른 상태에서 떼면 발생)
    /// <para>주요 활용: 꾹 누르기(Hold) 종료, 누름 연출 원복, 드래그 상태 해제</para>
    /// </summary>
    /// <param name="eventData">포인터의 위치, 클릭한 마우스 버튼 정보 등을 담고 있는 이벤트 데이터</param>
    public virtual void OnPointerUp(PointerEventData eventData)
    {

    }

}
