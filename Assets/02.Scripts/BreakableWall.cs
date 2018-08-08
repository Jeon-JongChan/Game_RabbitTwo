
using UnityEngine;

public class BreakableWall : MonoBehaviour {
    [SerializeField] string[] targetTag;
    [Tooltip("벽돌이 사라지고 다시 재생성해야 할 경우 체크")]
    [SerializeField] bool isReActive = false;
    ParticleSystem particle;
    SpriteRenderer sprite = null;
    BoxCollider2D boxCollider;
    private void Start()
    {
        particle = gameObject.GetComponent<ParticleSystem>();
        sprite = gameObject.GetComponent<SpriteRenderer>();
        boxCollider = gameObject.GetComponent<BoxCollider2D>();

    }
    void OnEnable() {
        if(isReActive && sprite != null)
        {
            sprite.enabled = true;
            boxCollider.enabled = true;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(targetTag.Length > 0)
        {
            foreach(var v in targetTag)
            {
                if(collision.gameObject.CompareTag(v))
                {
                    particle.Play();
                    sprite.enabled = false;
                    boxCollider.enabled = false;
                }
            }
        }
        else{
            particle.Play();
            sprite.enabled = false;
            boxCollider.enabled = false;
        }
        
    }
}
