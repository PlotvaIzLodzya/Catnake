using UnityEngine;

public abstract class GridMover : MonoBehaviour
{
    public abstract Vector3 LastPos { get; set; }
    public abstract float TimeToCell { get; }
}
