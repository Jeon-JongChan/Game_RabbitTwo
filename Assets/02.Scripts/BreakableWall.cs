using UnityEngine;

public class BreakableWall : MonoBehaviour {
    [SerializeField] string[] targetTag;
    [Tooltip("벽돌이 사라지고 다시 재생성해야 할 경우 체크")]
    [SerializeField] bool isReActive = false;
    [Tooltip("벽돌이 다시 사라지고 다시 나타나는데 걸리는 시간")]
    float reActiveTime = 3f; 
    ParticleSystem particle;
    SpriteRenderer sprite = null;
    BoxCollider2D boxCollider;
    bool particleSetActive = true;
    private void Start()
    {
        particle = gameObject.GetComponent<ParticleSystem>();
        sprite = gameObject.GetComponent<SpriteRenderer>();
        boxCollider = gameObject.GetComponent<BoxCollider2D>();
    }
    void Enable() {
        if(isReActive && sprite != null)
        {
            sprite.enabled = true;
            boxCollider.enabled = true;
            particleSetActive = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //print("충돌!");
        if (!particleSetActive)
        {
            return;
        }
        if (targetTag.Length > 0)
        {

            foreach (var v in targetTag)
            {
                print(v);
                if (collision.gameObject.CompareTag(v))
                {
                    print("충돌");
                    particle.Play();
                    sprite.enabled = false;
                    boxCollider.enabled = false;
                    particleSetActive = false;
                    if (isReActive)
                    {
                        Invoke("Enable", reActiveTime);
                    }
                }
            }
        }
    }
}
