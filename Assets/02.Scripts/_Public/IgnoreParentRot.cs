using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreParentRot : MonoBehaviour {

    public Vector3 _wantFreezeRot;
    public Space _space = Space.World;
	
	// Update is called once per frame
	void LateUpdate () {
        transform.rotation =  Quaternion.Euler(_wantFreezeRot);
	}
}
