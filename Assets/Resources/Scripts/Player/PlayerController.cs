using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IHittable
{
    const float fHEAD_OFFSET = 1f;
    const float fNPC_DISTANCE_CHECK = 0.8f;
    const float fDISTANCE_TO_GROUND = 0.1f;
    const float fINVULNERABILITY_TIME = 0.7f;

    Rigidbody rbody;
    Animator anim;

    public int iMaxHitPoints;
    public int iCurrentHitPoints;
    public float fSpeed;
    public float fJumpForce;
    public float sSpeedMultiplier = 1.5f;
    const float fSpeedDivision = 0.5f;
    float horizontal;
    float vertical;
    Vector3 lastFacinDirection;

    public bool bIsAlive;
    [HideInInspector]
    public bool bIsShielding;
    bool bIsSprinting;
    [HideInInspector]
    public bool bSwordEquipped = false;
    [HideInInspector]
    public bool bCanAttack = true;
    [HideInInspector]
    public bool bIsAttacking = false;
    private bool bIsInteracting = false;
    private bool bIsInvulnerable;
    int iAttackCombo = -1;

    void Start()
    {
        // controller = GetComponent<CharacterController>();
        rbody = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        iCurrentHitPoints = iMaxHitPoints;
        bIsAlive = true;
    }
    void Update()
    {
        if (GameController.Instance.inPlayMode)
        {
            if (bIsAlive)
            {
                GetDirectionalInput();
                UseShield();
                DrawSword();
                SwordAttacks();
                CheckForNPC();
            }
        }
    }
    void FixedUpdate()
    {
        if (GameController.Instance.inPlayMode)
        {
            if (bIsAlive)
            {
                if (Grounded())
                {
                    DirectionalMovement();
                }
                else // for natural jumping, can't change direction in mid air
                {
                    rbody.velocity = new Vector3(horizontal * fSpeed * Time.fixedDeltaTime, rbody.velocity.y, vertical * fSpeed * Time.fixedDeltaTime);
                }
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

        anim.SetBool("isSprinting", bIsSprinting);

        if (horizontal != 0 || vertical != 0)
        {
            anim.SetFloat("moveVelocity", 1f);

            if (Input.GetButton("Sprint") && !bIsShielding)
                bIsSprinting = true;
            else
                bIsSprinting = false;
            lastFacinDirection = new Vector3(horizontal, 0f, vertical);
        
            // It is for keeping the direction while shielding
            //if (!isShielding)
            //    if (Mathf.Abs(horizontal) > 0.2f || Mathf.Abs(vertical) > 0.2f)
            //        lastFacinDirection = new Vector3(horizontal, 0f, vertical);
        }
        else
        {
            anim.SetFloat("moveVelocity", 0f);
            bIsSprinting = false;
        }
        if(lastFacinDirection != Vector3.zero)
            transform.forward = lastFacinDirection.normalized;

        Vector3 movementVector = new Vector3(horizontal, rbody.velocity.y, vertical);

        if (movementVector.magnitude > 0.1f)
        { 
            if (bIsSprinting)
                rbody.velocity = movementVector.normalized * fSpeed * sSpeedMultiplier * Time.fixedDeltaTime;
            else if (bIsShielding)
                rbody.velocity = movementVector.normalized * fSpeedDivision * fSpeed * Time.fixedDeltaTime;
            else
                rbody.velocity = movementVector.normalized * fSpeed * Time.fixedDeltaTime;
        }
    }
    void UseShield()
    {   
        if (Input.GetButton("Shield"))
        {
            bIsShielding = true;
        }
        else
            bIsShielding = false;
        anim.SetBool("isShielding", bIsShielding);
    }
    void DrawSword()
    {
        if (!bSwordEquipped)
        {
            if (Input.GetButton("Attack"))
            {
                anim.SetBool("draw_Sword", true);
                bSwordEquipped = true;
            }
        }
        else
        {
            if(Input.GetKeyDown(KeyCode.T))
            {
                anim.SetBool("draw_Sword", false);
                bSwordEquipped = false;
                bIsAttacking = false;
                iAttackCombo = -1;
            }
        }
    }
    void SwordAttacks()
    {
        if (bSwordEquipped)
        {
            if (Input.GetButtonDown("Attack"))
            {
                iAttackCombo++;
                if(iAttackCombo > 0 && bCanAttack)
                {
                    bIsAttacking = true;
                    bCanAttack = false;
                    anim.SetTrigger("attack1");
                }
            }
            else
            {
                bIsAttacking = !bCanAttack;
            }
        } 
    }
    public bool Grounded()
    {
        // use a spherecast instead
        return Physics.Raycast(transform.position, Vector3.down, fDISTANCE_TO_GROUND);
    }
    public void Jumping()
    {
        /// For Jumping
        if (Grounded())
        {
            if (Input.GetKeyDown(KeyCode.Space))
                rbody.AddForce(rbody.velocity.x, fJumpForce, rbody.velocity.z, ForceMode.Impulse);
        }
    }
    public void CheckForNPC()
    {
       // Debug.DrawRay(transform.position + new Vector3(0.1f, HEAD_OFFSET, 0), transform.forward * NPC_DISTANCE_CHECK, Color.red);
       // Debug.DrawRay(transform.position + new Vector3(-0.1f, HEAD_OFFSET, 0), transform.forward * NPC_DISTANCE_CHECK, Color.red);

        RaycastHit hit;
        if(Physics.CapsuleCast(transform.position, transform.position + new Vector3(0, fHEAD_OFFSET * 2, 0), 0.8f/*radius*/, transform.forward, out hit, fNPC_DISTANCE_CHECK/*distance*/)
            || Physics.Raycast(transform.position + new Vector3(0, fHEAD_OFFSET, 0), transform.forward, out hit, fNPC_DISTANCE_CHECK))
        {
            if (hit.transform.GetComponent<NPCEntity>() && !bIsInteracting)
            {
                //Debug.Log("in range");
                if (Input.GetButtonDown("Interact"))
                {
                    bIsInteracting = true;
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
                if (bIsInteracting)
                {
                    bIsInteracting = false;
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

    /// Health System
    public void TakeDamage(int _damage)
    {
        if (!bIsInvulnerable)
        {
            IsInvulnerable(true);
            iCurrentHitPoints -= _damage;
            StartCoroutine(ChangeBoolAfter((bool b) => { IsInvulnerable(b);}, false, fINVULNERABILITY_TIME));
        }
        if(iCurrentHitPoints <= 0)
        {
            Die();
        }
    }
    public void Die()
    {
        bIsAlive = false;
        gameObject.SetActive(false);
    }

    public void IsInvulnerable(bool _invulnerable)
    {
        bIsInvulnerable = _invulnerable;
    }
    public IEnumerator ChangeBoolAfter(System.Action<bool> _callBack, bool _setBool, float _time)
    {
        yield return new WaitForSeconds(_time);
        _callBack(_setBool);
        //StopAllCoroutines();
    }
}
