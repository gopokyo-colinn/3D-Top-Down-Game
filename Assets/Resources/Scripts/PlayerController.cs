using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
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
    int attackCombo = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        DirectionalMovement();
        UseShield();
        DrawSword();
        SwordAttacks();
    }

    void DirectionalMovement()
    {
        horizontal = -Input.GetAxis("Horizontal");
        vertical = -Input.GetAxis("Vertical");

        anim.SetBool("isSprinting", isSprinting);
        anim.SetFloat("walkDir", (horizontal * vertical > 0) ? -1 : 1) ;

        if (horizontal != 0 || vertical != 0)
        {
            anim.SetFloat("moveVelocity", 1f);

            if (Input.GetButton("Sprint") && !isShielding)
                isSprinting = true;
            else
                isSprinting = false;

            //if (!isShielding)
            //    if (Mathf.Abs(horizontal) > 0.2f || Mathf.Abs(vertical) > 0.2f)
            //        lastFacinDirection = new Vector3(horizontal, 0f, vertical);
            lastFacinDirection = new Vector3(horizontal, 0f, vertical);
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
        if (Input.GetButton("Attack"))
        {
            anim.SetBool("draw_Sword", true);
            swordEquipped = true;
        }
        if(Input.GetKeyDown(KeyCode.T))
        {
            anim.SetBool("draw_Sword", false);
            swordEquipped = false;
        }
    }

    void SwordAttacks()
    {
        if (swordEquipped && attackCombo == 0)
        {
            if (Input.GetButtonDown("Attack"))
            {
                anim.SetTrigger("attack1");
            }
        }
    }
    
}
