using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour, IHittable, ISaveable
{
    protected static PlayerController instance;
    public static PlayerController Instance { get { return instance; } }

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

    private bool bIsAlive;
    private bool bIsShielding;
    private bool bIsSprinting;
    private bool bCanSprint = true;
    private bool bDrawPrimaryWeapon;
    private bool bPrimaryWeaponEquipped;
    private bool bShieldEquipped;
    private bool bCanAttack = true;
    private bool bIsAttacking;
    private bool bIsInteracting;
    private bool bIsInvulnerable;
    private bool bIsStun;
    
    int iAttackCombo = -1;

    public UnityEvent OnReciveDamageUI;
    public UnityEvent OnStaminaChangeUI;

    public Inventory playerInventory;
    public int iStartInventorySize;

    PlayerEquipmentManager pEquimentManager;

    void Awake()
    {
        instance = this;

        rbody = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        fCurrentHitPoints = fMaxHitPoints;
        fCurrentStamina = fMaxStamina;
        bIsAlive = true;
        playerInventory = new Inventory(iStartInventorySize);
        pEquimentManager = GetComponent<PlayerEquipmentManager>();
    }
    void Update()
    {
        if (GameController.inPlayMode)
        {
            if (bIsAlive && !bIsStun)
            {
                if(Grounded())
                    GetDirectionalInput();

                StaminaCheck();
                UseShield();
                DrawSheathPrimaryWeapon();
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

            //It is for keeping the direction while shielding
            //if (!bIsShielding)
            //{
            //    if (Mathf.Abs(horizontal) > 0.2f || Mathf.Abs(vertical) > 0.2f)
            //        lastFacinDirection = new Vector3(horizontal, 0f, vertical);
            //}
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
    void DrawSheathPrimaryWeapon()
    {
        if (pEquimentManager.primaryWeapon != null)
        {
            if (!bDrawPrimaryWeapon)
            {
                if (Input.GetButton("Attack"))
                {
                    bDrawPrimaryWeapon = true;
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.T))
                {
                    SheathWeapon();
                }
            }
        }
        anim.SetBool("draw_Weapon", bDrawPrimaryWeapon);
    }
    void SheathWeapon(bool _bIsRemoved = false)
    {
        if(_bIsRemoved)
            anim.SetTrigger("weapon_removed");
        bDrawPrimaryWeapon = false;
        bIsAttacking = false;
        iAttackCombo = -1;
    }
    void SwordAttacks()
    {
        if (bDrawPrimaryWeapon)
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
    public void Jumping()
    {
        /// For Jumping
        if (Grounded())
        {
            if (Input.GetKeyDown(KeyCode.Space))
                rbody.AddForce(new Vector3( rbody.velocity.x, fJumpForce, rbody.velocity.z) * Time.fixedDeltaTime, ForceMode.Impulse);
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

            _collidedNPC.SetDialogWithQuest();
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
        if (_collidedItemContainer.item.bIsStackable)
        {
            bool _bItemAlreadyInventory = false;
            for (int i = 0; i < playerInventory.lstItems.Count; i++)
            {
                if (playerInventory.lstItems[i].sItemName == _collidedItemContainer.item.sItemName)
                {
                    if (playerInventory.lstItems[i].iQuantity < playerInventory.lstItems[i].iStackLimit)
                    {
                        playerInventory.lstItems[i].SetQuantity(playerInventory.lstItems[i].iQuantity + 1); // increase stack amount by 1
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
                    Item _newItem = new Item(_collidedItemContainer.item);
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
                Item _newItem = new Item(_collidedItemContainer.item);
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

   
    /// Health System
    public void ApplyKnockback(Vector3 _sourcePosition, float _pushForce)
    {
        // Stops for a hit time to play the hit animation and then move
        Stun();
        if (!bIsInvulnerable)
        {
            fCurrentStamina -= _pushForce * 1.5f;
            OnStaminaChangeUI.Invoke();
            Vector3 pushForce = transform.position - _sourcePosition;
            pushForce.y = 0;
            //transform.forward = -pushForce.normalized;
            rbody.AddForce(pushForce.normalized * _pushForce, ForceMode.Impulse);
        }
    }
    public void ApplyDamage(float _damage)
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
        //TODO: Add a beath animation and remove collisions
        //gameObject.SetActive(false); // deactivating it or destroying can coz some loading bugs
    }
    public void HealthCheck()
    {
        if (fCurrentHitPoints > fMaxHitPoints)
            fCurrentHitPoints = fMaxHitPoints;

    }
    public void StaminaCheck()
    {
        if(fCurrentStamina < 0)
        {
            fCurrentStamina = 0;
        }

        if(bIsSprinting || bIsAttacking)
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

        if(!bIsSprinting && !bIsAttacking)
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
                    if (bIsShielding)
                    {
                        fCurrentStamina += (fSTAMINA_RECOVERY_RATE / 5) * Time.deltaTime;
                    }
                    else
                    {
                        fCurrentStamina += fSTAMINA_RECOVERY_RATE * Time.deltaTime;
                    }

                    if(fCurrentStamina > fMaxStamina)
                    {
                        fCurrentStamina = fMaxStamina;
                    }
                }
            }
        }
    }
    public void Stun()
    {
        //rbody.velocity = HelperFunctions.VectorZero(rbody);
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
        // TODO: Update Player about the sword that its gone now, make make sword to null;
        playerInventory = _inventory;
        PopupUIManager.Instance.inventoryPopup.UpdateInventoryUI(playerInventory);
    }
    public Inventory GetInventory()
    {
        return playerInventory;
    }
    /// Bools Getter And Setters
    public bool Grounded()
    {
        // use a spherecast instead
        return Physics.Raycast(transform.position, Vector3.down, fDISTANCE_TO_GROUND);
    }
    public bool IsInteracting()
    {
        return bIsInteracting;
    }
    public bool IsInvulnerable()
    {
        return bIsInvulnerable;
    }
    public bool IsAlive()
    {
        return bIsAlive;
    }
    public bool IsAttacking()
    {
        return bIsAttacking;
    }
    public bool IsSwordEquipped()
    {
        return bDrawPrimaryWeapon;
    }
    public void SetPrimaryWeaponEquipped(Item _swordToEquip)
    {
        // TODO: Set player animtaion to deafult on changing new weapon or place the new weapon in its hand according to drawWeaponBool
        ItemContainer _newWeapon = null;
        if (_swordToEquip != null)
        {
            SheathWeapon(true);
            _newWeapon = Instantiate(_swordToEquip.GetItemPrefab(), pEquimentManager.phPrimaryWeaponUnEquipped);
            _newWeapon.SetItemEquipable();
            pEquimentManager.SetPrimaryWeapon(_newWeapon.gameObject);
        }
        else // when there is no weapon equipped
        {
            SheathWeapon(true);
            pEquimentManager.SetPrimaryWeapon(null);
        }
    }
    public void SetSecondaryWeaponEquipped(Item _swordToEquip)
    {
        

    }
    public void SetShieldEquipped(Item _swordToEquip)
    {

    }
    public void SetCanAttack(bool _bCanAttack)
    {
        bCanAttack = _bCanAttack;
    }
    public Animator GetAnimator()
    {
        return anim;
    }
    ///// Saving And Loading Data
    public void SaveAllData(SaveData _saveData)
    {
        structGameSavePlayer _playerSaveData = new structGameSavePlayer();
        _playerSaveData.fCurrentHitPoints = instance.fCurrentHitPoints;
        _playerSaveData.fCurrentStamina = instance.fCurrentStamina;
        _playerSaveData.tPosition = new float[3] { instance.transform.position.x, instance.transform.position.y, instance.transform.position.z};
        _playerSaveData.tRotation = new float[3] {lastFacinDirection.x, lastFacinDirection.y, lastFacinDirection.z};
        _playerSaveData.playerInventory = instance.playerInventory.GetInventory();
        //_playerSaveData.tRotation = new float[3] { transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z};

        _saveData.playerSaveData = _playerSaveData;
    }

    public void LoadSaveData(SaveData _saveData)
    {
        structPlayerSaveData = new structGameSavePlayer();
        structPlayerSaveData = _saveData.playerSaveData;

        instance.fCurrentHitPoints = structPlayerSaveData.fCurrentHitPoints;
        instance.fCurrentStamina = structPlayerSaveData.fCurrentStamina;

        instance.transform.position = new Vector3(structPlayerSaveData.tPosition[0], structPlayerSaveData.tPosition[1], structPlayerSaveData.tPosition[2]); // 0 = x, 1 = y, 2 = z
        instance.lastFacinDirection = new Vector3(structPlayerSaveData.tRotation[0], structPlayerSaveData.tRotation[1], structPlayerSaveData.tRotation[2]); // 0 = x, 1 = y, 2 = z

        instance.playerInventory = new Inventory(structPlayerSaveData.playerInventory.iInventorySize);
        instance.playerInventory.SetInventory(structPlayerSaveData.playerInventory);
        PopupUIManager.Instance.inventoryPopup.UpdateInventoryUI(instance.playerInventory);

        instance.OnReciveDamageUI.Invoke();
        instance.OnStaminaChangeUI.Invoke();
    }
}

