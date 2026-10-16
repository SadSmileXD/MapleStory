using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.ComponentModel;
using UnityEngine.UI;
public class World_Set_UI : MonoBehaviour
{
     public List<World_Set_UI_Data> worldDataList;
    public Image m_image;
    private void Awake()
    {
        m_image=GetComponent<Image>();  
    }
    void Start()
    {
        var name = firestoreManager.Instance.m_SelectWorldName;
        foreach (var worldData in worldDataList)
        {
           if(worldData.worldName.ToString()== name)
            {
                m_image.sprite = worldData.worldSprite;
                break;
            }
        }
    }

   
}
[System.Serializable]
public struct World_Set_UI_Data
{
    public MapleWorld worldName;
    public Sprite worldSprite;
}

public enum MapleWorld
{
    [Description("스카니아")]
    Scania,

    [Description("베라")]
    Bera,

    [Description("루나")]
    Luna,

    [Description("제니스")]
    Zenith,

    [Description("크로아")]
    Croa,

    [Description("유니온")]
    Union,

    [Description("엘리시움")]
    Elysium,

    [Description("이노시스")]
    Innosys,

    [Description("레드")]
    Red,

    [Description("오로라")]
    Aurora,

    [Description("아케인")]
    Arcane,

    [Description("노바")]
    Nova,

    [Description("에오스")]
    Eos,

    [Description("핼리오스")]
    Helios
}