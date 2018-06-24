using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class CloudControler : ObjectControl {
    public GameObject MovePoints;
    public GameObject[] arr;
    Rigidbody2D rigidbody;
  

    void Start () {
        arr = new GameObject[MovePoints.transform.childCount];
        for (int i = 0; i < MovePoints.transform.childCount; i++) {
            arr[i] = MovePoints.transform.GetChild(i).gameObject;
        }
        rigidbody = GetComponent<Rigidbody2D>();
        InitVelocity(rigidbody);
    }
	
	// Update is called once per frame
	void Update () {
        
        StartCoroutine(ObjectTargetsTraceMove2D(rigidbody, arr, arr.Length));       
    }

    
}
