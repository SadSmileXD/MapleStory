using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Testsc : MonoBehaviour
{
    public Image charcreate;
    public Image charcreate2;
    public TextMeshProUGUI t1;
    public TextMeshProUGUI t2;
    public TextMeshProUGUI t3;
    public TextMeshProUGUI t4;
    public TextMeshProUGUI t5;
    public TextMeshProUGUI   t6;
    public TextMeshProUGUI t7;

    public void Start()
    {
        var data= googleSheetManager.Instance.GetClassData<CharData>();
        var data2=data.m_Data[0];
        SetCharData(data2);
    }
    public   void SetCharData (CharData data)
    {
        charcreate.sprite = data.m_sprite;
        charcreate2.sprite = data.m_sprite2;
        t1.text = data.JobName;
        t2.text = data.description1;
        t3.text = data.description2;
        t4.text = data.description3;
        t5.text = data.jobtype;
        t6.text = data.movetype;
        t7.text = data.useStats;
    }
}
