using UnityEngine;

public class RoomDoorScript : MonoBehaviour
{

    [HideInInspector]
    public RoomScript roomScript;

    //When the player gets far enough away from the door, check if they are in a room
    void OnTriggerExit(Collider col)
    {
        if (col.tag == "Player")
        {
            roomScript.CheckPlayerIsInRoom();
        }
    }
}
