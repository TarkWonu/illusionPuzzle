using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public enum PathRotateState
{
    one = 0
    ,two = 90
    ,three = 180
    ,four = 270
}

public enum RotateDir
{
    x,y,z
}

public class RotatePath : MonoBehaviour
{
    public PathRotateState pathRotate;

    [SerializeField] private float rotationSpeed;
    [SerializeField] private RotateDir rotdir; 
    private Vector3 dir = new Vector3(0,0,0);
    void Start()
    {
        
        if(rotdir == RotateDir.x)
        {
            dir = new Vector3(90,0,0);
            transform.rotation = Quaternion.Euler(new Vector3((int)pathRotate,0,0));
        }
        if(rotdir == RotateDir.y)
        {
            dir = new Vector3(0,90,0);
            transform.rotation = Quaternion.Euler(new Vector3(0,(int)pathRotate,0));
        }
        if(rotdir == RotateDir.z)
        {
            dir = new Vector3(0,0,90);
            transform.rotation = Quaternion.Euler(new Vector3(0,0,(int)pathRotate));
        }
        
    }




    public void RotateBridge()
    {
        
        if(RoadConnectManager.Instance.state == GameStates.idle&&!CheckPlayerInRoad())
        {
            
            Sequence seq = DOTween.Sequence();

            seq.Append(transform.DORotate(dir,0.3f,RotateMode.LocalAxisAdd).SetEase(Ease.Linear));
            if ((int)pathRotate < 270)
            {
                pathRotate += 90;
            }
            else
            {
                pathRotate = 0;
            }

            seq.OnStart(() => RoadConnectManager.Instance.state = GameStates.bridgeMoving);
            seq.OnComplete(() => RoadConnectManager.Instance.state = GameStates.idle);
            
        }
        
        
    }

    bool CheckPlayerInRoad()
    {
        Transform[] myChildren = this.GetComponentsInChildren<Transform>();
        
        foreach(Transform child in myChildren)
        {
            if(child.gameObject.layer == 7)
            {
                return true;
            }
        }
        return false;
            
    }

    
    
}
