
using UnityEngine;
using UnityEngine.EventSystems;

public class RotateHandle : MonoBehaviour,IDraggableHandle
{
    
    public RotatePath rotatePath;

     

    
    


    private float startMouseAngle;
    private Quaternion startObjectAngle;
    private Quaternion startHandleRot;

    public void BeginDrag()
    {
        if(rotatePath.CheckPlayerInRoad()||RoadConnectManager.Instance.state == GameStates.playerMoving) return;
        RoadConnectManager.Instance.state = GameStates.bridgeMoving;

        Vector3 centerScreenPos = Camera.main.WorldToScreenPoint(transform.position);
        
       
        Vector3 dir = Input.mousePosition- centerScreenPos;
        
        
        startMouseAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        
        
        startObjectAngle = transform.rotation;
        startHandleRot = rotatePath.transform.rotation;

        
    }

    public void UpdateDrag()
    {
        if(rotatePath.CheckPlayerInRoad()||RoadConnectManager.Instance.state == GameStates.playerMoving) return;
            Vector3 centerScreenPos = Camera.main.WorldToScreenPoint(transform.position);
            Vector3 dir = Input.mousePosition - centerScreenPos;
            
           
            float currentMouseAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            
            
            float angleDifference = currentMouseAngle - startMouseAngle;

            Vector3 axis = new Vector3();
            

            switch (rotatePath.rotdir)
            {
                case RotateDir.x : axis = Vector3.right; angleDifference *= -1; break;
                case RotateDir.y : axis = Vector3.up; angleDifference *= -1; break;
                case RotateDir.z : axis = Vector3.forward; angleDifference *= -1; break;


            }

            Quaternion delta = Quaternion.AngleAxis(angleDifference,axis); 
            rotatePath.transform.rotation = delta * startHandleRot;
            transform.rotation = delta * startObjectAngle;


            

    }

    public void EndDrag()
    {
        FloorRot(rotatePath.transform);
        FloorRot(transform);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        RoadConnectManager.Instance.state = GameStates.idle;
    }

    void FloorRot(Transform t)
    {
        Vector3 euler = t.rotation.eulerAngles;

        float x = Mathf.Floor(euler.x);
        float y = Mathf.Floor(euler.y);
        float z = Mathf.Floor(euler.z);

        t.rotation = Quaternion.Euler(new Vector3(x,y,z));
    }

    
   
}