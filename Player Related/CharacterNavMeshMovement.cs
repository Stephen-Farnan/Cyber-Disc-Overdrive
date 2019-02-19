using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CharacterNavMeshMovement : MonoBehaviour
{


    #region Variables
    [Header("Movement")]
    public float moveSpeed;
    public float turnSpeed;
    private float moveAcc = 0.05f;
    public float knockbackResistance;
    public float knockbackSpeedMultiplier = 5;
    private Vector3 knockbackDest;

    [Header("Hoverboard")]
    public GameObject hoverBoard;
    public float hoverSpeed;
    public float hoverAcc;
    private bool hovering = false;
    private bool canToggleHover = true;
    [Header("Dash")]
    public float dashDistance = 3;
    public float dashSpeedMultiplier = 3;
    public float dashCooldown = 2;
    private bool canDash = true;
    public float dash_Duration_Extension = .1f;
    public ParticleSystem dashTrailFX;
    [HideInInspector]
    public bool isDashing = false;
    private Vector3 dashTarget;
    [HideInInspector]
    public bool canMove = true;
    [HideInInspector]
    public float targetSpeed;
    private float targetAcc;
    private NavMeshAgent navAgent;

    //Current Speed
    [HideInInspector]
    public float speed;
    private float refVelocity;

    private Animator anim;

    //The camera relative to which the player will be moving
    public Transform cameraTarget;
    private Camera cam;
    private PlayerStatus playStat;

    public LayerMask mouseInputLayerMask;
    public LayerMask dashLayerMask;

    public bool usingController = false;
    #endregion

    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        cam = cameraTarget.GetComponent<Camera>();
        playStat = GetComponent<PlayerStatus>();
        anim = GetComponentInChildren<Animator>();
        targetSpeed = moveSpeed;
        targetAcc = moveAcc;
    }

    /// <summary>
    /// Enables and disables the players hoverboard
    /// </summary>
    public void ToggleHoverboard()
    {
        if (!canToggleHover)
        {
            return;
        }

        //Start Hovering
        if (!hovering)
        {
            hovering = true;
            hoverBoard.GetComponent<MeshRenderer>().enabled = true;
            targetSpeed = hoverSpeed;
            targetAcc = hoverAcc;
            canDash = false;
        }
        //Stop Hovering
        else
        {
            hovering = false;
            hoverBoard.GetComponent<MeshRenderer>().enabled = false;
            targetSpeed = moveSpeed;
            targetAcc = moveAcc;
            canDash = true;
        }

        anim.SetTrigger("Hover");
    }

    public void LockHover(bool lockState)
    {
        if (hovering)
        {
            ToggleHoverboard();
        }

        canToggleHover = !lockState;
    }

    public IEnumerator HoverboardToggleCooldown()
    {
        if (!canToggleHover)
        {
            yield break;
        }
        canToggleHover = false;
        yield return new WaitForSeconds(0.5f);
        canToggleHover = true;
    }

    public void RotateCharacter(float x = 0, float y = 0)
    {
        //If canMove is set to false, do nothing
        if (!canMove)
        {
            return;
        }

        //Mouse
        if (!usingController)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000, mouseInputLayerMask))
            {
                Vector3 dir = (hit.point - gameObject.transform.position);
                dir = new Vector3(dir.x, 0, dir.z);
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir, Vector3.up), turnSpeed);
            }
        }
        //Controller
        else
        {
            float newYRotation = Mathf.Atan2(x, y) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, newYRotation, 0);
        }
    }

    public Vector3 GetRotation()
    {
        return transform.forward;
    }

    //Checking for collisions with enemies
    void OnCollisionEnter(Collision col)
    {
        if (col.collider.tag == "Enemy")

        {
            if (!playStat.isInvulnerable)
            {
                TakeDamage(col.transform, col.gameObject.GetComponent<Enemy_AI>().contact_Damage);
            }
        }

        if (col.collider.tag == "Laser_Boss_Main_Part" || col.collider.tag == "Laser_Boss_First_Part" || col.collider.tag == "Laser_Boss_Second_Part" || col.collider.tag == "Lasers")
        {
            TakeDamage(col.transform, 1);
        }
    }

    public void TakeDamage(Transform damageSource, int damage)
    {
        if (!playStat.isInvulnerable)
        {
            GetComponent<HealthScript>().LoseHealth(damage);

            //Calculating knockback direction
            Vector3 dir = transform.position - damageSource.position;
            Knockback(dir.normalized * (1 / knockbackResistance));
        }
    }

    public void Move(float x, float y)
    {
        //If canMove is set to false, do nothing
        if (!canMove)
        {
            return;
        }

        speed = Mathf.SmoothDamp(speed, targetSpeed, ref refVelocity, targetAcc);
        navAgent.Move(new Vector3(x, 0, y).normalized * speed * Time.deltaTime);
    }

    public void Dash(float x, float y)
    {
        //If canDash is set to false, do nothing
        if (!canDash)
        {
            return;
        }
        //Setting variables
        canMove = false;
        isDashing = true;
        playStat.isInvulnerable = true;
        canDash = false;

        //Put character on 'safe' layer
        //gameObject.layer = 11;

        StartCoroutine(playStat.Phase(true));
        StartDashFX();

        //Calculating Dash Direction
        Vector3 dashDir;
        if (x != 0 || y != 0)
        {
            dashDir = new Vector3(x, 0, y).normalized;
        }
        else
        {
            dashDir = transform.forward;
        }

        //Check for obstacles and calculate Dash Destination
        RaycastHit dashHit;
        //Debug.Break();
        if (Physics.Raycast(transform.position, dashDir, out dashHit, dashDistance, dashLayerMask))
        {
            dashTarget = dashHit.point;
        }
        else
        {
            dashTarget = transform.position + dashDir * dashDistance;
        }

        //Set Dash Destination and Speed
        navAgent.speed = moveSpeed * dashSpeedMultiplier;
        navAgent.SetDestination(dashTarget);
        navAgent.isStopped = false;

        //Wait until dash is complete to give control back to the player
        StartCoroutine(WaitForDashEnd());
    }

    IEnumerator WaitForDashEnd()
    {
        while (isDashing)
        {
            if (Vector3.Distance(transform.position, dashTarget) < 1.0f)
            {
                isDashing = false;
            }
            yield return null;
        }
        canMove = true;
        navAgent.isStopped = true;
        //navAgent.ResetPath();
        yield return new WaitForSeconds(dash_Duration_Extension);
        playStat.isInvulnerable = false;

        //Take character off 'safe' layer
        //gameObject.layer = 10;

        //Wait for the cooldown to make dash available again
        Invoke("MakeDashAvailable", dashCooldown);

        StartCoroutine(playStat.Phase(false));
        StopDashFX();
    }

    void MakeDashAvailable()
    {
        canDash = true;
    }

    void StartDashFX()
    {
        dashTrailFX.Play();
    }
    void StopDashFX()
    {
        dashTrailFX.Stop();
    }

    void Knockback(Vector3 knockback)
    {
        canMove = false;
        isDashing = true;
        canDash = false;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, knockback.normalized, out hit, knockback.magnitude))
        {
            knockbackDest = hit.point;
        }
        else
        {
            knockbackDest = transform.position + knockback;
        }
        navAgent.SetDestination(knockbackDest);
        navAgent.isStopped = false;
        StartCoroutine(WaitForKnockbackEnd());
    }

    IEnumerator WaitForKnockbackEnd()
    {
        while (isDashing)
        {
            if (Vector3.Distance(transform.position, knockbackDest) < 1.0f)
            {
                isDashing = false;
            }
            yield return null;
        }
        canMove = true;
        canDash = true;
        navAgent.isStopped = true;
    }
}
