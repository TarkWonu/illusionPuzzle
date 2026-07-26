using System;
using UnityEngine;


public class ObjClickManager: Singleton<ObjClickManager>
{
    public LayerMask roadLayer;
    public LayerMask handleLayer;

    public Action<Roads> playerMove;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CheckRay();
        }
    }



    void CheckRay()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if(Physics.Raycast(ray,out hit,1000f,handleLayer))
        {
            hit.transform.GetComponent<RotateHandle>()?.rotatePath.RotateBridge();
            hit.transform.GetComponent<ElevatorHandle>()?.elevatePath.MovePath();
        }else if(Physics.Raycast(ray,out hit, 1000f, roadLayer))
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