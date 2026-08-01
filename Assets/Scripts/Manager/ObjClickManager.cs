using System;
using UnityEngine;

public class ObjClickManager : Singleton<ObjClickManager>
{
    public LayerMask roadLayer;
    public LayerMask handleLayer;

    public Action<Roads> playerMove;

    private IDraggableHandle activeDrag;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CheckRay();
        }
        else if (activeDrag != null && Input.GetMouseButton(0))
        {
            activeDrag.UpdateDrag();
        }
        else if (activeDrag != null && Input.GetMouseButtonUp(0))
        {
            activeDrag.EndDrag();
            activeDrag = null;
        }
    }

    void CheckRay()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000f, handleLayer))
        {
            var drag = hit.transform.GetComponent<IDraggableHandle>();
            if (drag != null)
            {
                activeDrag = drag;
                activeDrag.BeginDrag();
                return;
            }

            
            hit.transform.GetComponent<ElevatorHandle>()?.elevatePath.MovePath();
        }
        else if (Physics.Raycast(ray, out hit, 1000f, roadLayer))
        {
            Roads clicked = hit.transform.GetComponent<Roads>();
            if (DebugConsole.ConnectionViewMode)
            {
                if (DebugConsole.ExistNow) DebugConsole.Instance.ShowConnections(clicked);
                return;
            }
            playerMove?.Invoke(clicked);
        }
    }
}