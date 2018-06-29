using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingWall : ObjectInteraction
{
    //public GameObject movePoint;
    public Transform aim;
    public void ToAim() {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Move(rb, aim.position);
    }
 
	// Update is called once per frame
	void Update () {
		
	}
}
