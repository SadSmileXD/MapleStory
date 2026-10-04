using UnityEngine;
[System.Serializable]
public class SecurityKey : BaseSheetData
{
    public string key;
    
    public override void ApplyRowData(string[] Data)
    {
        this.uniqueId = Data[0];
        key = Data[2];
    }
}
