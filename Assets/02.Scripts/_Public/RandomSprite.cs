using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[ExecuteInEditMode]
public class RandomSprite : MonoBehaviour {
	[SerializeField] Sprite[] sprites;
	[SerializeField] bool useParticleRandom = false;
	[SerializeField] Material[] materials;
	ParticleSystemRenderer ps = null;
	SpriteRenderer sr = null;
	int randomIdx = 0;
	
	private void Start() {
		sr = GetComponent<SpriteRenderer>();
		if(useParticleRandom) ps = GetComponent<ParticleSystemRenderer>();
	}
	void OnEnable() {
		if(sprites.Length > 0 && sr != null)
		{
			randomIdx = Random.Range(0,sprites.Length);
			//print(sprites[randomIdx]+ " " + sr.sprite);
			sr.sprite = sprites[randomIdx];
		}
		if(materials.Length > 0 && ps != null)
		{
			ps.material = materials[randomIdx];
		}
	}
}

