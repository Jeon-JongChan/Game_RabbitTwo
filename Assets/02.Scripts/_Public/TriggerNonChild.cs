using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 트리거에 자식객체는 ObjectInteraction을 반드시 가지고 있어야 한다. - 상속도 가능 */
public class TriggerNonChild : MonoBehaviour {

    [SerializeField]
    public string[] targetTag;
    [SerializeField]
    public GameObject[] usingTriggerObjects;

    public delegate void voidDelegate(Transform tf);
    private event voidDelegate SetEventFunc;

    private void Start()
    {
        if (usingTriggerObjects.Length > 0)
        {
            foreach (var v in usingTriggerObjects)
            {
                ObjectInteraction interaction = v.GetComponent<ObjectInteraction>();
                if(interaction != null) SetEventFunc += interaction.SetCollisionTarget;
            }
        }
        else
        {
            Debug.Log("MovementTrigger.cs - there aren't Trigger Target");
        }
    }


    private void OnTriggerEnter2D(Collider2D col)
    {
        foreach(var v in targetTag)
        {
            if(col.CompareTag(v))
            {
                print("MovementTrigger - 대상을 감지했습니다.");
                SetEventFunc(col.transform);
            }
        }
    }
}
