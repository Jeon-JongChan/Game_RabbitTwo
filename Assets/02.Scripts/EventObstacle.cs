using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PsybleScript;

public class EventObstacle : ObjectMovement2D{
    MovingObject movOb;
    Collider2D col;
    Rigidbody2D rg2d;
    public bool open = false;
    

    [Tooltip("0 - UP 1 - RIGHT 2 - DOWN 3 - LEFT ")]
    [Range(0,3)]
    public int direction;
    public float distance;

    Vector2 dir;


    public void OpenDoor(Collider2D col)
    {
        if(col.gameObject.tag =="Player" )
        {
            ToDestination(rg2d, 0.5f, 0);
            open = true;

        }
        
    }

    protected void ToDestination(Rigidbody2D rg2d, float speed, float accelation)
    {
        switch (direction)
        {
            case 0:
                dir = Vector2.up;
                break;
            case 1:
                dir = Vector2.right;
                break;
            case 2:
                dir = Vector2.down;
                break;
            case 3:
                dir = Vector2.left;
                break;
            default:
                print("방향설정 오류");
                break;
        }

        Vector2 destination = rg2d.position + (dir * distance);
        print("movingObject.cs : transform : " + rg2d.position + " destination : " + destination);
        StartCoroutine(MoveToDestination(rg2d, destination, speed, accelation));
        print("movingObject.cs : 목적지");
    }
    private void Start()
    {
        rg2d = GetComponent<Rigidbody2D>();
    }
}
