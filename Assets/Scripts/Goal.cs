using TMPro;
using UnityEngine;


public class Goal : MonoBehaviour
{
    [SerializeField] private float playerDis;
    public bool triggered{get; private set;}

    
    void Start()
    {
        
    }

    void Update()
    {
        CheckGoal();
        Debug.Log(Vector3.Distance(transform.position, GetPlayerPos()));
    }

    void CheckGoal()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (Vector3.Distance(transform.position, GetPlayerPos()) < playerDis)
            {
                Debug.Log("Triggered!");
                triggered = true;
                GetComponent<TMP_Text>().text = "";
            }
        }
    }

    Vector3 GetPlayerPos()
    {
        return GameObject.FindGameObjectWithTag("Player").transform.position;
    }








    void OnDrawGizmos()
    {
        

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, playerDis);
    } 
}