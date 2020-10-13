using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    const float HEAD_OFFSET = 1f;
    const float NPC_DISTANCE_CHECK = 0.8f;
    const float DISTANCE_TO_GROUND = 0.1f;

    //CharacterController controller;
    Rigidbody rbody;
    Animator anim;
    public float speed;
    public float jumpForce;
    public float speedMultiplier = 1.5f;
    const float walkSpeedDivision = 0.5f;

    float horizontal;
    float vertical;
    Vector3 lastFacinDirection;

    [HideInInspector]
    public bool isShielding;
    bool isSprinting;
    [HideInInspector]
    public bool swordEquipped = false;
    public bool isAttacking = false;

    private bool isInteracting = false;
    int attackCombo = -1;


    // Start is called before the first frame update
    void Start()
    {
        // controller = GetComponent<CharacterController>();
        rbody = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.Instance.inPlayMode)
        {
            GetDirectionalInput();
            UseShield();
            DrawSword();
            SwordAttacks();
            CheckForNPC();
        }

    }
    void FixedUpdate()
    {
        if (GameController.Instance.inPlayMode)
        {
            if (Grounded())
            {
                DirectionalMovement();
            }
            else // for natural jumping, can't change direction in mid air
            {
                rbody.velocity = new Vector3(horizontal * speed * Time.fixedDeltaTime, rbody.velocity.y, vertical * speed * Time.fixedDeltaTime);
            }
        }
    }

    void GetDirectionalInput()
    {
        horizontal = -Input.GetAxis("Horizontal");
        vertical = -Input.GetAxis("Vertical");
    }
    void DirectionalMovement()
    {

        anim.SetBool("isSprinting", isSprinting);

        if (horizontal != 0 || vertical != 0)
        {
            anim.SetFloat("moveVelocity", 1f);

            if (Input.GetButton("Sprint") && !isShielding)
                isSprinting = true;
            else
                isSprinting = false;
            lastFacinDirection = new Vector3(horizontal, 0f, vertical);
        
            // It is for keeping the direction while shielding
            //if (!isShielding)
            //    if (Mathf.Abs(horizontal) > 0.2f || Mathf.Abs(vertical) > 0.2f)
            //        lastFacinDirection = new Vector3(horizontal, 0f, vertical);
        }
        else
        {
            anim.SetFloat("moveVelocity", 0f);
            isSprinting = false;
        }

        transform.forward = lastFacinDirection.normalized;

        Vector3 movementVector = new Vector3(horizontal, rbody.velocity.y, vertical);

        if (movementVector.magnitude > 0.1f)
        { 
            if (isSprinting)
                rbody.velocity = movementVector.normalized * speed * speedMultiplier * Time.fixedDeltaTime;
            else if (isShielding)
                rbody.velocity = movementVector.normalized * walkSpeedDivision * speed * Time.fixedDeltaTime;
            else
                rbody.velocity = movementVector.normalized * speed * Time.fixedDeltaTime;
        }
    }

    void UseShield()
    {   
        if (Input.GetButton("Shield"))
        {
            isShielding = true;
        }
        else
            isShielding = false;
        anim.SetBool("isShielding", isShielding);
    }

    void DrawSword()
    {
        if (!swordEquipped)
        {
            if (Input.GetButton("Attack"))
            {
                anim.SetBool("draw_Sword", true);
                swordEquipped = true;
            }
        }
        else
        {
            if(Input.GetKeyDown(KeyCode.T))
            {
                anim.SetBool("draw_Sword", false);
                swordEquipped = false;
                isAttacking = false;
                attackCombo = -1;
            }
        }
    }

    void SwordAttacks()
    {
        if (swordEquipped)
        {
            if (Input.GetButtonDown("Attack"))
            {
                attackCombo++;
                if(attackCombo > 0)
                {
                    isAttacking = true;
                    anim.SetTrigger("attack1");
                }
            }
        } 
    }

    public bool Grounded()
    {
        // use a spherecast instead
        return Physics.Raycast(transform.position, Vector3.down, DISTANCE_TO_GROUND);
    }

    public void Jumping()
    {
        /// For Jumping
        if (Grounded())
        {
            if (Input.GetKeyDown(KeyCode.Space))
                rbody.AddForce(rbody.velocity.x, jumpForce, rbody.velocity.z, ForceMode.Impulse);
        }
    }

    public void CheckForNPC()
    {
       // Debug.DrawRay(transform.position + new Vector3(0.1f, HEAD_OFFSET, 0), transform.forward * NPC_DISTANCE_CHECK, Color.red);
       // Debug.DrawRay(transform.position + new Vector3(-0.1f, HEAD_OFFSET, 0), transform.forward * NPC_DISTANCE_CHECK, Color.red);

        RaycastHit hit;
       // if(Physics.Raycast(transform.position + new Vector3(-0.1f, HEAD_OFFSET,  0), transform.forward, out hit , NPC_DISTANCE_CHECK) || Physics.Raycast(transform.position + new Vector3(0.1f, HEAD_OFFSET, 0), transform.forward, out hit, NPC_DISTANCE_CHECK))
        if(Physics.CapsuleCast(transform.position, transform.position + new Vector3(0, HEAD_OFFSET * 2, 0), 0.8f/*radius*/, transform.forward, out hit, NPC_DISTANCE_CHECK/*distance*/)
            || Physics.Raycast(transform.position + new Vector3(0, HEAD_OFFSET, 0), transform.forward, out hit, NPC_DISTANCE_CHECK))
        {
            if (hit.transform.GetComponent<NPCEntity>() && !isInteracting)
            {
                Debug.Log("in range");
                if (Input.GetButtonDown("Interact"))
                {
                    isInteracting = true;
                    NPCEntity _collidedNPC = hit.transform.GetComponent<NPCEntity>();
                    //PopupUIManager.Instance.dialogBoxPopup.setDialogText(_collidedNPC.dialogLines);

                    // can make it bit more good
                    var targetRotation = Quaternion.LookRotation(_collidedNPC.transform.position - transform.position);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1);

                    _collidedNPC.SetDialog();
                    _collidedNPC.LookAtTarget(transform);
                    DisablePlayerMoveActions();
                    GameController.Instance.inPlayMode = false;
                }
            }
            else
            {
                if (isInteracting)
                {
                    isInteracting = false;
                }
            }
                
           // Debug.DrawRay(transform.position + new Vector3(0, HEAD_OFFSET, 0), transform.forward * NPC_DISTANCE_CHECK, Color.green);
        }
    }

    public void DisablePlayerMoveActions()
    {
        rbody.velocity = new Vector3(0, rbody.velocity.y, 0);
        anim.SetTrigger("idle");
        anim.SetFloat("moveVelocity", 0f);
        anim.SetBool("isShielding", false);
    }

    ///// Gizmos
    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.blue;
        //Gizmos.DrawSphere(transform.position + new Vector3(transform.forward.x / 2, HEAD_OFFSET, transform.forward.z / 2), 0.6f);
       // Gizmos.DrawWireCube(transform.position, Vector3.one / 2);
    }
}
