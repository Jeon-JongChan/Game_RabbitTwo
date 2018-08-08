using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
enum CollisionType
{
	DAMAGE,
	DAMAGE_AND_DISABLE,
	DISABLE,
	DESTROY
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
	void StartCollisionEvent(GameObject col)
	{
		collisionState = true;

		switch(type)
		{
			case CollisionType.DAMAGE:
				I_DamageScript = col.GetComponent<IDamageable>();
				I_DamageScript.TakeHit(damage);
				break;
			case CollisionType.DAMAGE_AND_DISABLE:
				I_DamageScript = col.GetComponent<IDamageable>();
				I_DamageScript.TakeHit(damage);
				//StopAllCoroutines();
				if(disappearTime == 0) gameObject.SetActive(false);
				else StartCoroutine("DelayDisable");
				break;
			case CollisionType.DISABLE:
				//StopAllCoroutines();
				if(disappearTime == 0) gameObject.SetActive(false);
				else StartCoroutine("DelayDisable");
				break;
			case CollisionType.DESTROY:
				//StopAllCoroutines();
				Destroy(gameObject,disappearTime);
			break;
		}
	}
	IEnumerator DelayDisable()
	{
		yield return wsDisapperTimeDelay;
		gameObject.SetActive(false);
	}
	private void OnCollisionEnter2D(Collision2D other) {
		if(targetTag.Length < 1) StartCollisionEvent(other.gameObject);
		else{
			foreach(var v in targetTag)
			{
				if(other.gameObject.CompareTag(v)) StartCollisionEvent(other.gameObject);
			}
		}
	}
	private void OnTriggerEnter2D(Collider2D other) {
		if(targetTag.Length < 1) StartCollisionEvent(other.gameObject);
		else{
			foreach(var v in targetTag)
			{
				if(other.CompareTag(v)) StartCollisionEvent(other.gameObject);
			}
		}
	}
}
