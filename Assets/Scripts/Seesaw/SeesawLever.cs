using UnityEngine;

public class SeesawLever : MonoBehaviour, IDraggableHandle
{
    public SeesawController seesawController;

    public void BeginDrag() => seesawController.BeginDrag();
    public void UpdateDrag() => seesawController.UpdateDrag();
    public void EndDrag() => seesawController.EndDrag();
}