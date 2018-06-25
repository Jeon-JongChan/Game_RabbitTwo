using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTrapController : MonoBehaviour {

    public MovingWall movingWallRight;
    public MovingWall movingWallLeft;
    public MovingWall movingWallUp;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Palyer") {
            movingWallLeft.ToAim();
            movingWallRight.ToAim();
            movingWallUp.ToAim();
        }
    }
}
