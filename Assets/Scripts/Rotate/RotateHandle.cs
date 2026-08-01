using UnityEngine;
using UnityEngine.EventSystems;

public class RotateHandle : MonoBehaviour,IDraggableHandle
{
    
    public RotatePath rotatePath;

     

    
    [SerializeField] private float sensitivity = 1f;

    private Vector3 dir;

    private float previousMouseAngle;

    public void BeginDrag()
    {
        if(rotatePath.CheckPlayerInRoad()||RoadConnectManager.Instance.state == GameStates.playerMoving) return;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        RoadConnectManager.Instance.state = GameStates.bridgeMoving;

        if(rotatePath.rotdir == RotateDir.x)
        {
            dir = new Vector3(1,0,0);
        }
        if(rotatePath.rotdir == RotateDir.y)
        {
            dir = new Vector3(0,1,0);
        }
        if(rotatePath.rotdir == RotateDir.z)
        {
            dir = new Vector3(0,0,1);
        }
    }

    public void UpdateDrag()
    {
        if(rotatePath.CheckPlayerInRoad()||RoadConnectManager.Instance.state == GameStates.playerMoving) return;
        float mouseX = Input.GetAxis("Mouse X");
        transform.Rotate(dir*sensitivity*mouseX);

        rotatePath.gameObject.transform.rotation = transform.rotation;

    }

    public void EndDrag()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        RoadConnectManager.Instance.state = GameStates.idle;
    }

    
   
}