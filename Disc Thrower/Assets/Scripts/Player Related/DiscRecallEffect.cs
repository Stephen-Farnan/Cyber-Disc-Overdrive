using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscRecallEffect : MonoBehaviour {

	[Tooltip("The time it takes for the entire effect to play out")]
	public float time = 0.8f;
	public ParticleSystem recallParticleSystem;
	private float t = 0;
	private Vector3 startPos;
	private Transform playerTransform;

	private void Start() {
		startPos = transform.position;
		recallParticleSystem.Play();
	}

	public void SetPlayerTransform (Transform playTrans) {
		playerTransform = playTrans;
	}
	
	void Update () {
		if (t < 1){
			t += Time.deltaTime / time;
			transform.position = Vector3.Lerp(startPos, playerTransform.position, t);
		}
		else if (recallParticleSystem.isPlaying) {
			recallParticleSystem.Stop();
		}
	}
}
