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
    [SerializeField]
    float MaxDistance;

    Vector3 StartPos;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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
            Hit();
        }
    }

    private void Hit()
    {
        if (!isRetrievable)
        {
            GameObject effect = Instantiate(HitEffect, transform.position, Quaternion.identity);
            Destroy(effect, 0.4f);
            Destroy(gameObject);
        }
    }

    public void Shoot(Vector2 target)
    {
        rb.AddForce(target);
    }
}
