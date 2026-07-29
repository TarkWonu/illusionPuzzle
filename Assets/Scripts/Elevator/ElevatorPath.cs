using System;
using DG.Tweening;
using UnityEngine;

public enum PathMoveState
{
    pos1,pos2
}
public class ElevatorPath : MonoBehaviour
{

    [SerializeField] private Vector3 pos1;
    [SerializeField] private Vector3 pos2;
    public PathMoveState moveState;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(moveState == PathMoveState.pos1)
        {
            transform.position = pos1;
        }
        else if(moveState == PathMoveState.pos2)
        {
            transform.position = pos2;
        }
    }

    // Update is called once per frame
    public void MovePath()
    {
        if(RoadConnectManager.Instance.state == GameStates.idle)
        {
            
            Sequence seq = DOTween.Sequence();

            if(moveState == PathMoveState.pos1)
            {
                seq.Append(transform.DOMove(pos2,0.3f));
                moveState = PathMoveState.pos2;
            }
            else if(moveState == PathMoveState.pos2)
            {
                seq.Append(transform.DOMove(pos1,0.3f));
                moveState = PathMoveState.pos1;
            }
            
            

            seq.OnStart(() => RoadConnectManager.Instance.state = GameStates.bridgeMoving);
            seq.OnComplete(() => RoadConnectManager.Instance.state = GameStates.idle);
            
        }
    }
}
