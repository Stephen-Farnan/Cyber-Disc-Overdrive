using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LevelGenerator : MonoBehaviour
{
    #region properites
    [Tooltip("Size of level grid (x, y) - Acts as a border to contain the level")]
    public Vector2Int levelGridSize;
    [Tooltip("The empty GameObject prefab with the room script attached to it")]
    public GameObject emptyRoomPrefab;
    public GameObject bossRoomPrefab;
    private Transform bossRoomSpawnPoint;
    [Tooltip("The number by which the total number of rooms can vary")]
    public int roomVariance = 0;
    //Base percentage chance of creating a corridor between adjacent rooms
    [Range(1, 100)]
    public float chanceToCreateRoom = 50;
    [Range(1, 100)]
    public float chanceToCreateCorridor = 75;
    //The gap between each grid point
    private float spaceBetweenRooms = 50;
    //Storing all grid points in an array
    private GameObject[] gridPoints;
    //The array that will track whether or not grid points contain a room
    private bool[] roomBool;
    //An array that tracks whether or not a corridor passe through a grid point
    private bool[] corridorBool;
    //Storing any created corridors in case they need to be cleared
    List<GameObject> corridorList = new List<GameObject>();
    //Storing any created rooms in case they need to be cleared
    List<GameObject> roomList = new List<GameObject>();
    //Storing any created doors in case they need to be cleared
    List<GameObject> doorList = new List<GameObject>();
    //Storing large rooms for reference
    List<int> largeRoomPositions = new List<int>();
    //Storing special rooms for checking
    List<int> specialRooms = new List<int>();

    //Radial Method Variables
    List<int> activeRooms = new List<int>();
    List<int> existingRooms = new List<int>();

    //Keeps track of number of rooms generated so far
    [HideInInspector]
    public int totalNoOfRoomsGenerated = 0;
    //The number of rooms to be generated in the level
    public int totalNoOfRooms = 13;
    //The maximum number of large rooms that can be generated in a level
    public int maxNoOfLargeRooms = 1;
    //Are we ensuring that special rooms appear as far away as possible, or randomized?
    public bool specialRoomsFurthestMethod = true;

    private int startRoom;

    //private bool loading = false;

    #endregion

    void Start()
    {
        //BeginProcess();
    }

    /*public bool GetLoadingState() {
		return loading;
	}*/

    public void BeginProcess()
    {
        GenerateGridPrefabs();
        GenerateStartRoom();

        StartCoroutine(GenerateLevel());
    }

    //This coroutine handles the procedural generation process from start to finish
    IEnumerator GenerateLevel()
    {
        //Store the value of the base variable
        float oGChance = chanceToCreateRoom;
        //The number of times we've done a check
        int passes = 0;
        //Do checks until we have all rooms
        while (totalNoOfRoomsGenerated < totalNoOfRooms)
        {
            RadialCheck();
            passes++;
            if (passes > 100)
            {
                chanceToCreateRoom += 10;
                passes = 0;
            }
            yield return null;
        }
        //Reset variable to original
        chanceToCreateRoom = oGChance;
        //loading = true;

        yield return null;

        SetLargeRooms();

        yield return null;
        //yield return new WaitForSeconds(2);

        SpawnBossRoom();
        SetSpecialRooms();
        //loading = false;
        GenerateNavMesh();
        PlacePlayer();
        SetDoorways();
    }

    void GenerateStartRoom()
    {
        gridPoints[startRoom].GetComponent<RoomGenerator>().GenerateStartRoom();
        roomBool[startRoom] = true;
        totalNoOfRoomsGenerated++;

        activeRooms.Add(startRoom);
    }

    void RadialCheck()
    {
        List<int> toBeRemoved = new List<int>();
        List<int> toBeAdded = new List<int>();

        foreach (int roomNumber in activeRooms)
        {
            int noOfRoomsGenerated = 0;
            for (int i = 0; i < 9; i++)
            {
                //If we have reached desired number of rooms, end
                if (totalNoOfRoomsGenerated >= totalNoOfRooms)
                {
                    break;
                }
                RoomGenerator roomGen = gridPoints[roomNumber].GetComponent<RoomGenerator>();

                bool canGenerateRoom = true;

                //The address of the other room in the gridPoints array
                int otherRoom = -1;
                //The position of this room relative to the newly generated room
                int j = -1;

                //If the room generator script says this position isn't valid, end
                if (roomGen.validRGPositions[i])
                {
                    //Setting otherRoom and j variables depending on room position
                    #region
                    switch (i)
                    {
                        case 0:
                            //Making sure a room doesn't get created diagonally if 2 already exist either side of that position
                            if (roomBool[roomNumber - 1] && roomBool[roomNumber - levelGridSize.x])
                            {
                                canGenerateRoom = false;
                            }

                            if (roomNumber - levelGridSize.x - 1 > 0 && roomNumber % levelGridSize.x != 0)
                            {
                                otherRoom = roomNumber - levelGridSize.x - 1;

                                j = 8;
                            }
                            break;

                        case 1:
                            if (roomNumber - levelGridSize.x > 0)
                            {
                                otherRoom = roomNumber - levelGridSize.x;

                                j = 7;
                            }
                            break;

                        case 2:
                            //Making sure a room doesn't get created diagonally if 2 already exist either side of that position
                            if (roomBool[roomNumber + 1] && roomBool[roomNumber - levelGridSize.x])
                            {
                                canGenerateRoom = false;
                            }

                            if (roomNumber - levelGridSize.x + 1 > 0 && roomNumber % levelGridSize.x != levelGridSize.x - 1)
                            {
                                otherRoom = roomNumber - levelGridSize.x + 1;

                                j = 6;
                            }
                            break;

                        case 3:
                            if (roomNumber - 1 > 0 && roomNumber % levelGridSize.x != 0)
                            {
                                otherRoom = roomNumber - 1;

                                j = 5;
                            }
                            break;

                        case 5:
                            if (roomNumber + 1 < gridPoints.Length && roomNumber % levelGridSize.x != levelGridSize.x - 1)
                            {
                                otherRoom = roomNumber + 1;

                                j = 3;
                            }
                            break;

                        case 6:
                            //Making sure a room doesn't get created diagonally if 2 already exist either side of that position
                            if (roomBool[roomNumber - 1] && roomBool[roomNumber + levelGridSize.x])
                            {
                                canGenerateRoom = false;
                            }

                            if (roomNumber + levelGridSize.x - 1 < gridPoints.Length && roomNumber % levelGridSize.x != 0)
                            {
                                otherRoom = roomNumber + levelGridSize.x - 1;

                                j = 2;
                            }
                            break;

                        case 7:
                            if (roomNumber + levelGridSize.x < gridPoints.Length)
                            {
                                otherRoom = roomNumber + levelGridSize.x;

                                j = 1;
                            }
                            break;

                        case 8:
                            //Making sure a room doesn't get created diagonally if 2 already exist either side of that position
                            if (roomBool[roomNumber + 1] && roomBool[roomNumber + levelGridSize.x])
                            {
                                canGenerateRoom = false;
                            }

                            if (roomNumber + levelGridSize.x + 1 < gridPoints.Length && roomNumber % levelGridSize.x != levelGridSize.x - 1)
                            {
                                otherRoom = roomNumber + levelGridSize.x + 1;

                                j = 0;
                            }
                            break;

                        default:

                            break;
                    }
                    #endregion

                    //If valid room was selected
                    if (otherRoom >= 0 && j >= 0 && otherRoom != 4)
                    {
                        RoomGenerator otherRoomGen = gridPoints[otherRoom].GetComponent<RoomGenerator>();

                        if (roomBool[otherRoom])
                        {
                            //Telling this room that there is a room in the i position
                            roomGen.SetAdjacentRoomBool(i, true);
                            //Telling other room that there is a room in the j position
                            otherRoomGen.SetAdjacentRoomBool(j, true);
                        }

                        if (Roll(chanceToCreateRoom) && canGenerateRoom)
                        {
                            bool createdCorridor = false;

                            //If there is no room or corridor occupying this space
                            if (!roomBool[otherRoom] && !corridorBool[otherRoom])
                            {
                                //Generating room in position i
                                GenerateRoom(otherRoom, j);
                                roomBool[otherRoom] = true;

                                //Telling this room that there is a room in the i position
                                roomGen.SetAdjacentRoomBool(i, true);
                                //Telling other room that there is a room in the j position
                                otherRoomGen.SetAdjacentRoomBool(j, true);

                                //Choosing the shape of the room
                                otherRoomGen.ChooseSmallRoomLayout(j);
                                //Setting distance from start
                                otherRoomGen.SetDistanceFromStart(roomGen.GetDistanceFromStart() + 1);

                                //Create Corridor to new room
                                StartCoroutine(CreateCorridor(roomNumber, i));
                                createdCorridor = true;

                                //Storing this room number to be added later to active rooms
                                toBeAdded.Add(otherRoom);

                                totalNoOfRoomsGenerated++;

                                //Note room has been generated
                                noOfRoomsGenerated++;
                            }

                            //Generate corridor if room already exists
                            if (!roomGen.CheckForCorridor(i) && roomBool[otherRoom] && !createdCorridor)
                            {
                                if (Roll(chanceToCreateCorridor) && otherRoomGen.validRGPositions[j])
                                {
                                    StartCoroutine(CreateCorridor(roomNumber, i));

                                    //Set the new distances from start
                                    if (otherRoomGen.GetDistanceFromStart() < roomGen.GetDistanceFromStart())
                                    {
                                        roomGen.SetDistanceFromStart(otherRoomGen.GetDistanceFromStart() + 1);
                                    }
                                    else
                                    {
                                        otherRoomGen.SetDistanceFromStart(roomGen.GetDistanceFromStart() + 1);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            //If this room has generated another room, replace it on the active list with the new room(s)
            if (noOfRoomsGenerated > 0)
            {
                toBeRemoved.Add(roomNumber);
            }
        }

        //Removing rooms from active list and adding to passive
        foreach (int room in toBeRemoved.ToArray())
        {
            activeRooms.Remove(room);
            existingRooms.Add(room);
        }
        toBeRemoved.Clear();


        //Adding newly generated rooms to active list
        foreach (int room in toBeAdded.ToArray())
        {
            activeRooms.Add(room);
        }
        toBeAdded.Clear();
    }

    void GenerateNavMesh()
    {
        GetComponent<NavMeshSurface>().BuildNavMesh();
    }

    //Generating a grid of room prefabs
    void GenerateGridPrefabs()
    {
        gridPoints = new GameObject[levelGridSize.x * levelGridSize.y];

        for (int i = 0; i < levelGridSize.y; i++)
        {
            for (int j = 0; j < levelGridSize.x; j++)
            {
                gridPoints[j + levelGridSize.x * i] = Instantiate(emptyRoomPrefab, new Vector3((j * spaceBetweenRooms) - spaceBetweenRooms * (Mathf.Floor(levelGridSize.x / 2)), 0, ((i * -spaceBetweenRooms) + spaceBetweenRooms * Mathf.Floor(levelGridSize.x / 2))) + transform.position, transform.rotation, transform);
                gridPoints[j + levelGridSize.x * i].name = "Room " + (j + levelGridSize.x * i);
            }
        }

        roomBool = new bool[gridPoints.Length];
        corridorBool = new bool[gridPoints.Length];

        startRoom = gridPoints.Length / 2;
    }

    void GenerateRoom(int gridPoint, int adjacentRoomPosition)
    {
        RoomGenerator roomGen = gridPoints[gridPoint].GetComponent<RoomGenerator>();

        roomList.Add(roomGen.GenerateRoom());
    }

    IEnumerator CreateCorridor(int roomNumber, int adjacentRoomPosition)
    {
        //The RoomGenerator script associated with this room
        RoomGenerator roomGen = gridPoints[roomNumber].GetComponent<RoomGenerator>();

        //If we have already spawned a corridor in this position, do nothing
        if (roomGen.CheckForCorridor(adjacentRoomPosition))
        {
            yield break;
        }

        int doorPosition = 0;
        int otherRoomDoorPosition = 0;

        Vector3 spawnPos = Vector3.zero;
        Vector3 spawnRot = Vector3.zero;


        //The RoomGenrator script of the room we are connecting to
        RoomGenerator connectingRoomGen;
        //The position that this room is in relative to the room we are connecing with
        int roomPositionRelativeToConnectingRoom = 4;

        //The grid point which the corridor will be in (if it's a diagonal connection)
        int corridorPosition = -1;

        //Which position and rotation to create the corridor in. Also gets the RoomGenerator script of the connecting room
        switch (adjacentRoomPosition)
        {
            //Top Left
            case 0:

                if (roomBool[roomNumber - 1] && roomBool[roomNumber - levelGridSize.x])
                {
                    yield break;
                }

                if (!roomBool[roomNumber - 1] && !roomBool[roomNumber - levelGridSize.x])
                {
                    if (Roll(50))
                    {
                        //From Left
                        spawnPos = gridPoints[roomNumber].transform.position + (Vector3.forward * (29) - Vector3.right * 36);
                        spawnRot = Vector3.up * 180;

                        //Int to send PlaceDoorway function so it knows what position to place the doorway
                        doorPosition = 11;
                        otherRoomDoorPosition = 6;

                        corridorPosition = roomNumber - 1;
                    }
                    else
                    {
                        //From Top
                        spawnPos = gridPoints[roomNumber].transform.position + (Vector3.forward * (21) - Vector3.right * 14);
                        spawnRot = Vector3.zero;

                        //Int to send PlaceDoorway function so it knows what position to place the doorway
                        doorPosition = 0;
                        otherRoomDoorPosition = 5;

                        corridorPosition = roomNumber - levelGridSize.x;
                    }
                }

                if (roomBool[roomNumber - 1] && !roomBool[roomNumber - levelGridSize.x])
                {
                    //From Top
                    spawnPos = gridPoints[roomNumber].transform.position + (Vector3.forward * (21) - Vector3.right * 14);
                    spawnRot = Vector3.zero;

                    //Int to send PlaceDoorway function so it knows what position to place the doorway
                    doorPosition = 0;
                    otherRoomDoorPosition = 5;

                    corridorPosition = roomNumber - levelGridSize.x;
                }

                if (!roomBool[roomNumber - 1] && roomBool[roomNumber - levelGridSize.x])
                {
                    //From Left
                    spawnPos = gridPoints[roomNumber].transform.position + (Vector3.forward * (29) - Vector3.right * 36);
                    spawnRot = Vector3.up * 180;

                    //Int to send PlaceDoorway function so it knows what position to place the doorway
                    doorPosition = 11;
                    otherRoomDoorPosition = 6;

                    corridorPosition = roomNumber - 1;
                }

                connectingRoomGen = gridPoints[roomNumber - levelGridSize.x - 1].GetComponent<RoomGenerator>();
                roomPositionRelativeToConnectingRoom = 8;
                break;

            //Top Mid
            case 1:
                spawnPos = gridPoints[roomNumber].transform.position + (Vector3.forward * (21) - Vector3.right * 4);
                spawnRot = Vector3.zero;

                //Int to send PlaceDoorway function so it knows what position to place the doorway
                doorPosition = 1;
                otherRoomDoorPosition = 7;

                connectingRoomGen = gridPoints[roomNumber - levelGridSize.x].GetComponent<RoomGenerator>();
                roomPositionRelativeToConnectingRoom = 7;
                break;

            //Top Right
            case 2:

                if (roomBool[roomNumber + 1] && roomBool[roomNumber - levelGridSize.x])
                {
                    yield break;
                }

                if (!roomBool[roomNumber + 1] && !roomBool[roomNumber - levelGridSize.x])
                {
                    if (Roll(50))
                    {
                        //From Right
                        spawnPos = gridPoints[roomNumber].transform.position + (Vector3.forward * (14) + Vector3.right * 21);
                        spawnRot = Vector3.up * 90;

                        //Int to send PlaceDoorway function so it knows what position to place the doorway
                        doorPosition = 3;
                        otherRoomDoorPosition = 8;

                        corridorPosition = roomNumber + 1;
                    }
                    else
                    {
                        //From Top
                        spawnPos = gridPoints[roomNumber].transform.position + (Vector3.forward * (36) + Vector3.right * 29);
                        spawnRot = Vector3.up * -90;

                        //Int to send PlaceDoorway function so it knows what position to place the doorway
                        doorPosition = 2;
                        otherRoomDoorPosition = 9;

                        corridorPosition = roomNumber - levelGridSize.x;
                    }
                }

                if (roomBool[roomNumber + 1] && !roomBool[roomNumber - levelGridSize.x])
                {
                    //From Top
                    spawnPos = gridPoints[roomNumber].transform.position + (Vector3.forward * (36) + Vector3.right * 29);
                    spawnRot = Vector3.up * -90;

                    //Int to send PlaceDoorway function so it knows what position to place the doorway
                    doorPosition = 2;
                    otherRoomDoorPosition = 9;

                    corridorPosition = roomNumber - levelGridSize.x;
                }

                if (!roomBool[roomNumber + 1] && roomBool[roomNumber - levelGridSize.x])
                {
                    //From Right
                    spawnPos = gridPoints[roomNumber].transform.position + (Vector3.forward * (14) + Vector3.right * 21);
                    spawnRot = Vector3.up * 90;

                    //Int to send PlaceDoorway function so it knows what position to place the doorway
                    doorPosition = 3;
                    otherRoomDoorPosition = 8;

                    corridorPosition = roomNumber + 1;
                }

                connectingRoomGen = gridPoints[roomNumber - levelGridSize.x + 1].GetComponent<RoomGenerator>();
                roomPositionRelativeToConnectingRoom = 6;
                break;

            //Left
            case 3:
                spawnPos = gridPoints[roomNumber].transform.position + (-Vector3.right * (21) - Vector3.forward * 4);
                spawnRot = Vector3.up * -90;

                //Int to send PlaceDoorway function so it knows what position to place the doorway
                doorPosition = 10;
                otherRoomDoorPosition = 4;

                connectingRoomGen = gridPoints[roomNumber - 1].GetComponent<RoomGenerator>();
                roomPositionRelativeToConnectingRoom = 5;
                break;

            //Right
            case 5:
                spawnPos = gridPoints[roomNumber].transform.position + (Vector3.right * (21) + Vector3.forward * 4);
                spawnRot = Vector3.up * 90;

                //Int to send PlaceDoorway function so it knows what position to place the doorway
                doorPosition = 4;
                otherRoomDoorPosition = 10;

                connectingRoomGen = gridPoints[roomNumber + 1].GetComponent<RoomGenerator>();
                roomPositionRelativeToConnectingRoom = 3;
                break;

            case 6:
                //Bottom Left

                if (roomBool[roomNumber - 1] && roomBool[roomNumber + levelGridSize.x])
                {
                    yield break;
                }

                if (!roomBool[roomNumber - 1] && !roomBool[roomNumber + levelGridSize.x])
                {
                    if (Roll(50))
                    {
                        //From Left
                        spawnPos = gridPoints[roomNumber].transform.position + (-Vector3.forward * (14) - Vector3.right * 21);
                        spawnRot = Vector3.up * -90;

                        //Int to send PlaceDoorway function so it knows what position to place the doorway
                        doorPosition = 9;
                        otherRoomDoorPosition = 2;

                        corridorPosition = roomNumber - 1;
                    }
                    else
                    {
                        //From Bottom
                        spawnPos = gridPoints[roomNumber].transform.position + (-Vector3.forward * (36) - Vector3.right * 29);
                        spawnRot = Vector3.up * 90;

                        //Int to send PlaceDoorway function so it knows what position to place the doorway
                        doorPosition = 8;
                        otherRoomDoorPosition = 3;

                        corridorPosition = roomNumber + levelGridSize.x;
                    }
                }

                if (roomBool[roomNumber - 1] && !roomBool[roomNumber + levelGridSize.x])
                {
                    //From Bottom
                    spawnPos = gridPoints[roomNumber].transform.position + (-Vector3.forward * (36) - Vector3.right * 29);
                    spawnRot = Vector3.up * 90;

                    //Int to send PlaceDoorway function so it knows what position to place the doorway
                    doorPosition = 8;
                    otherRoomDoorPosition = 3;

                    corridorPosition = roomNumber + levelGridSize.x;
                }

                if (!roomBool[roomNumber - 1] && roomBool[roomNumber + levelGridSize.x])
                {
                    //From Left
                    spawnPos = gridPoints[roomNumber].transform.position + (-Vector3.forward * (14) - Vector3.right * 21);
                    spawnRot = Vector3.up * -90;

                    //Int to send PlaceDoorway function so it knows what position to place the doorway
                    doorPosition = 9;
                    otherRoomDoorPosition = 2;

                    corridorPosition = roomNumber - 1;
                }

                connectingRoomGen = gridPoints[roomNumber + levelGridSize.x - 1].GetComponent<RoomGenerator>();
                roomPositionRelativeToConnectingRoom = 2;
                break;

            //Bottom Mid
            case 7:
                spawnPos = gridPoints[roomNumber].transform.position + (-Vector3.forward * (21) + Vector3.right * 4);
                spawnRot = Vector3.up * 180;

                //Int to send PlaceDoorway function so it knows what position to place the doorway
                doorPosition = 7;
                otherRoomDoorPosition = 1;

                connectingRoomGen = gridPoints[roomNumber + levelGridSize.x].GetComponent<RoomGenerator>();
                roomPositionRelativeToConnectingRoom = 1;
                break;

            case 8:
                //Bottom Right

                if (roomBool[roomNumber + 1] && roomBool[roomNumber + levelGridSize.x])
                {
                    yield break;
                }

                if (!roomBool[roomNumber + 1] && !roomBool[roomNumber + levelGridSize.x])
                {
                    if (Roll(50))
                    {
                        //From Right
                        spawnPos = gridPoints[roomNumber].transform.position + (-Vector3.forward * (29) + Vector3.right * 36f);
                        spawnRot = Vector3.zero;

                        //Int to send PlaceDoorway function so it knows what position to place the doorway
                        doorPosition = 5;
                        otherRoomDoorPosition = 0;

                        corridorPosition = roomNumber + 1;
                    }
                    else
                    {
                        //From Bottom
                        spawnPos = gridPoints[roomNumber].transform.position + (-Vector3.forward * (21) + Vector3.right * 14);
                        spawnRot = Vector3.up * 180;

                        //Int to send PlaceDoorway function so it knows what position to place the doorway
                        doorPosition = 6;
                        otherRoomDoorPosition = 11;

                        corridorPosition = roomNumber + levelGridSize.x;
                    }
                }

                if (roomBool[roomNumber + 1] && !roomBool[roomNumber + levelGridSize.x])
                {
                    //From Bottom
                    spawnPos = gridPoints[roomNumber].transform.position + (-Vector3.forward * (21) + Vector3.right * 14);
                    spawnRot = Vector3.up * 180;

                    //Int to send PlaceDoorway function so it knows what position to place the doorway
                    doorPosition = 6;
                    otherRoomDoorPosition = 11;

                    corridorPosition = roomNumber + levelGridSize.x;
                }

                if (!roomBool[roomNumber + 1] && roomBool[roomNumber + levelGridSize.x])
                {
                    //From Right
                    spawnPos = gridPoints[roomNumber].transform.position + (-Vector3.forward * (29) + Vector3.right * 36f);
                    spawnRot = Vector3.zero;

                    //Int to send PlaceDoorway function so it knows what position to place the doorway
                    doorPosition = 5;
                    otherRoomDoorPosition = 0;

                    corridorPosition = roomNumber + 1;
                }

                connectingRoomGen = gridPoints[roomNumber + levelGridSize.x + 1].GetComponent<RoomGenerator>();
                roomPositionRelativeToConnectingRoom = 0;
                break;

            default:
                spawnPos = Vector3.zero;
                spawnRot = Vector3.zero;

                connectingRoomGen = gridPoints[0].GetComponent<RoomGenerator>();
                break;
        }


        //Spawning corridor
        GameObject corridor;
        if (adjacentRoomPosition == 1 || adjacentRoomPosition == 3 || adjacentRoomPosition == 5 || adjacentRoomPosition == 7)
        {
            corridor = Instantiate(roomGen.corridorStraightShort, spawnPos, Quaternion.Euler(spawnRot), gridPoints[roomNumber].transform);
        }
        else
        {
            corridor = Instantiate(roomGen.corridorCorner, spawnPos, Quaternion.Euler(spawnRot), gridPoints[roomNumber].transform);
        }


        //Adding corridor to list
        corridorList.Add(corridor);
        //Telling the RoomGenerator script of this room that there now exists a connection to another room and storing GO in array
        roomGen.SetRoomConnections(adjacentRoomPosition, true);
        roomGen.SetCorridors(corridor, doorPosition);
        //Telling the RoomGenerator script of the other room that there now exists a connection to this room and storing GO in array
        connectingRoomGen.SetRoomConnections(roomPositionRelativeToConnectingRoom, true);
        connectingRoomGen.SetCorridors(corridor, otherRoomDoorPosition);

        if (corridorPosition >= 0)
        {
            //Setting corridor bool on relevant grid point
            corridorBool[corridorPosition] = true;
        }

        //Calling door generating function
        doorList.Add(roomGen.PlaceDoorway(doorPosition));
        doorList.Add(connectingRoomGen.PlaceDoorway(otherRoomDoorPosition));
    }

    void SetLargeRooms()
    {
        //Keeping track of how many large rooms our level has
        int largeRooms = 0;
        //Checking our level for any suitable positions to place a large room
        List<int> availableRooms = new List<int>();
        for (int i = 0; i < roomBool.Length; i++)
        {
            if (roomBool[i])
            {
                if (!roomBool[i - 1] && !roomBool[i + 1] && i != startRoom)
                {
                    availableRooms.Add(i);
                }
            }
        }

        if (availableRooms.Count > 0)
        {
            while (largeRooms < maxNoOfLargeRooms && availableRooms.Count > 0)
            {
                int randNo = Random.Range(0, availableRooms.Count);
                //Storing door positions of old room to place doorways in new room
                List<int> doorPositions = new List<int>();
                RoomGenerator roomGen = gridPoints[availableRooms[randNo]].GetComponent<RoomGenerator>();
                for (int i = 0; i < 12; i++)
                {
                    if (roomGen.CheckForDoor(i))
                    {
                        doorPositions.Add(i);
                    }
                }

                //Clearing old room
                roomGen.transform.GetChild(0).GetComponent<RoomScript>().PrepareForDeletion();
                Destroy(roomGen.transform.GetChild(0).gameObject);

                //Generating new room
                roomList.Add(roomGen.GenerateLargeRoom());
                largeRoomPositions.Add(availableRooms[randNo]);
                roomGen.SetAsLargeRoom();
                //Placing doorways
                foreach (int doorPosition in doorPositions)
                {
                    roomGen.PlaceDoorway(doorPosition);
                    roomGen.ReplaceCorridorForLargeRoom(doorPosition);
                }
                //Choosing large room layout
                roomGen.ChooseLargeRoomLayout();

                availableRooms.Remove(availableRooms[randNo]);
                largeRooms++;
            }
        }
    }

    //Setting Boss Room and Boss Key Room
    void SetSpecialRooms()
    {
        //For storing all rooms with one connection
        List<int> oneConnectionRooms = new List<int>();
        //For storing all rooms with two connections
        List<int> twoConnectionRooms = new List<int>();

        //The room number that will have the boss door in it
        int bossDoorRoomNumber = -1;
        //The room number that will have the boss key in it
        int bossKeyRoomNumber = -1;

        //This list is set to twoConnectionRooms if oneConnectionRooms is empty;
        List<int> currentList;

        //The room number of the 2nd furthest room from the start
        int secondRoomNumber = -1;
        //If no room number found for secondRoomNumber, choose this
        int altFurthestRoomNumber = -1;
        //The previous greatest distance
        int previousDistance = 0;

        //Cycling through all rooms and storing ones with one or two connections
        for (int i = 0; i < gridPoints.Length; i++)
        {
            //Making it so that large rooms and the start room cannot be 'special' rooms
            if (i != startRoom)
            {
                //If there is a room in this position
                if (roomBool[i])
                {
                    RoomGenerator roomGen = gridPoints[i].GetComponent<RoomGenerator>();
                    if (roomGen.NoOfCorridors() == 1)
                    {
                        oneConnectionRooms.Add(i);
                    }

                    if (roomGen.NoOfCorridors() == 2)
                    {
                        twoConnectionRooms.Add(i);
                    }
                }
            }
        }

        //Have we selected a room yet?
        bool hasSelectedRoom = false;

        //Setting currentList
        if (oneConnectionRooms.Count == 0 && twoConnectionRooms.Count == 0)
        {
            currentList = new List<int>();
            for (int i = 0; i < roomBool.Length; i++)
            {
                if (roomBool[i])
                {
                    currentList.Add(i);
                }
            }
        }
        else
        {
            if (oneConnectionRooms.Count == 0)
            {
                currentList = twoConnectionRooms;
            }
            else
            {
                currentList = oneConnectionRooms;
            }
        }

        foreach (int roomNumber in currentList)
        {
            if (!largeRoomPositions.Contains(roomNumber))
            {
                if (!specialRoomsFurthestMethod)
                {
                    //Chance to set room
                    if (Roll(100 / oneConnectionRooms.Count))
                    {
                        hasSelectedRoom = true;
                    }
                    //If room hasn't been set but we have reached the last in the list, set
                    if (!hasSelectedRoom && oneConnectionRooms.IndexOf(roomNumber) == oneConnectionRooms.Count - 1)
                    {
                        hasSelectedRoom = true;
                    }

                    if (hasSelectedRoom)
                    {
                        if (gridPoints[roomNumber].GetComponent<RoomGenerator>().GetDistanceFromStart() > 1)
                        {

                            bossDoorRoomNumber = roomNumber;

                            currentList.Remove(roomNumber);
                            break;
                        }
                        else
                        {
                            hasSelectedRoom = false;
                        }
                    }
                }
                else
                {
                    RoomGenerator roomGen = gridPoints[roomNumber].GetComponent<RoomGenerator>();
                    if (roomGen.GetDistanceFromStart() > previousDistance)
                    {
                        previousDistance = roomGen.GetDistanceFromStart();
                        secondRoomNumber = bossDoorRoomNumber;

                        bossDoorRoomNumber = roomNumber;

                    }
                    else if (roomGen.GetDistanceFromStart() == previousDistance - 1)
                    {
                        secondRoomNumber = roomNumber;
                    }
                    else if (roomGen.GetDistanceFromStart() == previousDistance)
                    {
                        altFurthestRoomNumber = roomNumber;
                    }
                }
            }
            //If we didn't get a room number for the boss room earlier, try a different, more reliable way
            if (bossDoorRoomNumber < 0)
            {
                List<int> impRoomList = new List<int>();
                //Go through all possible rooms
                for (int i = 0; i < roomBool.Length; i++)
                {
                    //If there is a room in this position and the grid point is more than 2 spaces from the start room, add int
                    if (roomBool[i] && gridPoints[i].GetComponent<RoomGenerator>().GetDistanceFromStart() > 2)
                    {
                        impRoomList.Add(i);
                    }
                }
                //If no rooms were more than 2 spaces from the start room, our list will still be empty; so try again
                if (impRoomList.Count <= 0)
                {
                    for (int i = 0; i < roomBool.Length; i++)
                    {
                        //This time, rooms will only need to be more than one grid space away
                        if (roomBool[i] && gridPoints[i].GetComponent<RoomGenerator>().GetDistanceFromStart() > 1)
                        {
                            impRoomList.Add(i);
                        }
                    }
                }
                //If there were still no rooms that met the requirements so far, just add all possible rooms
                if (impRoomList.Count <= 0)
                {
                    for (int i = 0; i < roomBool.Length; i++)
                    {
                        if (roomBool[i])
                        {
                            impRoomList.Add(i);
                        }
                    }
                }
                //Choose one of the rooms at random and make it the boss room
                int randNo = Random.Range(0, impRoomList.Count);
                bossDoorRoomNumber = impRoomList[randNo];
            }
        }

        //Select Boss Key Room
        hasSelectedRoom = false;

        //If the one room that was in currentList is now used up, reassign currentList
        if (oneConnectionRooms.Count == 1)
        {
            if (twoConnectionRooms.Count == 0)
            {
                currentList = new List<int>();
                for (int i = 0; i < roomBool.Length; i++)
                {
                    if (roomBool[i])
                    {
                        currentList.Add(i);
                    }
                }
            }
            else
            {
                currentList = twoConnectionRooms;
            }
        }

        if (!specialRoomsFurthestMethod)
        {
            foreach (int roomNumber in currentList)
            {
                if (Roll(100 / currentList.Count))
                {
                    hasSelectedRoom = true;
                }

                if (!hasSelectedRoom && currentList.IndexOf(roomNumber) == currentList.Count - 1)
                {
                    hasSelectedRoom = true;
                }

                if (hasSelectedRoom)
                {
                    if (gridPoints[roomNumber].GetComponent<RoomGenerator>().GetDistanceFromStart() > 1)
                    {
                        bossKeyRoomNumber = roomNumber;
                        currentList.Remove(roomNumber);
                        break;
                    }
                    else
                    {
                        hasSelectedRoom = false;
                    }
                }
            }
        }
        else
        {
            if (secondRoomNumber >= 0)
            {
                bossKeyRoomNumber = secondRoomNumber;
            }
            else
            {
                bossKeyRoomNumber = altFurthestRoomNumber;
            }
        }

        //Setting Boss Room
        Debug.Log("Boss Room: " + bossDoorRoomNumber);
        gridPoints[bossDoorRoomNumber].GetComponent<RoomGenerator>().SetSpecialRoom(0);
        specialRooms.Add(bossDoorRoomNumber);

        //Looking for an empty wall that doesn't have a room behind it to put the boss door there
        bool lookingForEmptyWall = true;
        while (lookingForEmptyWall)
        {
            int bossDoorPosition = Random.Range(0, 9);

            switch (bossDoorPosition)
            {
                case 1:
                    if (!gridPoints[bossDoorRoomNumber].GetComponent<RoomGenerator>().CheckForRoom(bossDoorPosition))
                    {
                        lookingForEmptyWall = false;
                        gridPoints[bossDoorRoomNumber].GetComponent<RoomGenerator>().PlaceBossDoor(1).
                            transform.GetChild(0).GetComponent<BossDoorScript>().SetBossRoomSpawnPoint(bossRoomSpawnPoint);
                    }
                    break;

                case 3:
                    if (!gridPoints[bossDoorRoomNumber].GetComponent<RoomGenerator>().CheckForRoom(bossDoorPosition))
                    {
                        lookingForEmptyWall = false;
                        gridPoints[bossDoorRoomNumber].GetComponent<RoomGenerator>().PlaceBossDoor(10).
                            transform.GetChild(0).GetComponent<BossDoorScript>().SetBossRoomSpawnPoint(bossRoomSpawnPoint);
                    }
                    break;

                case 5:
                    if (!gridPoints[bossDoorRoomNumber].GetComponent<RoomGenerator>().CheckForRoom(bossDoorPosition))
                    {
                        lookingForEmptyWall = false;
                        gridPoints[bossDoorRoomNumber].GetComponent<RoomGenerator>().PlaceBossDoor(4).
                            transform.GetChild(0).GetComponent<BossDoorScript>().SetBossRoomSpawnPoint(bossRoomSpawnPoint);
                    }
                    break;

                case 7:
                    if (!gridPoints[bossDoorRoomNumber].GetComponent<RoomGenerator>().CheckForRoom(bossDoorPosition))
                    {
                        lookingForEmptyWall = false;
                        gridPoints[bossDoorRoomNumber].GetComponent<RoomGenerator>().PlaceBossDoor(7).
                            transform.GetChild(0).GetComponent<BossDoorScript>().SetBossRoomSpawnPoint(bossRoomSpawnPoint);
                    }
                    break;

                default:

                    break;
            }
        }

        //Setting boss key room if a room was found
        Debug.Log("Boss Key Room: " + bossKeyRoomNumber);
        if (bossKeyRoomNumber != -1)
        {
            gridPoints[bossKeyRoomNumber].GetComponent<RoomGenerator>().SetSpecialRoom(1);
            specialRooms.Add(bossKeyRoomNumber);
        }

        //Redoing room list
        currentList.Clear();
        currentList = new List<int>();
        for (int i = 0; i < roomBool.Length; i++)
        {
            if (roomBool[i] && !specialRooms.Contains(i))
            {
                if (gridPoints[i].GetComponent<RoomGenerator>().GetDistanceFromStart() > 1)
                {
                    currentList.Add(i);
                }
            }
        }

        //Retrying boss key room allocation with new room list if one hasn't been found
        if (bossKeyRoomNumber == -1)
        {
            //Set boss key room if none found earlier
            int randNo = Random.Range(0, currentList.Count);
            bossKeyRoomNumber = currentList[randNo];
            Debug.Log("New Boss Key Room: " + bossKeyRoomNumber);
            gridPoints[bossKeyRoomNumber].GetComponent<RoomGenerator>().SetSpecialRoom(2);
            specialRooms.Add(bossKeyRoomNumber);
            currentList.Remove(bossKeyRoomNumber);
        }

        //Setting any chest rooms
        int chestRoomNumber = Random.Range(0, currentList.Count);
        Debug.Log("Chest Room: " + currentList[chestRoomNumber]);
        gridPoints[currentList[chestRoomNumber]].GetComponent<RoomGenerator>().SetSpecialRoom(2);
        specialRooms.Add(currentList[chestRoomNumber]);
        currentList.Remove(currentList[chestRoomNumber]);

        //Setting challenge rooms
        int challengeRoomNumber = -1;
        while (challengeRoomNumber < 0)
        {
            challengeRoomNumber = Random.Range(0, currentList.Count);
            if (largeRoomPositions.Contains(challengeRoomNumber))
            {
                challengeRoomNumber = -1;
            }
        }
        Debug.Log("Challenge Room: " + currentList[challengeRoomNumber]);
        RoomGenerator challengeRoomGen = gridPoints[currentList[challengeRoomNumber]].GetComponent<RoomGenerator>();
        challengeRoomGen.SetSpecialRoom(3);
        specialRooms.Add(currentList[challengeRoomNumber]);
        currentList.Remove(currentList[challengeRoomNumber]);

        //Activating all GameObjects relevant to the challenge room and deactivating others
        Transform roomTransform = challengeRoomGen.transform.GetChild(0).GetChild(challengeRoomGen.GetLayoutNumber());
        for (int i = 0; i < roomTransform.childCount; i++)
        {
            GameObject roomChildGO = roomTransform.GetChild(i).gameObject;
            if (roomChildGO.tag == "Challenge_Room")
            {
                roomChildGO.SetActive(true);
            }
            else
            {
                roomChildGO.SetActive(false);
            }
        }
    }

    void PlacePlayer()
    {
        GameObject player = GameObject.FindWithTag("Player");

        player.transform.position = gridPoints[startRoom].transform.position;
        gridPoints[startRoom].GetComponent<RoomGenerator>().SetAsStartRoom();

        player.GetComponent<NavMeshAgent>().enabled = true;
        player.GetComponent<CapsuleCollider>().enabled = true;
        player.GetComponent<Player_Input>().input_Enabled = true;
    }

    void SetDoorways()
    {
        for (int i = 0; i < roomBool.Length; i++)
        {
            if (roomBool[i])
            {
                gridPoints[i].GetComponent<RoomGenerator>().SetDoorways();
            }
        }
    }

    void SpawnBossRoom()
    {
        GameObject bossRoom = Instantiate(bossRoomPrefab, new Vector3((levelGridSize.x / 1.5f) * spaceBetweenRooms, 0, 0), Quaternion.identity);
        //REQUIRES THE SPAWN POINT TO BE THE LAST CHILD OF THE BOSS ROOM PREFAB!!!
        bossRoomSpawnPoint = bossRoom.transform.GetChild(bossRoom.transform.childCount - 1);
    }

    void ClearAllRooms()
    {
        foreach (GameObject go in roomList.ToArray())
        {
            roomList.Remove(go);
            Destroy(go);
        }
        roomList.Clear();

        for (int i = 0; i < gridPoints.Length; i++)
        {
            RoomGenerator rG = gridPoints[i].GetComponent<RoomGenerator>();
            rG.ClearAdjacentRoomBools();
            rG.ClearRoomConnections();
        }
    }

    IEnumerator ClearRoom(int roomNumber, bool plusCorridors)
    {
        Transform roomTransform = gridPoints[roomNumber].transform;
        RoomGenerator roomGen = roomTransform.GetComponent<RoomGenerator>();
        roomGen.ClearAdjacentRoomBools();
        roomGen.ClearRoomConnections();
        roomGen.ClearDoorways();

        roomTransform.GetChild(0).GetComponent<RoomScript>().PrepareForDeletion();
        Destroy(roomTransform.GetChild(0).gameObject);
        //roomTransform.GetChild(0).gameObject.SetActive(false);
        if (plusCorridors)
        {
            while (roomTransform.childCount > 0)
            {
                Destroy(roomTransform.GetChild(0).gameObject);
                yield return null;
            }
        }
    }

    void ClearAllCorridors()
    {
        foreach (GameObject go in corridorList.ToArray())
        {
            corridorList.Remove(go);
            Destroy(go);
        }
        corridorList.Clear();

        foreach (GameObject go in doorList.ToArray())
        {
            doorList.Remove(go);
            Destroy(go);
        }
        doorList.Clear();

        for (int i = 0; i < gridPoints.Length; i++)
        {
            RoomGenerator rG = gridPoints[i].GetComponent<RoomGenerator>();
            rG.ClearRoomConnections();
        }
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
