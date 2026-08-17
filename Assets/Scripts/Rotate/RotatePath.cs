using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;



public enum RotateDir
{
    x,y,z
}

public class RotatePath : MonoBehaviour
{
    

    [SerializeField] private float rotationSpeed;
    [SerializeField] public RotateDir rotdir; 
    
    LayerMask mask;
    private Vector3 dir = new Vector3(0,0,0);
    
    void Start()
    {
        mask = LayerMask.GetMask("Entity") | LayerMask.GetMask("Player");
        
        
    }

    




   

    public bool CheckPlayerInRoad()
    {
        Transform[] myChildren = this.GetComponentsInChildren<Transform>();
        Debug.Log(mask.value);
        
        foreach(Transform child in myChildren)
        {
            if((mask.value & (1<<child.gameObject.layer)) != 0 )
            {
                return true;
            }
        }
        return false;
            
    }

    public float GetAngle()
    {
        if(rotdir == RotateDir.x)
        {
            return transform.eulerAngles.x;
        }
        if(rotdir == RotateDir.y)
        {
            return transform.eulerAngles.y;
        }
        
        return transform.eulerAngles.z;
        
    }

    
    
}
