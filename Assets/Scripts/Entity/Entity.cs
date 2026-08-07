using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Entity : MonoBehaviour
{
    [Header("경로 설정")]
    [SerializeField] private List<Roads> path;

    [Header("이동 설정")]
    [SerializeField] private float moveDuration = 0.2f;
    [SerializeField] private float stepInterval = 0.4f;
    [SerializeField] private Vector3 heightOffset = new Vector3(0, 0.5f, 0);

    private Roads currentRoad;
    private bool reversed;
    private int index = 0;
    private bool moving;

    void Start()
    {
        currentRoad = path[0];
        transform.parent = currentRoad.transform;
        transform.position = currentRoad.transform.position + heightOffset;

        InvokeRepeating(nameof(MoveEntity), stepInterval, stepInterval);
        RoadConnectManager.Instance.connectRoad.Invoke();
    }

    void MoveEntity()
    {
        if (moving) return;
        

        if (index <= 0)              reversed = false;
        if (index >= path.Count - 1) reversed = true;

        int nextIndex = Mathf.Clamp(index + (reversed ? -1 : 1), 0, path.Count - 1);
        Roads next = path[nextIndex];

        if (IsConnected(next))
        {
            StepTo(next, nextIndex);
        }
        else
        {
            
            reversed = !reversed;
            int backIndex = Mathf.Clamp(index + (reversed ? -1 : 1), 0, path.Count - 1);
            Roads back = path[backIndex];

            if (backIndex != index && IsConnected(back))
                StepTo(back, backIndex);
            
        }
    }

    bool IsConnected(Roads road)
    {
        return currentRoad == road || currentRoad.connectRoad.Contains(road);
    }

    void StepTo(Roads next, int nextIndex)
    {
        moving = true;
        index = nextIndex;

        
        Vector3 targetPos = next.transform.position + heightOffset;

        transform.DOMove(targetPos, moveDuration).OnComplete(() =>
        {
            // 도착 후에 밟은 길로 부모 교체 (그 길이 돌면 같이 돌게)
            currentRoad = next;
            transform.SetParent(next.transform);
            moving = false;
            RoadConnectManager.Instance.connectRoad.Invoke();
        });
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector3.down);
    }
}