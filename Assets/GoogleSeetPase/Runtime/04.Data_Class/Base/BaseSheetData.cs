using UnityEngine;
[System.Serializable]
public abstract class BaseSheetData : ISheetParsable, IIdentifiable
{
    
    [field: SerializeField] public string uniqueId { get; set; }
    public abstract void ApplyRowData(string[] Data);
    
}
