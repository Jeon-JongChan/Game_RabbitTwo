using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HingeJoint2D))]
public class HingeCtrl2D : ObjectInteraction {

    [SerializeField] float delayOperateTime = 0;
    [SerializeField] bool useTrigger = false;
    HingeJoint2D hj2d;
    WaitForSeconds wsDelayTrigger;

    private void Start() {
        hj2d = GetComponent<HingeJoint2D>();
        wsDelayTrigger = new WaitForSeconds(0.2f);
    }

    private void OnBecameVisible() {
        StopCoroutine("StartHinge");
        StartCoroutine("StartHinge");
    }

    IEnumerator StartHinge()
    {
        while(CollisionTargetTransform == null && useTrigger) yield return wsDelayTrigger;
        if(delayOperateTime > 0)yield return new WaitForSeconds(delayOperateTime);
        if(hj2d != null) hj2d.useMotor = true;

    }

}
