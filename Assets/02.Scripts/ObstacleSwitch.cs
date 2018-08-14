using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSwitch : MonoBehaviour {
    EventObstacle[] evnObstacle;
    

    public delegate void voidDelegate(Collider2D col);
    private static event voidDelegate SetEventFunc;
    

    private void Start()
    {
        evnObstacle = GetComponentsInChildren<EventObstacle>();
        foreach (EventObstacle o in evnObstacle)
        {
            SetEventFunc  += o.OpenDoor;
        }
       
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        if(col.CompareTag("Player"))
        {
            SetEventFunc(col);
            print("스위치- 플레이어 감지");
        }
    }
        

}
