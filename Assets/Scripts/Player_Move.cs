
using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Player_Move : MonoBehaviour
{
    [SerializeField] private LayerMask layer;

    [SerializeField] private Vector3 rayCastoffset;

    

   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ObjClickManager.Instance.playerMove += CheckClick;
    }
    void OnDestroy()
    {
        if(ObjClickManager.Instance.playerMove != null)
        {
            ObjClickManager.Instance.playerMove -= CheckClick;

        }
    }





    void CheckClick(Roads goal)
    {

        
        if (RoadConnectManager.Instance.state != GameStates.idle) return;
        

    
        List<Roads> route = new();

        Sequence mvseq = DOTween.Sequence();
        Sequence roseq = DOTween.Sequence();
        
        route = GetRoute(goal);
        
        if(route!= null&&RoadConnectManager.Instance.state == GameStates.idle)
        {
            transform.SetParent(goal.transform);
            
            Vector3 prevPos = transform.position;
            float moveDuration = 0.2f;
            foreach (var road in route)
            {
                Vector3 targetPos = road.transform.position + new Vector3(0, 0.5f, 0);
                Vector3 dir = targetPos - prevPos;
                dir.y = 0f;
                bool isIllusionConnection = road.parentRoad != null && road.parentRoad.IsIllusionConnectedTo(road);

                if (!isIllusionConnection && dir.sqrMagnitude > 0f)
                {
                    Quaternion lookRot = Quaternion.LookRotation(dir.normalized);
                    roseq.Append(transform.DORotateQuaternion(lookRot, moveDuration));
                }
                else
                {
                    roseq.AppendInterval(moveDuration);
                }

                mvseq.Append(transform.DOMove(targetPos, moveDuration).SetEase(Ease.Linear));

                prevPos = targetPos;
            }
            mvseq.OnStart(() =>
            {
                RoadConnectManager.Instance.state = GameStates.playerMoving;
            });
            mvseq.OnComplete(() =>
            {
                RoadConnectManager.Instance.state = GameStates.idle;
            });
        }
        else
        {
            Debug.LogWarning("갈수없는 경로");
        }
        
    }

    List<Roads> GetRoute(Roads goal)
    {
        RaycastHit hit;
        Roads startPos;
       HashSet<Roads> visited = new();

       RoadConnectManager.Instance.connectRoad.Invoke();
       Queue<Roads> q = new();
        
        List<Roads> route = new();
        Debug.DrawRay(transform.position+rayCastoffset, Vector3.down, Color.red, 1f);
        if(Physics.Raycast(transform.position+rayCastoffset,Vector3.down,out hit,1f,layer))
        {
            startPos = hit.transform.GetComponent<Roads>();
            visited.Add(startPos);
            q.Enqueue(startPos);
            Debug.Log("와샌즈");
            
        }
        else
        {
            return null;
        }

        while (q.Count != 0)
        {
            Roads road = q.Dequeue();

            if(road == goal)
            {
                Roads tmp = road;
                while(tmp.parentRoad != null)
                {
                    route.Add(tmp);
                    tmp = tmp.parentRoad;
                }
                route.Reverse();
                return route;
            }

            foreach(var item in road.connectRoad)
            {
                if (!visited.Contains(item))
                {
                    item.parentRoad = road;
                    visited.Add(item);
                    q.Enqueue(item);
                    
                }
            }
        }

        return null;

        
    }


}
