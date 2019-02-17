using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderPositionUpdate : MonoBehaviour {

	public Material[] mapMask;
	private static bool revealerOn = true;

	void Update () {
		if (revealerOn)
		{
			foreach (Material mat in mapMask)
			{
				mat.SetVector("_playerPosition", transform.position);
			}
		}
	}

	public static void SetRevealer(bool state) {
		revealerOn = state;
	}
}
