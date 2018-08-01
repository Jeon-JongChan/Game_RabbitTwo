using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
enum CollisionType
{
	DAMAGE,
	DISABLE,
	DISAPPEAR
}
public class CollisionEvent : MonoBehaviour {
	[SerializeField] CollisionType type = CollisionType.DAMAGE;
	[SerializeField] string[] targetTag;
	[SerializeField] int damage = 1;
	[SerializeField] float disappearTime = 0;
	
	WaitForSeconds wsDisapperTimeDelay;
	IDamageable I_DamageScript;
	bool collisionState = false;
	private void Start() 
	{
		wsDisapperTimeDelay = new WaitForSeconds(disappearTime);
	}
	IEnumerator StartCollisionEvent(GameObject col)
	{
		collisionState = true;
		if(disappearTime != 0) yield return wsDisapperTimeDelay;

		switch(type)
		{
			case CollisionType.DAMAGE:
			I_DamageScript = col.GetComponent<IDamageable>();
			I_DamageScript.TakeHit(damage);
			break;
			case CollisionType.DISABLE:
			StopAllCoroutines();
			gameObject.SetActive(false);
			break;
			case CollisionType.DISAPPEAR:
			StopAllCoroutines();
			Destroy(gameObject);
			break;
		}
	}

	private void OnCollisionEnter2D(Collision2D other) {
		if(targetTag.Length < 1) StartCoroutine(StartCollisionEvent(other.gameObject));
		else{
			foreach(var v in targetTag)
			{
				if(other.gameObject.CompareTag(v)) StartCoroutine(StartCollisionEvent(other.gameObject));
			}
		}
	}
	private void OnTriggerEnter2D(Collider2D other) {
		if(targetTag.Length < 1) StartCoroutine(StartCollisionEvent(other.gameObject));
		else{
			foreach(var v in targetTag)
			{
				if(other.CompareTag(v)) StartCoroutine(StartCollisionEvent(other.gameObject));
			}
		}
	}
}
