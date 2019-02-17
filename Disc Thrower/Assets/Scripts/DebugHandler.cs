using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugHandler : MonoBehaviour {

	[Tooltip("Use this to override all debug options in the game")]
	public bool debugOn = false;
	public static bool debugEnabled = false;

	[Tooltip("Turn all room masks on/off at level generation")]
	public bool roomMasksOn = true;
	public static bool roomMasksEnabled = true;

	private void Awake()
	{
		debugEnabled = debugOn;
		roomMasksEnabled = roomMasksOn;
	}
}
