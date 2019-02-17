using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationHandler : MonoBehaviour {

	public Animator anim;
	public CharacterNavMeshMovement charMovement;

	public void UpdateAnimationParameters(Vector2 input) {
		Vector3 inputDir = new Vector3 (input.x, 0, input.y);
		Vector3 lookDir = transform.forward;

		float angle = Vector3.Angle(lookDir, Vector3.forward);
		Vector3 cross = Vector3.Cross(lookDir, Vector3.forward);
		if (cross.y < 0) angle = -angle;

		Quaternion lookRot = Quaternion.AngleAxis(angle, Vector3.up);
		Vector3 inputRot = lookRot * inputDir;

		anim.SetFloat("h", Mathf.Clamp(inputRot.x, -1, 1));
		anim.SetFloat("v", Mathf.Clamp(inputRot.z, -1, 1));
	}

	public float CheckParameters() {
		float sqr = Vector2.SqrMagnitude(new Vector2(anim.GetFloat("h"), anim.GetFloat("v")));
		return sqr;
	}

	public void ZeroOutParameters() {
		anim.SetFloat("h", 0);
		anim.SetFloat("v", 0);
	}
}
