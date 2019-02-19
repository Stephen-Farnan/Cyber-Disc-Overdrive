
//This script is to be attached to a camera
//It will make the camera follow a gameobject tagged "Player" on the x, y and/or z axis (set in the inspector)

using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    [Tooltip("Defaults to GameObject tagged 'Player'")]
    public Transform target;
    private Transform player;
    public float smoothTime = 1.0f;
    private float xRefVelocity = 0.0f;
    private float yRefVelocity = 0.0f;
    private float zRefVelocity = 0.0f;
    private float xOffset;
    private float yOffset;
    private float zOffset;

    private float xOffsetAdd;
    private float yOffsetAdd;
    private float zOffsetAdd;

    public bool xFollow = true;
    public bool yFollow = false;
    public bool zFollow = false;

    private float newXPosition;
    private float newYPosition;
    private float newZPosition;

    void Awake()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    void Start()
    {
        if (target == null)
        {
            target = player;
        }
        xOffset = (gameObject.transform.position - target.position).x;
        yOffset = (gameObject.transform.position - target.position).y;
        zOffset = (gameObject.transform.position - target.position).z;
    }

    void LateUpdate()
    {
        float xPos = transform.position.x;
        float yPos = transform.position.y;
        float zPos = transform.position.z;

        if (xFollow)
        {
            newXPosition = Mathf.SmoothDamp(xPos, target.position.x + xOffset, ref xRefVelocity, smoothTime);
        }
        else
        {
            newXPosition = xPos;
        }
        if (yFollow)
        {
            newYPosition = Mathf.SmoothDamp(yPos, target.position.y + yOffset, ref yRefVelocity, smoothTime);
        }
        else
        {
            newYPosition = yPos;
        }
        if (zFollow)
        {
            newZPosition = Mathf.SmoothDamp(zPos, target.position.z + zOffset, ref zRefVelocity, smoothTime);
        }
        else
        {
            newZPosition = zPos;
        }

        transform.position = new Vector3(newXPosition, newYPosition, newZPosition);
    }

    public void SetNewTarget(Transform newTarget, Vector3 additionalOffset = default(Vector3))
    {
        xOffset -= xOffsetAdd;
        yOffset -= yOffsetAdd;
        zOffset -= zOffsetAdd;

        xOffsetAdd = additionalOffset.x;
        yOffsetAdd = additionalOffset.y;
        zOffsetAdd = additionalOffset.z;

        xOffset += xOffsetAdd;
        yOffset += yOffsetAdd;
        zOffset += zOffsetAdd;

        target = newTarget;
    }

    public void SetTargetToPlayer()
    {
        if (target = player)
        {
            return;
        }
        xOffset -= xOffsetAdd;
        yOffset -= yOffsetAdd;
        zOffset -= zOffsetAdd;

        target = player;
    }
}
