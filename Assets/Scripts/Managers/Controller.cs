using UnityEngine;

public class Controller : MonoBehaviour
{
    public virtual Index CurrentIndex { get; set; }
    public virtual bool AllowPointSetting { get; set; }
    public virtual bool IsCurrentControlPointBomb { get; }
    public virtual bool IsAnableToSet { get; }
    public virtual Vector3 CurrentPointPos { get; }
    public virtual void SetPoint(int index) { }
    public virtual void SaveGameState() { }
}
