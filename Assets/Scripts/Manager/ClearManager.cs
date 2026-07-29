using System.Collections.Generic;
using PixeLadder.EasyTransition;
using UnityEngine;


public class ClearManager : Singleton<ClearManager>
{

    protected override bool UseDontDestroyOnLoad => false;
    public string prevScene;    
    public string nextScene;

    public List<Goal> goals;


    void Update()
    {
        if (IsCleared()&&goals.Count>=1)
        {
            MoveNextScene();
        }
    }





    bool IsCleared()
    {
        foreach(var item in goals)
        {
            if (!item.triggered)
            {
                return false;
            }
        }
        return true;
    }

    public void MovePrevScene()
    {
        if(prevScene=="") return;
        SceneTransitioner.Instance.LoadScene(prevScene);
    }

    public void MoveNextScene()
    {
        if(nextScene=="") return;
        SceneTransitioner.Instance.LoadScene(nextScene);
    }
}