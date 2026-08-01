using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;



public enum RotateDir
{
    x,y,z
}

public class RotatePath : MonoBehaviour
{
    

    [SerializeField] private float rotationSpeed;
    [SerializeField] public RotateDir rotdir; 
    
    private Vector3 dir = new Vector3(0,0,0);
    
    void Start()
    {
        
        
        
    }

    




   

    public bool CheckPlayerInRoad()
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
