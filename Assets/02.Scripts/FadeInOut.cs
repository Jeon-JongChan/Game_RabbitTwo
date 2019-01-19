using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInOut : ObjectInteraction
{
    /* needs components */
    SpriteRenderer srComponets = null;
    Collider2D col = null;

    /* needs variable */
    bool SpriteRendererTrigger = true;
    bool ColliderTrigger = true;
    bool startStatus = false; //코루틴이 반복 호출되는 것을 막아줍니다.
    float detectRange = 0.5f;
    float transparency = 1f;
    float speedOfFade = 0.01f;
    Color color;
    [Tooltip("collider가 존재하는 시간")]
    public float liveTime = 1f;
    [Tooltip("collider가 존재하지 않는 시간")]
    public float hideTime = 1f;
    [Tooltip("화면에 보일경우 첫 시작을 delay 시킬수 있는 인수입니다. default = 0")]
    public float delayTime = 0;
    /* 세이브 변수 */
    bool colActive;
    bool srActive;

    public override void SaveState(bool selfState, bool selfActive, Vector2 pos, bool init = false)
    {
        if (!col.enabled && col != null) colActive = col.enabled;
        if (!srComponets.enabled) srActive = srComponets.enabled;
        base.SaveState(selfState, selfActive, pos);
    }

    public override bool LoadState(bool init = false)
    {
        startStatus = initState;
        if (!srComponets.enabled) srComponets.enabled = srActive;
        if (!col.enabled && col != null) col.enabled = colActive;
        return base.LoadState();
    }

    /* 주요 기능*/

    IEnumerator FadeIn(SpriteRenderer srComponets, Color color,Collider2D col, float speedOfFade) {
        while (srComponets.color.a < 1) {
            color.a += speedOfFade;
            srComponets.color = color;
            yield return new WaitForSeconds(speedOfFade);
        }
        col.enabled = true;
        yield return new WaitForSeconds(liveTime);
        StartCoroutine(FadeOut(srComponets, color, col, speedOfFade));
        
    }

    IEnumerator FadeOut(SpriteRenderer srCompoents, Color color, Collider2D col, float speedOfFade) {
        yield return new WaitForSeconds(delayTime);
        while (srComponets.color.a > 0)
        {
            color.a -= speedOfFade;
            srComponets.color = color;
            yield return new WaitForSeconds(speedOfFade);
        }
        col.enabled = false;
        yield return new WaitForSeconds(hideTime);
        StartCoroutine(FadeIn(srComponets, color, col, speedOfFade));
    }



    // Use this for initialization
    void Start () {
        srComponets = GetComponent<SpriteRenderer>();
        col = GetComponent<BoxCollider2D>();
        SaveState(false, gameObject.activeSelf, transform.position);
        color = new Color(1,1,1,transparency);
    }

    private void OnBecameVisible()
    {
        StartCoroutine(FadeOut(srComponets, color, col, speedOfFade));
    }
    private void OnBecameInvisible()
    {
        StopAllCoroutines();
    }
}
