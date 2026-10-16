using UnityEngine;

public class CharatorCreateManager : MonoBehaviour
{
    public ClassSelectData CurrentCharatorTypeInfo;
    private void Awake()
    {
        CurrentCharatorTypeInfo= ClassSelectManager.Instance.Get();
        ClassSelectManager.Instance.DestroySingletonObject();
    }

    
}
