using Unity.VisualScripting;
using UnityEngine;

public class ClassSelectManager : Singleton<ClassSelectManager>
{
    public ClassSelectData data;

    private void Awake()
    {
        base.Awake();
       
    }
    private void OnEnable()
    {
        EventBus.Subscribe<ClassSelectData>(SaveSelectData);
    }
    private void OnDisable()
    {
        EventBus.Unsubscribe<ClassSelectData>(SaveSelectData);
    }

    public void SaveSelectData(ClassSelectData e)
    {
        data = e;
    }
    public ClassSelectData Get() => data;

    
}
[System.Serializable]
public struct ClassSelectData
{
    public string className; // 직업 이름
    public string classType; // 직업 타입
    public string groupType;

    public ClassSelectData(string a, string b, string c)
    {
        this.className = a;
        this.classType = b;
        this.groupType = c;
    }
}