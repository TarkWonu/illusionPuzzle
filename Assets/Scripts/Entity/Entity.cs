using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class Entity : MonoBehaviour
{
    
    [SerializeField] private float moveDuration = 2f;
    [SerializeField] private LayerMask layer;

    private Roads currentRoad;
    private Roads targetRoad;
    private Roads previousRoad;

    void Start()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position,Vector3.down,out hit,1f,layer);
        currentRoad = hit.transform.GetComponent<Roads>();
        MoveToNext();
    }

    void MoveToNext()
    {
        if (currentRoad == null)
        {
            Debug.Log("CurrentRoad is Null idiot.");
            return;
        }
        targetRoad = PickNextRoad();
        transform.DOMove(targetRoad.transform.position+ new Vector3(0, 1f, 0) ,moveDuration).
        SetEase(Ease.Linear).OnComplete(() =>
        {
            if (targetRoad != currentRoad)
            {
                previousRoad = currentRoad;
                currentRoad = targetRoad;
            }
            transform.parent = currentRoad.transform;
            
            MoveToNext();
        });
        
            
        
    }

    Roads PickNextRoad()
    {
        Debug.Log("fahhhhhhh");
        var ableRoadList = currentRoad.connectRoad.Where(r=> r != previousRoad&& r!= null).ToList();
        if (ableRoadList.Count > 0)
        {
            if (ableRoadList[0].CheckPlayerInRoad())
            {
                return currentRoad;
            }
            return ableRoadList[0];
        }
        return previousRoad;
    }
}