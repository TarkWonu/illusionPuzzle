using System;
using UnityEngine;
public enum GameStates
{
    idle,playerMoving,bridgeMoving
}

public class RoadConnectManager : Singleton<RoadConnectManager>
{
    public Action connectRoad;

    public GameStates state;

    void Update()
    {
        Debug.Log(state);
    }

    
}