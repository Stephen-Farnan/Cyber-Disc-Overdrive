using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyShaderParameterController : MonoBehaviour {

	public float value;
	public float speed = 1;
	private bool canPress = true;
	private bool isZero = false;
	private Material mat;

	void Awake() {
		mat = GetComponent<MeshRenderer>().material;
	}
	
	void Update () {
		if (Input.GetKeyDown(KeyCode.M))
		{
			if (canPress)
			{
				StartCoroutine(SetNewValue());
			}
		}
	}

	IEnumerator SetNewValue() {
		canPress = false;
		if (isZero)
		{
			Debug.Log("Going to 1");
			while (value < 1)
			{
				value += Time.deltaTime * speed;
				mat.SetFloat("Vector1_FBE0D744", value);
				yield return null;
			}
			isZero = false;
		}
		else
		{
			Debug.Log("Going to 0");
			while (value > 0)
			{
				value -= Time.deltaTime * speed;
				mat.SetFloat("Vector1_FBE0D744", value);
				yield return null;
			}
			isZero = true;
		}
		canPress = true;
	}
}
