using UnityEngine;
using UnityEngine.EventSystems;

public class SendData : MonoBehaviour, IPointerClickHandler
{
    // 현재 활성화되어 있는 선택 오브젝트를 공유/추적하는 static 변수
    private static GameObject _currentSelected;

    [Header("Class Data")]
    public string className; // 직업 이름
    public string classType; // 직업 타입
    public string groupType;

    // 본인 직계 자식 오브젝트
    private GameObject _mySelectObject;

    private void Awake()
    {
        // 바로 아래 첫 번째 직계 자식 참조
        if (transform.childCount > 0)
        {
            _mySelectObject = transform.GetChild(0).gameObject;
            _mySelectObject.SetActive(false); // 시작 시 기본 비활성화
        }
        else
        {
            Debug.LogWarning($"[{gameObject.name}] 자식 오브젝트가 존재하지 않습니다.", this);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 1. 이벤트 데이터 발행
        EventBus.Publish(new ClassSelectData(className, classType, groupType));

        if (_mySelectObject == null) return;

        // 2. 이전에 선택된 다른 오브젝트가 있다면 비활성화
        if (_currentSelected != null && _currentSelected != _mySelectObject)
        {
            _currentSelected.SetActive(false);
        }

        // 3. 자기 자신의 자식 오브젝트 활성화 및 static 참조 갱신
        _mySelectObject.SetActive(true);
        _currentSelected = _mySelectObject;
    }
}