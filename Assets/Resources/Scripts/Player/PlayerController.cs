using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour, IHittable, ISaveable
{
    structGameSavePlayer structPlayerSaveData;

    const float fHEAD_OFFSET = 1f;
    const float fNPC_DISTANCE_CHECK = 0.8f;
    const float fDISTANCE_TO_GROUND = 0.1f;
    const float fINVULNERABILITY_TIME = 0.5f;
    const float fSTUN_TIME = 0.4f;
    const float fSPRINT_STAMINA_COST = 10f; // is multipleid by deltaTime
    const float fATTACK_STAMINA_COST = 5f;
    const float fSHIELD_STAMINA_COST = 10f;
    const float fSTAMINA_RECOVER_START_TIME = 0.1f;
    const float fSTAMINA_RECOVERY_RATE = 25f; // is multipleid by deltaTime
    const float fSPEED_DIVISION = 0.7f;

    Rigidbody rbody;
    Animator anim;

    public float fMaxHitPoints;
    [HideInInspector]
    public float fCurrentHitPoints;

    public float fMaxStamina;
    [HideInInspector]
    public float fCurrentStamina;
    float fStaminaTimeCounter = 0;
    bool bStaminaUsed;

    public float fSpeed;
    public float fJumpForce;
    public float sSpeedMultiplier = 1.5f;
    float horizontal;
    float vertical;
    Vector3 lastFacinDirection;

    public bool bIsAlive;
    [HideInInspector]
    public bool bIsShielding;
    bool bIsSprinting;
    bool bCanSprint = true;
    [HideInInspector]
    public bool bSwordEquipped = false;
    [HideInInspector]
    public bool bCanAttack = true;
    [HideInInspector]
    public bool bIsAttacking = false;
    private bool bIsInteracting = false;
    private bool bIsInvulnerable;
    int iAttackCombo = -1;
    bool bIsStun;

    public UnityEvent OnReciveDamageUI;
    public UnityEvent OnStaminaChangeUI;

    public Inventory playerInventory;
    public int iStartInventorySize;

    void Awake()
    {
        // controller = GetComponent<CharacterController>();
        rbody = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        fCurrentHitPoints = fMaxHitPoints;
        fCurrentStamina = fMaxStamina;
        bIsAlive = true;

        playerInventory = new Inventory(iStartInventorySize);
    }
    void Update()
    {
        
        if (GameController.inPlayMode)
        {
            if (bIsAlive && !bIsStun)
            {
                StaminaCheck();
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
            if (bIsAlive && !bIsStun)
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

            if (Input.GetButton("Sprint"))
            {
                if (!bIsShielding && bCanSprint)
                {
                    if(fCurrentStamina > fSPRINT_STAMINA_COST * Time.deltaTime)
                    {
                        fCurrentStamina -= fSPRINT_STAMINA_COST * Time.deltaTime;
                        bIsSprinting = true;
                    }
                    else
                    {
                        bIsSprinting = false;
                        bCanSprint = false;
                    }
                }
            }
            
            else
            {
                if (fCurrentStamina > 5f)
                {
                    bCanSprint = true;
                }
                bIsSprinting = false;
            }

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
            if(lastFacinDirection != Vector3.zero) // 
            {
                transform.forward = lastFacinDirection.normalized;
            }
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
                rbody.velocity = movementVector.normalized * fSPEED_DIVISION * fSpeed * Time.fixedDeltaTime;
            else
                rbody.velocity = movementVector.normalized * fSpeed * Time.fixedDeltaTime;
        }
    }
    void UseShield()
    {   
        if (Input.GetButton("Shield"))
        {
            if(fCurrentStamina > fSHIELD_STAMINA_COST)
            {
                bIsShielding = true;
                bIsSprinting = false;
            }
            else
            {
                bIsShielding = false;
            }
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
                if(iAttackCombo > 0 && bCanAttack && fCurrentStamina > fATTACK_STAMINA_COST)
                {
                    fCurrentStamina -= fATTACK_STAMINA_COST;
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
            // TODO: can make it bit more good
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
            for (int i = 0; i < playerInventory.lstItems.Count; i++)
            {
                if (playerInventory.lstItems[i].sItemName == _collidedItemContainer.item.sItemName)
                {
                    if (playerInventory.lstItems[i].iAmount < playerInventory.lstItems[i].iStackLimit)
                    {
                        playerInventory.lstItems[i].UpdateAmount(+1); // increase stack amount by 1
                        _bItemAlreadyInventory = true;
                        _collidedItemContainer.DestroySelf();
                        break;
                    }
                }
            }
            if (!_bItemAlreadyInventory)
            {
                if (playerInventory.lstItems.Count < playerInventory.iInventorySize)
                {
                    Item _newItem = new Item(_collidedItemContainer.item.GetItem());
                    playerInventory.AddItem(_newItem);
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
            if (playerInventory.lstItems.Count < playerInventory.iInventorySize)
            {
                Item _newItem = new Item(_collidedItemContainer.item.GetItem());
                playerInventory.AddItem(_newItem);
                _collidedItemContainer.DestroySelf();
            }
            else
            {
                Debug.Log("Inventory is Full");
            }
        }
        PopupUIManager.Instance.inventoryPopup.UpdateInventoryUI(playerInventory);
    }
    public void DisablePlayerMoveActions()
    {
        rbody.velocity = new Vector3(0, rbody.velocity.y, 0);
        anim.SetTrigger("idle");
        anim.SetFloat("moveVelocity", 0f);
        anim.SetBool("isShielding", false);
    }

    public bool IsInteracting()
    {
        return bIsInteracting;
    }
    /// Health System
    public void TakeDamage(int _damage)
    {
        if (!bIsInvulnerable)
        {
            bIsInvulnerable = true;
            fCurrentHitPoints -= _damage;
            OnReciveDamageUI.Invoke();
            StartCoroutine(HelperFunctions.ChangeBoolAfter((bool b) => { bIsInvulnerable = b;}, false, fINVULNERABILITY_TIME));
        }
        if(fCurrentHitPoints <= 0)
        {
            Die();
        }
    }
    public void Die()
    {
        bIsAlive = false;
        gameObject.SetActive(false);
    }
    public bool IsInvulnerable()
    {
        return bIsInvulnerable;
    }

    public void HealthCheck()
    {
        if (fCurrentHitPoints > fMaxHitPoints)
            fCurrentHitPoints = fMaxHitPoints;

    }

    public void StaminaCheck()
    {
        if(bIsShielding || bIsSprinting || bIsAttacking)
        {
            OnStaminaChangeUI.Invoke();
            bStaminaUsed = true;
        }
        if (!bCanSprint)
        {
            if((fCurrentStamina >= fMaxStamina / 2f))
            {
                bCanSprint = true;
            }
        }

        if(!bIsSprinting && !bIsAttacking && !bIsShielding)
        {
            if((int)fCurrentStamina <= (int)fMaxStamina + 10)
            {
                OnStaminaChangeUI.Invoke();
                if (bStaminaUsed)
                {
                    fStaminaTimeCounter += Time.deltaTime;
                    if(fStaminaTimeCounter >= fSTAMINA_RECOVER_START_TIME)
                    {
                        bStaminaUsed = false;
                        fStaminaTimeCounter = 0;
                    }
                }
                if(fStaminaTimeCounter <= 0)
                {
                    fCurrentStamina += fSTAMINA_RECOVERY_RATE * Time.deltaTime;
                    if(fCurrentStamina > fMaxStamina)
                    {
                        fCurrentStamina = fMaxStamina;
                    }
                }
            }
        }
    }
    public void Knockback(Vector3 _sourcePosition, float _pushForce)
    {
        // Stops for a hit time to play the hit animation and then move

        Stun();

        if (!bIsInvulnerable)
        {
            fCurrentStamina -= _pushForce * 2f;
            Vector3 pushForce = transform.position - _sourcePosition;
            pushForce.y = 0;
            //transform.forward = -pushForce.normalized;
            rbody.AddForce(pushForce.normalized * _pushForce, ForceMode.Impulse);
        }
    }
    public void Stun()
    {
        rbody.velocity = HelperFunctions.VectorZero(rbody);
        anim.SetFloat("moveVelocity", 0f);
        bIsStun = true;
        if (bIsStun)
        {
           StartCoroutine(HelperFunctions.ChangeBoolAfter((bool b)=> { bIsStun = b; }, false, fSTUN_TIME)); // Replace stun time with stun animation
        }
    }

    // Inventory
    public void UpdateInventory(Inventory _inventory)
    {
        playerInventory = _inventory;
        PopupUIManager.Instance.inventoryPopup.UpdateInventoryUI(playerInventory);
    }
    public Inventory GetInventory()
    {
        return playerInventory;
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

    public void SaveAllData(SaveData _saveData)
    {
        structGameSavePlayer _playerSaveData = new structGameSavePlayer();
        _playerSaveData.fCurrentHitPoints = fCurrentHitPoints;
        _playerSaveData.fCurrentStamina = fCurrentStamina;
        _playerSaveData.tPosition = new float[3] { transform.position.x, transform.position.y, transform.position.z};
        _playerSaveData.tRotation = new float[3] {lastFacinDirection.x, lastFacinDirection.y, lastFacinDirection.z};
        _playerSaveData.playerInventory = playerInventory.GetInventory();
        //_playerSaveData.tRotation = new float[3] { transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z};

        _saveData.playerSaveData = _playerSaveData;
    }

    public void LoadSaveData(SaveData _saveData)
    {
        structPlayerSaveData = new structGameSavePlayer();
        structPlayerSaveData = _saveData.playerSaveData;

        fCurrentHitPoints = structPlayerSaveData.fCurrentHitPoints;
        fCurrentStamina = structPlayerSaveData.fCurrentStamina;

        transform.position = new Vector3(structPlayerSaveData.tPosition[0], structPlayerSaveData.tPosition[1], structPlayerSaveData.tPosition[2]); // 0 = x, 1 = y, 2 = z
        lastFacinDirection = new Vector3(structPlayerSaveData.tRotation[0], structPlayerSaveData.tRotation[1], structPlayerSaveData.tRotation[2]); // 0 = x, 1 = y, 2 = z

        playerInventory = new Inventory(structPlayerSaveData.playerInventory.iInventorySize);
        playerInventory.SetInventory(structPlayerSaveData.playerInventory);
        PopupUIManager.Instance.inventoryPopup.UpdateInventoryUI(playerInventory);

        OnReciveDamageUI.Invoke();
        OnStaminaChangeUI.Invoke();
    }
}

