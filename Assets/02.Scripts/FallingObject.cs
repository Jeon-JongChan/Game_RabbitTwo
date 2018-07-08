using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FallingObject : MonoBehaviour {

    /* inspector variable */
    [Range(3,10)]
    public float limitVelocity = 3f;

    /* needs component */
    Rigidbody2D rg2d;

    /* need variable */
    Vector2 saveVelocity = Vector2.zero;
    float delay = 0.5f;
    bool state = true;

    // Use this for initialization
    void Start () {
        rg2d = GetComponent<Rigidbody2D>();
        StartCoroutine(SaveVelocity());
    }
    IEnumerator SaveVelocity()
    {
        while(state)
        {
            saveVelocity = rg2d.velocity;
            yield return new WaitForSeconds(delay);
        }
    }
    bool CheckMovement()
    {
        if (Mathf.Abs(saveVelocity.x) > limitVelocity || saveVelocity.y  < -limitVelocity) return true;
        return false;
    }
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            if(CheckMovement())
            {
                LifeInteraction interaction = col.gameObject.GetComponent<LifeInteraction>();
                print("FallingObject.cs - 충돌 발생 ");
                if (interaction != null) interaction.TakeHit(1);
            }
        }
    }
}
