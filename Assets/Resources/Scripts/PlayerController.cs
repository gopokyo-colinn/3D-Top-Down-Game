﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    CharacterController controller;
    Animator anim;
    public float speed;
    public float speedMultiplier = 1.5f;

    float horizontal;
    float vertical;
    Vector3 lastFacinDirection;

    bool isSprinting;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        DirectionalMovement();
    }

    void DirectionalMovement()
    {
        horizontal = -Input.GetAxis("Horizontal");
        vertical = -Input.GetAxis("Vertical");

        anim.SetBool("isSprinting", isSprinting);

        if (horizontal != 0 || vertical != 0)
        {
            anim.SetFloat("moveVelocity", 1f);

            if (Input.GetKey(KeyCode.LeftShift))
                isSprinting = true;
            else
                isSprinting = false;

            if (Mathf.Abs(horizontal) > 0.2f || Mathf.Abs(vertical) > 0.2f)
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
        else
            controller.Move(new Vector3(horizontal, 0, vertical).normalized * speed * Time.deltaTime /** Time.deltaTime*/);
    }
    
}