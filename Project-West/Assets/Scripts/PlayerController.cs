using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    public enum PlayerState
    {
        Undetacted,
        Detected,
        Hidden
    }

    public PlayerState state;
    public float moveSpeed = 5f;
    public Rigidbody2D rb;

    GameObject hideableobjs;
    
    Vector2 movement;
    Animator anim;
    bool isfaceRight;
    public bool CanHideInObj;

    private void Awake()
    {
        state = PlayerState.Undetacted;
        rb = this.GetComponent<Rigidbody2D>();
        anim = this.GetComponentInChildren<Animator>();

        CanHideInObj = false;
        isfaceRight = true;
    }

    void Start()
    {
       
    }

    void Update()
    {
        bool InteractPressed = false;
        if(state != PlayerState.Hidden)
        {
            movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
            AnimUpdate();
        }

        if(state == PlayerState.Undetacted)
        {
            if (CanHideInObj)
            {
                if (!InteractPressed && Input.GetButtonDown("Interact"))
                {
                    Hide(hideableobjs);
                    InteractPressed = true;
                }
            }
        }

        if (state == PlayerState.Hidden)
        {
            CanHideInObj = false;
            if (!InteractPressed && Input.GetButtonDown("Interact"))
            {
                UnHide(hideableobjs);
                InteractPressed = true;
            }
        }
    }

    private void FixedUpdate()
    {
            rb.MovePosition(rb.position + movement * moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Hide"))
        {
            CanHideInObj = true;
            hideableobjs = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Hide"))
        {
            CanHideInObj = false;
        }
    }

    private void AnimUpdate()
    {
        anim.SetFloat("HorizontalSpeed", Math.Abs(movement.x));
        anim.SetFloat("Vertical", movement.y);
        anim.SetFloat("Speed", movement.sqrMagnitude);
        if (movement.x > 0 && !isfaceRight)
        {
            Flip();
        }

        if (movement.x < 0 && isfaceRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        Vector3 currentScale = gameObject.transform.localScale;
        currentScale.x *= -1;
        gameObject.transform.localScale = currentScale;

        isfaceRight = !isfaceRight;
    }

    private void Hide(GameObject inObject)
    {
        state = PlayerState.Hidden;
        Debug.Log("Hide!");
        if (inObject.GetComponent<HidableObject>())
        {
            inObject.GetComponent<HidableObject>().ishideable = false;
        }
        CanHideInObj = false;
        GetComponentInChildren<SpriteRenderer>().enabled = false;
        GetComponent<CapsuleCollider2D>().enabled = false;
        GetComponent<Revolver>().enabled = false;
    }

    private void UnHide(GameObject inObject)
    {
        state = PlayerState.Undetacted;
        if (inObject.GetComponent<HidableObject>())
        {
            inObject.GetComponent<HidableObject>().ishideable = true;
        }
        CanHideInObj = true;
        GetComponentInChildren<SpriteRenderer>().enabled = true;
        GetComponent<CapsuleCollider2D>().enabled = true;
        GetComponent<Revolver>().enabled = true;
    }

}
