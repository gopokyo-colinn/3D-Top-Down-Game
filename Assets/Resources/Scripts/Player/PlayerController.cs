using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
    const float fSpeedDivision = 0.7f;
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

    public UnityEvent OnReciveDamage;
    public Inventory myInventory;
    public int iInventorySize;

    void Awake()
    {
        // controller = GetComponent<CharacterController>();
        rbody = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        iCurrentHitPoints = iMaxHitPoints;
        bIsAlive = true;
        myInventory = new Inventory(iInventorySize);
    }
    void Update()
    {
        if (GameController.inPlayMode)
        {
            if (bIsAlive)
            {
                GetDirectionalInput();
                UseShield();
                DrawSword();
                SwordAttacks();
                CheckAheadForColliders();
                Jumping();
            }
        }
    }
    void FixedUpdate()
    {
        if (GameController.inPlayMode)
        {
            if (bIsAlive)
            {
                if (Grounded())
                {
                    DirectionalMovement();
                }
                else // for natural jumping, can't change direction in mid air
                {
                    JumpControlling();
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
        if (!bIsAttacking)
        {
            if(lastFacinDirection != Vector3.zero)
                transform.forward = lastFacinDirection.normalized;
        }

        Vector3 movementVector = new Vector3(horizontal, rbody.velocity.y, vertical);

        if (movementVector.magnitude > 0.1f)
        { 
            if (bIsSprinting)
            {
                if (bIsAttacking)
                    bIsSprinting = false;
                else
                    rbody.velocity = movementVector.normalized * fSpeed * sSpeedMultiplier * Time.fixedDeltaTime;
            }
            else if (bIsShielding || bIsAttacking)
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
    void JumpControlling()
    {
        if (bIsSprinting)
            rbody.velocity = new Vector3(horizontal * fSpeed * (sSpeedMultiplier / 1.5f) * Time.fixedDeltaTime, rbody.velocity.y, vertical * fSpeed * (sSpeedMultiplier / 1.5f) * Time.fixedDeltaTime);
        else
            rbody.velocity = new Vector3(horizontal * fSpeed * Time.fixedDeltaTime, rbody.velocity.y, vertical * fSpeed * Time.fixedDeltaTime);
    }
    public void CheckAheadForColliders()
    {
        RaycastHit hit;
        // now the boxcast is as same as in the gizmos below
        if(Physics.BoxCast(transform.position + (transform.forward * -1f), transform.localScale, transform.forward, out hit, transform.rotation, 1f))
        //|| Physics.Raycast(transform.position + new Vector3(0, fHEAD_OFFSET, 0), transform.forward, out hit, fNPC_DISTANCE_CHECK))
        {
            if (hit.collider)
            {
                ItemContainer _itemContainer = hit.transform.GetComponent<ItemContainer>();
                NPCEntity _npc = hit.transform.GetComponent<NPCEntity>();

                if (Input.GetButtonDown("Interact"))
                {
                    if (_npc)
                    {
                        CheckForNPC(_npc);
                    }
                    if (_itemContainer)
                    {
                        CheckForItems(_itemContainer);
                    }
                }
            }
        }
    }
    public void CheckForNPC(NPCEntity _collidedNPC)
    {
        if (!bIsInteracting)
        {
            bIsInteracting = true;
            // can make it bit more good
            var targetRotation = Quaternion.LookRotation(_collidedNPC.transform.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1);

            _collidedNPC.SetDialog();
            _collidedNPC.LookAtTarget(transform);
            DisablePlayerMoveActions();
            GameController.inPlayMode = false;
        }
        else
        {
            if (bIsInteracting)
            {
                bIsInteracting = false;
            }
        }
    }
    private void CheckForItems(ItemContainer _collidedItemContainer)
    {
        if (_collidedItemContainer.item.isStackable)
        {

            bool _bItemAlreadyInventory = false;
            for (int i = 0; i < myInventory.lstItems.Count; i++)
            {
                if (myInventory.lstItems[i].sItemName == _collidedItemContainer.item.sItemName)
                {
                    if (myInventory.lstItems[i].iAmount < myInventory.lstItems[i].iStackLimit)
                    {
                        myInventory.lstItems[i].iAmount++;
                        _bItemAlreadyInventory = true;
                        _collidedItemContainer.DestroySelf();
                        break;
                    }
                }
            }
            if (!_bItemAlreadyInventory)
            {
                if (myInventory.lstItems.Count < myInventory.iInventoryLimit)
                {
                    Item _newItem = new Item(_collidedItemContainer.item);
                    myInventory.AddItem(_newItem);
                    _collidedItemContainer.DestroySelf();
                }
                else
                {
                    Debug.Log("Inventory is Full");
                }
            }
        }
        else
        {
            if (myInventory.lstItems.Count < myInventory.iInventoryLimit)
            {
                Item _newItem = new Item(_collidedItemContainer.item);
                myInventory.AddItem(_newItem);
                _collidedItemContainer.DestroySelf();
            }
            else
            {
                Debug.Log("Inventory is Full");
            }
        }
        PopupUIManager.Instance.inventoryPopup.UpdateInventoryUI(myInventory);
    }
    public void DisablePlayerMoveActions()
    {
        rbody.velocity = new Vector3(0, rbody.velocity.y, 0);
        anim.SetTrigger("idle");
        anim.SetFloat("moveVelocity", 0f);
        anim.SetBool("isShielding", false);
    }
    /// Health System
    public void TakeDamage(int _damage)
    {
        if (!bIsInvulnerable)
        {
            IsInvulnerable(true);
            iCurrentHitPoints -= _damage;
            OnReciveDamage.Invoke();
            StartCoroutine(HelperFunctions.ChangeBoolAfter((bool b) => { IsInvulnerable(b);}, false, fINVULNERABILITY_TIME));
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

    public void HealthCheck()
    {
        if (iCurrentHitPoints > iMaxHitPoints)
            iCurrentHitPoints = iMaxHitPoints;

    }
    // Inventory
    public void UpdateInventory(Inventory _inventory)
    {
        myInventory = _inventory;
        PopupUIManager.Instance.inventoryPopup.UpdateInventoryUI(myInventory);
    }
    public Inventory GetInventory()
    {
        return myInventory;
    }

    /// Triggers

    private void OnTriggerEnter(Collider other)
    {
        //ItemContainer _itemContainer = other.gameObject.GetComponent<ItemContainer>();
        //if (_itemContainer)
        //{
        //    //if (Input.GetButtonDown("Interact"))
        //    {
        //        myInventory.AddItem(_itemContainer.item);
        //        _itemContainer.DestroySelf();
        //        PopupUIManager.Instance.inventoryPopup.UpdateInventoryUI(myInventory);
        //    }
        //}
    }

    ///// Gizmos
    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.red;
        //Gizmos.DrawWireCube(transform.position + transform.forward * 0.5f, transform.localScale);
    }
}

