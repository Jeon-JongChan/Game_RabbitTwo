
using UnityEngine;

public class BreakableWall : MonoBehaviour {
    ParticleSystem particle;
    SpriteRenderer sprite;
    BoxCollider2D boxCollider;
    private void Start()
    {
        particle = gameObject.GetComponent<ParticleSystem>();
        sprite = gameObject.GetComponent<SpriteRenderer>();
        boxCollider = gameObject.GetComponent<BoxCollider2D>();

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        particle.Play();
        sprite.enabled = false;
        boxCollider.enabled = false;
        
    }
}
