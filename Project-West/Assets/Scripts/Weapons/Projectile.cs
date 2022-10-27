using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Rigidbody2D rb;

    [SerializeField]
    GameObject HitEffect;

    [SerializeField]
    bool isRetrievable;
    bool used;
    [SerializeField]
    float MaxDistance;

    Vector3 StartPos;

    private void Awake()
    { 
        rb = GetComponent<Rigidbody2D>();
        if (isRetrievable)
        {
            used = false;
        }
    }

    void Start()
    {
        StartPos = this.transform.position;
    }

    void Update()
    {
        if (Vector3.Distance(this.transform.position,StartPos) > MaxDistance)
        {
            Hit();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
        {
            if (!isRetrievable)
            {
                Hit();
            }else if (isRetrievable)
            {
                if (!used)
                {
                    if (!collision.gameObject.CompareTag("Knife"))
                    {
                        Hit();
                    }
                }
            }
        }

        if (isRetrievable)
        {
            if (used)
            {
                if (collision.gameObject.CompareTag("Player"))
                {
                    Retrieve(collision.gameObject);
                }
            }
        }
    }

    private void Hit()
    {
        if (!isRetrievable)
        {
            if (HitEffect)
            {
                GameObject effect = Instantiate(HitEffect, transform.position, Quaternion.identity);
                Destroy(effect, 0.4f);
            }
            Destroy(gameObject);
        }
        else
        {
            rb.velocity = Vector2.zero;
            used = true;
        }
    }

    private void Retrieve(GameObject Owner)
    {
        Owner.GetComponent<ThrowKnives>().GetKnife();
        Destroy(gameObject);
    }

    public void Shoot(Vector2 target)
    {
        rb.AddForce(target);
    }
}
