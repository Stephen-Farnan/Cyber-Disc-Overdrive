using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour {

	//The room prefab that can be spawned here
	[Header("Prefabs")]
	public GameObject roomPrefab;
	public GameObject roomPrefabLarge;
	//Short straight corridor prefab
	public GameObject corridorStraightShort;
	//Long straight corridor prefab
	public GameObject corridorStraightLong;
	//90 degree angle corridor
	public GameObject corridorCorner;
	//Same as above but with short bottom-right
	public GameObject corridorDLShort;
	//Same as above but with short bottom-left
	public GameObject corridorDRShort;
	//Doorway prefab
	public GameObject doorPrefab;
	//Boss door prefab
	public GameObject bossDoorPrefab;
	//Chest Prefab
	public GameObject chestPrefab;
	//Health Regen Prefab
	public GameObject healthRegenPrefab;
	//An array to tell which wall segments are doors
	private bool[] doors;
	//An array to hold corridors connected to this room (index = position)
	private GameObject[] corridors;
	//To keep track of how many doors have been placed in the room
	private int noOfDoors = 0;
	//Which of this room's adjacent grid points are rooms
	private bool[] adjacentRoomBools;
	//The connections this room has to other rooms
	private bool[] hasConnection;

	public int noOfSmallLayouts;
	public int noOfLargeLayouts;
	[Tooltip("For testing only")]
	public int roomLayout = -1;

	private int distanceFromStart = 0;

	//Positions in which it is possible to generate a room
	[HideInInspector]
	public bool[] validRGPositions;

	//private LevelGenerator levGen;
	private RoomScript roomScript;
	private LevelGenerator levGen;

	void Awake() {
		adjacentRoomBools = new bool[9];
		hasConnection = new bool[9];
		doors = new bool[12];
		corridors = new GameObject[12];
		validRGPositions = new bool[9];
		for (int i = 0; i < validRGPositions.Length; i++)
		{
			validRGPositions[i] = true;
		}
		levGen = transform.parent.GetComponent<LevelGenerator>();
	}

	public void GenerateStartRoom() {
		//Instantiate room prefab
		GameObject room = Instantiate(roomPrefab, transform.position, transform.rotation, transform);
		roomScript = room.GetComponent<RoomScript>();
		StartCoroutine(roomScript.RemoveRoomMask());
	}

	public GameObject GenerateRoom() {
		//Instantiate room prefab
		GameObject room = Instantiate(roomPrefab, transform.position, transform.rotation, transform);
		roomScript = room.GetComponent<RoomScript>();

		return room;
	}

	public GameObject GenerateLargeRoom()
	{
		//Instantiate room prefab
		GameObject room = Instantiate(roomPrefabLarge, transform.position, transform.rotation, transform);
		room.transform.SetAsFirstSibling();
		roomScript = room.GetComponent<RoomScript>();

		return room;
	}

	public void ResetRoomScript() {
		roomScript = null;
	}

	public int GetDistanceFromStart() {
		return distanceFromStart;
	}
	public void SetDistanceFromStart(int distance) {
		distanceFromStart = distance;
	}

	public void ChooseSmallRoomLayout(int sourceRoomPosition) {
		//Choose layout
		roomLayout = Random.Range(0, noOfSmallLayouts);
		bool reset = false;

		//Setting what positions will remain valid after layout selection
		if (roomLayout == 0)
		{
			//In default layout, all odds will be same and all evens too. But challenge variants will be affected differently
			int randRot = Random.Range(0, 8);
			transform.GetChild(0).GetChild(roomLayout).Rotate(0, 45 * randRot, 0);
		}

		if (roomLayout == 1)
		{
			for (int i = 0; i < validRGPositions.Length; i++)
			{
				if (i == 1 || i == 3 || i == 5 || i == 7)
				{
					validRGPositions[i] = true;
				}
				else {
					validRGPositions[i] = false;
					//If source room position is set to invalid, reset
					if (i == sourceRoomPosition)
					{
						reset = true;
						break;
					}
				}
			}

			int randRot = Random.Range(0, 4);
			transform.GetChild(0).GetChild(roomLayout).Rotate(0, 90 * randRot, 0);
		}

		if (roomLayout == 2)
		{
			if (Roll(50))
			{
				transform.GetChild(0).GetChild(roomLayout).Rotate(0, 90, 0);

				validRGPositions[0] = false;
				validRGPositions[8] = false;
			}
			else
			{
				validRGPositions[2] = false;
				validRGPositions[6] = false;
			}

			if (!validRGPositions[sourceRoomPosition])
			{
				reset = true;
			}
		}

		if (roomLayout == 3)
		{
			int randRot = Random.Range(0, 4);
			if (randRot == 1)
			{
				validRGPositions[8] = false;
			}
			else if (randRot == 2)
			{
				validRGPositions[6] = false;
			}
			else if (randRot == 3)
			{
				validRGPositions[0] = false;
			}
			else if (randRot == 0)
			{
				validRGPositions[2] = false;
			}

			if (!validRGPositions[sourceRoomPosition])
			{
				reset = true;
			}
			else
			{
				transform.GetChild(0).GetChild(roomLayout).Rotate(0, 90 * randRot, 0);
			}
		}

		if (roomLayout == 4)
		{
			int randRot = Random.Range(0, 8);
			transform.GetChild(0).GetChild(roomLayout).Rotate(0, 45 * randRot, 0);
		}

		//Layout is incompatible with source room position, try again
		if (reset)
		{
			for (int i = 0; i < validRGPositions.Length; i++)
			{
				validRGPositions[i] = true;
			}

			roomLayout = -1;

			reset = false;

			ChooseSmallRoomLayout(sourceRoomPosition);
			return;
		}

		//Activate the selected layout and store it in RoomScript
		Transform layoutTransform = transform.GetChild(0).GetChild(roomLayout);
		layoutTransform.gameObject.SetActive(true);
		//Set up enemy spawning in room
		if (roomScript != null)
		{
			roomScript.SetEnemySpawnPoints(layoutTransform);
			StartCoroutine (roomScript.SelectEnemies());
		}
		else {
			Debug.LogError("Room Script of" + gameObject.name + "is null");
		}
	}

	public void ChooseLargeRoomLayout() {
		//Choose layout
		roomLayout = Random.Range(0, noOfLargeLayouts);
		int randRot = 0;

		switch (roomLayout)
		{
			case 0:
				randRot = Random.Range(0, 2);
				transform.GetChild(0).GetChild(roomLayout).Rotate(0, 45 * randRot, 0);
				break;

			case 3:
				randRot = Random.Range(0, 4);
				transform.GetChild(0).GetChild(roomLayout).Rotate(0, 45 * randRot, 0);
				break;

			default:
				break;
		}

		//Activate the selected layout and store it in RoomScript
		Transform layoutTransform = transform.GetChild(0).GetChild(roomLayout);
		layoutTransform.gameObject.SetActive(true);

		roomScript.SetEnemySpawnPoints(layoutTransform);
		StartCoroutine(roomScript.SelectEnemies());
	}

	public int GetLayoutNumber() {
		return roomLayout;
	}

	public GameObject PlaceDoorway(int doorPosition)
	{
		GameObject door = Instantiate(doorPrefab, roomScript.walls[doorPosition].transform.position, roomScript.walls[doorPosition].transform.rotation, roomScript.walls[doorPosition].transform);
		roomScript.walls[doorPosition].GetComponent<MeshRenderer>().enabled = false;
		roomScript.walls[doorPosition].GetComponent<MeshCollider>().enabled = false;
		doors[doorPosition] = true;
		noOfDoors++;

		return door;
	}

	public bool CheckForDoor(int position) {
		return doors[position];
	}

	public GameObject PlaceBossDoor(int doorPosition)
	{
		GameObject door = Instantiate(bossDoorPrefab, roomScript.walls[doorPosition].transform.position, roomScript.walls[doorPosition].transform.rotation, roomScript.walls[doorPosition].transform);
		roomScript.walls[doorPosition].GetComponent<MeshRenderer>().enabled = false;
		roomScript.walls[doorPosition].GetComponent<MeshCollider>().enabled = false;
		doors[doorPosition] = true;
		noOfDoors++;

		return door;
	}

	public void SpawnChest(bool isBossKeyChest) {
		//Chest Position GO MUST be last child of layout GO!!!
		Transform spawnTransform = transform.GetChild(0).GetChild(roomLayout).GetChild(transform.GetChild(0).GetChild(roomLayout).childCount - 1);
		GameObject chest = Instantiate(chestPrefab, spawnTransform.position, Quaternion.Euler(Vector3.zero), spawnTransform);
		if (isBossKeyChest)
		{
			chest.GetComponent<ChestScript>().SetAsBossKey();
		}
		Debug.Log("Spawn Chest");
	}

	public void SpawnHealthRegen() {
		Transform spawnTransform = transform.GetChild(0).GetChild(roomLayout).GetChild(transform.GetChild(0).GetChild(roomLayout).childCount - 1);
		Instantiate(healthRegenPrefab, spawnTransform.position, Quaternion.Euler(Vector3.zero), spawnTransform);
	}

	/// <summary>
	/// 0: Boss Door Room, 1: Key Room, 2: Chest Room, 3: Challenge Room
	/// </summary>
	/// <param name="specialType"></param>
	public void SetSpecialRoom(int specialType) {
		roomScript.SetAsSpecialRoom(specialType);

		if (specialType == 0)
		{
			transform.GetChild(0).GetChild(roomLayout).gameObject.SetActive(false);
		}
	}
	public void SetAsStartRoom()
	{
		roomScript.SetRoomCleared();
	}
	public void SetAsLargeRoom() {
		roomScript.SetAsLargeRoom();
	}

	public void SetAdjacentRoomBool (int i, bool roomBool) {
		adjacentRoomBools[i] = roomBool;
	}
	//Clears the bools that track which adjacent positions have rooms
	public void ClearAdjacentRoomBools() {
		for (int i = 0; i < adjacentRoomBools.Length; i++)
		{
			adjacentRoomBools[i] = false;
		}
	}
	//Checks if there is a room in a specific direction
	public bool CheckForRoom(int i)
	{
		return adjacentRoomBools[i];
	}

	public void SetRoomConnections(int i, bool connectionBool)
	{
		hasConnection[i] = connectionBool;
	}
	public void ClearRoomConnections()
	{
		for (int i = 0; i < hasConnection.Length; i++)
		{
			hasConnection[i] = false;
		}

		for (int i = 0; i < doors.Length; i++)
		{
			doors[i] = false;
		}
	}
	//Is there a corridor in position i?
	public bool CheckForCorridor(int i)
	{
		return hasConnection[i];
	}

	public void ClearDoorways() {
		for (int i = 0; i < roomScript.walls.Length; i++)
		{
			roomScript.walls[i].GetComponent<MeshRenderer>().enabled = true;
			roomScript.walls[i].GetComponent<MeshCollider>().enabled = true;
		}
	}

	public void SetCorridors(GameObject corridor, int position)
	{
		corridors[position] = corridor;
	}

	public void ReplaceCorridorForLargeRoom(int position)
	{
		GameObject newCorridor = null;
		if (position == 3 || position == 9)
		{
			newCorridor = Instantiate(corridorDLShort, corridors[position].transform.position, corridors[position].transform.rotation, transform);
			Destroy(corridors[position]);
		}
		if (position == 5 || position == 11)
		{
			newCorridor = Instantiate(corridorDRShort, corridors[position].transform.position, corridors[position].transform.rotation, transform);
			Destroy(corridors[position]);
		}
		if (newCorridor != null)
		{
			corridors[position] = newCorridor;
		}
	}

	//How many corridors is this room connected to?
	public int NoOfCorridors() {
		int x = 0;
		for (int i = 0; i < adjacentRoomBools.Length; i++)
		{
			if (hasConnection[i])
			{
				x++;
			}
		}
		return x;
	}

	public void SetDoorways() {
		roomScript.SetRoomScriptRefOnDoors();
	}

	public bool Roll(float chance)
	{
		int randNum = Random.Range(1, 101);
		if (randNum <= chance)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
}
