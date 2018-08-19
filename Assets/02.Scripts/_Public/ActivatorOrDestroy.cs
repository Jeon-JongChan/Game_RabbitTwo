using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum SelectType
{
    TRIGGER,
    ACTIVE,
    DISABLE,
    DESTROY,
    REPLACE,
    ENABLE,
    BLANK
}
public class ActivatorOrDestroy : MonoBehaviour {
    [SerializeField] bool isEnable = false;
    [SerializeField] SelectType type = SelectType.DESTROY;
    [SerializeField] float lifeTime = 5f;
    [SerializeField] float blankTime = 3f;
    [SerializeField] Object target;                       // The game object to affect. If none, the trigger work on this game object
    [SerializeField] GameObject replaceSource;
    private void OnEnable()
    {
        if (isEnable) StartAOD();
    }
    private void OnDisable()
    {
        if (isEnable) StopAOD();
    }
    IEnumerator StartActivatorOrDestroy()
    {
        yield return new WaitForSeconds(lifeTime);

        target = target ?? gameObject;
        MonoBehaviour targetBehaviour = target as MonoBehaviour;
        GameObject targetGameObject = target as GameObject;
        if (targetBehaviour != null)
        {
            targetGameObject = targetBehaviour.gameObject;
        }
        else
        {
            targetGameObject = gameObject;
        }

        switch(type)
        {
            case SelectType.TRIGGER:
                if (targetGameObject != null)
                {
                    targetGameObject.SendMessage("StartAOD");
                }
                break;
            case SelectType.DESTROY:
                Destroy(targetGameObject);
                break;
            case SelectType.REPLACE:
                if (replaceSource != null)
                {
                    if (targetGameObject != null)
                    {
                        Instantiate(replaceSource, targetGameObject.transform.position,
                                    targetGameObject.transform.rotation);
                        Destroy(targetGameObject);
                    }
                }
                break;
            case SelectType.DISABLE:
                targetGameObject.SetActive(false);
                break;
            case SelectType.ACTIVE:
                targetGameObject.SetActive(true);
                break;
            case SelectType.ENABLE:
                if (targetBehaviour != null)
                {
                    targetBehaviour.enabled = true;
                }
                break;
            case SelectType.BLANK:
                targetGameObject.SetActive(false);
                yield return new WaitForSeconds(blankTime);
                targetGameObject.SetActive(true);
                break;
        }
    }
    public void StartAOD()
    {
        StartCoroutine("StartActivatorOrDestroy");
    }
    public void StopAOD()
    {
        StopCoroutine("StartActivatorOrDestroy");
    }
}
