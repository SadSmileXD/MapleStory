using UnityEngine;
[System.Serializable]
public class CharData: BaseSheetData
{
    public Sprite m_sprite;
    public Sprite m_sprite2;
    public string JobName;
    public string description1;
    public string description2;
    public string description3;
    public string jobtype;
    public string movetype;
    public string useStats;
    public override void ApplyRowData(string[] Data)
    {
        this.uniqueId = Data[0];
        JobName= Data[1];
        description1 = Data[2];
        
        description2 = Data[3];
        description3 = Data[4].Replace("/", ",");
        jobtype = Data[5];
        movetype = Data[6];
        useStats = Data[7];

    }
}
