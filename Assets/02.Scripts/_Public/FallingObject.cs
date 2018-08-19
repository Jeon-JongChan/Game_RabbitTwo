using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FallingObject : MonoBehaviour {

    /* inspector variable */
    [Range(3,10)]
    [Tooltip("물체가 움직일때 플레이어를 죽이지 않을 속도제한입니다.")]
    public float limitVelocity = 3f;

    /* needs component */
    Rigidbody2D rg2d;

    /* need variable */
    WaitForSeconds wsDelay;
    Vector2 saveVelocity = Vector2.zero;
    float delay = 0.5f;
    bool state = true;

    // Use this for initialization
    void Start () {
        wsDelay = new WaitForSeconds(delay);
        rg2d = GetComponent<Rigidbody2D>();
        rg2d.gravityScale = 0;
    }
    private void OnBecameVisible() {
        StartCoroutine("SaveVelocity");
    }
    private void OnBecameInvisible() {
        StopCoroutine("SaveVelocity");
    }
    ///<summary>
    ///속도를 지속적으로 저장하는 코루틴. 이 속도를 통해 플레이어에 영향을 준다.
    ///</summary>
    IEnumerator SaveVelocity()
    {
        while(state)
        {
            saveVelocity = rg2d.velocity;
            yield return wsDelay;
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
