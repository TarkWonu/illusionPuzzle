using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;



public class Roads : MonoBehaviour
{
    
    [Serializable]
    class liusionCondition
    {
        [SerializeReference, SubclassSelector] public List<Condition> conditions = new();
        [SerializeField] public List<Roads> llusionPath = new();
    }

    [Header("환각 경로 연결")]
    
    [SerializeField] private List<liusionCondition> conditionsList;
    
    
    [field:SerializeField] public List<Roads> connectRoad{get; private set;} = new();

    [SerializeField] private LayerMask roadLayer;

    public Roads parentRoad{get;set;}


    private readonly Vector3[] dirs ={Vector3.back,Vector3.forward,
        Vector3.left,Vector3.right};


    void Awake()
    {
        RoadConnectManager.Instance.connectRoad += MakeConnect;
    }
    void OnDestroy()
    {
        if (RoadConnectManager.Instance != null)
            RoadConnectManager.Instance.connectRoad -= MakeConnect;
    }

    public void MakeConnect()
    {
        connectRoad = new();
        parentRoad = null;
        foreach(var item in conditionsList)
        {
            if (item.conditions.Count != 0 && IsCondition(item.conditions))
            {
                foreach(var i in item.llusionPath)
                {
                    connectRoad.Add(i);
                }
            }
        }

        
        
        

        foreach(var dir in dirs)
        {
            if(Physics.Raycast(transform.position,dir,out var info, 1.3f, roadLayer))
            {
                
                connectRoad.Add(info.transform.GetComponent<Roads>());
            }
        }
    }

    public bool IsIllusionConnectedTo(Roads road)
    {
        if (road == null) return false;

        foreach (var item in conditionsList)
        {
            if (item.conditions.Count != 0 &&
                IsCondition(item.conditions) &&
                item.llusionPath.Contains(road))
            {
                return true;
            }
        }

        return false;
    }

    bool IsCondition(List<Condition> conditionsList)
    {
        foreach(var item in conditionsList)
        {
            if (!item.IsCorrect())
            {
                return false;
            }
        }
        return true;
    }
}
