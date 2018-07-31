using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpingMonster : MonoBehaviour {
    Rigidbody2D rigidbody;
    bool isJumping;
    public int jumpPower;
    public float deley;
    private void Start()
    {
        rigidbody = gameObject.GetComponent<Rigidbody2D>();
        isJumping = false;
        StartCoroutine(MonsterJump());
    }
    private void FixedUpdate()
    {
        Jump();
    }

    public void Jump() {
        if (!isJumping)
            return;
        rigidbody.velocity = Vector2.zero;
        rigidbody.AddForce(Vector2.up *jumpPower,ForceMode2D.Impulse);
        isJumping = false;
    }
    IEnumerator MonsterJump()
    {
       
        isJumping = true;
      

        yield return new WaitForSeconds(deley);
        StartCoroutine(MonsterJump());
    }
}
