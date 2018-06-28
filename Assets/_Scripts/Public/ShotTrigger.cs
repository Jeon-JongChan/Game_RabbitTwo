using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotTrigger : MonoBehaviour {
    Shoter[] shoters;

    public delegate void voidDelegate(Transform tf);
    private static event voidDelegate SetEventFunc;

    private void Start()
    {
        shoters = GetComponentsInChildren<Shoter>();
        foreach(Shoter s in shoters)
        {
            print(s.gameObject.name);
            SetEventFunc += s.SetCollisionTargetDirection;
        }
    }


    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.CompareTag("Player"))
        {
            print("ShotTrigger - 플레이어를 감지했습니다.");
            SetEventFunc(col.transform);
        }
    }
}
