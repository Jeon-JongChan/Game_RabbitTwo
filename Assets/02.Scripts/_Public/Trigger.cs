using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 트리거에 자식객체는 ObjectInteraction을 반드시 가지고 있어야 한다. - 상속도 가능 */
public class Trigger : MonoBehaviour {

    [SerializeField]
    public string[] targetTag;

    public delegate void voidDelegate(Transform tf);
    private static event voidDelegate SetEventFunc;

    private void Start()
    {
        var childsComponents = GetComponentsInChildren<ObjectInteraction>();
        try {
            foreach (ObjectInteraction o in childsComponents)
            {
                SetEventFunc += o.SetCollisionTargetDirection;
            }
        }
        catch(Exception e)
        {
            Debug.Log("Trigger.cs - 에러" + e.Message);
        }
    }


    private void OnTriggerEnter2D(Collider2D col)
    {
        foreach(var v in targetTag)
        {
            if(col.CompareTag(v))
            {
                print("Trigger - 대상을 감지했습니다.");
                SetEventFunc(col.transform);
            }
        }
    }
}
