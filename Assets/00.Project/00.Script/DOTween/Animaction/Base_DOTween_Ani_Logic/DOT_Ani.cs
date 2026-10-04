using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public abstract class DOT_Ani:ScriptableObject
{
    public abstract void Init(Context InitData, Image image);
    public abstract void ONDOTweenAni();
    
}
