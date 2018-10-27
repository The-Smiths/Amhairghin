using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    public CharacterController2D controller;
    public float Speed = 40f;

    private float m_hInput = 0;
    private bool m_isJumping = false;
    private bool m_isCrouching = false;

    private void Update()
    {
        m_hInput = Input.GetAxisRaw("Horizontal");
        m_isJumping = Input.GetButtonDown("Jump");
        m_isCrouching = Input.GetButton("Crouch");
    }

    private void FixedUpdate()
    {
        controller.Move(m_hInput * Speed * Time.fixedDeltaTime, m_isCrouching, m_isJumping);
        if (m_isJumping)
            Debug.Log("SPACE PRESSED");
    }
}
