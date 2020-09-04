using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    const float HEAD_OFFSET = 1f;
    const float NPC_DISTANCE_CHECK = 0.8f;

    CharacterController controller;
    Animator anim;
    public float speed;
    public float speedMultiplier = 1.5f;
    const float shieldWalkSpeedDivision = 0.5f;

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
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.Instance.inPlayMode)
        {
            DirectionalMovement();
            UseShield();
            DrawSword();
            SwordAttacks();
            CheckForNPC();
        }
        else
        {
            if (isInteracting)
                DisablePlayerMoveActions();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("TestScene2");
        }
            
    }

    void DirectionalMovement()
    {
        horizontal = -Input.GetAxis("Horizontal");
        vertical = -Input.GetAxis("Vertical");

        anim.SetBool("isSprinting", isSprinting);
        //anim.SetFloat("walkDir", (horizontal * vertical > 0) ? -1 : 1) ;

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

        if (isSprinting)
            controller.Move(new Vector3(horizontal, 0, vertical).normalized * speed * speedMultiplier * Time.deltaTime /** Time.deltaTime*/);
        else if(isShielding)
            controller.Move(new Vector3(horizontal, 0, vertical).normalized * shieldWalkSpeedDivision * speedMultiplier * Time.deltaTime /** Time.deltaTime*/);
        else
            controller.Move(new Vector3(horizontal, 0, vertical).normalized * speed * Time.deltaTime /** Time.deltaTime*/);
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
            else
            {
                isAttacking = false;
            }
        } 
    }

    public void CheckForNPC()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position + new Vector3(0, HEAD_OFFSET,  0), transform.forward, out hit , NPC_DISTANCE_CHECK))
        {
            if (hit.transform.GetComponent<NPCEntity>() && !isInteracting)
            {
                if (Input.GetButtonDown("Interact"))
                {
                    NPCEntity _collidedNPC = hit.transform.GetComponent<NPCEntity>();
                    isInteracting = true;
                    PopupUIManager.Instance.dialogBoxPopup.setDialogText(_collidedNPC.dialogLines);
                    _collidedNPC.LookAtTarget(transform);
                    controller.Move(new Vector3(0, 0, 0));
                    GameController.Instance.inPlayMode = false;
                    DirectionalMovement();
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
        anim.SetTrigger("idle");
        anim.SetFloat("moveVelocity", 0f);
        anim.SetBool("isShielding", false);
    }


}
