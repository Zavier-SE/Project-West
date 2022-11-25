using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class CompanionController : MonoBehaviour
{
    public enum CompanionState
    {
        Follow,
        Hidden,
        Attack
    }
    private CompanionState state;

    [SerializeField]
    private Transform playerRf;
    private PlayerController.PlayerState playerState;
    [SerializeField]
    private float speed;
    [SerializeField]
    private float nextWayPointDistance;
    [SerializeField]
    private float FollowRadius = 2f;

    [SerializeField]
    private Transform target;
    private Vector2 targetPos;
    GameObject targetHideable = null;
    GameObject targetEnemy = null;

    Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath;

    Seeker seeker;
    Rigidbody2D rb;

    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        target = playerRf;
        targetPos = target.position;
        state = CompanionState.Follow;

        InvokeRepeating("UpdatePath", 0f, .5f);
    }

    void Update()
    {
        playerState = playerRf.GetComponent<PlayerController>().getPlayerState();
        if (playerState == PlayerController.PlayerState.Detected)
        {
            if(state != CompanionState.Attack)
            {
                state = CompanionState.Attack;
            }
            if(state == CompanionState.Attack)
            {
                //Attack nearby Enemies
                FindNearstEnemy();
            }
        }

        if(playerState == PlayerController.PlayerState.Hidden)
        {
            //Hide
            if(state != CompanionState.Hidden)
            {
                FindHidable();
            }
        }

        if(playerState == PlayerController.PlayerState.Undetacted)
        {
            //UnHide
            if(state == CompanionState.Hidden)
            {
                UnHide(targetHideable);
            }
            if (state != CompanionState.Follow)
            {
                state = CompanionState.Follow;
            }
            //Follow
            if (state == CompanionState.Follow)
            {
                if (distenceTo(playerRf) > FollowRadius)
                {
                    FollowPlayer();
                }
                else
                {
                    path = null;
                }
            }
        }

        if (!reachedEndOfPath)
        {
            MoveAlongPath();
        }
        else
        {
            Invoke("MoveAlongPath", Random.Range(0, 3.0f));
        }
    }

    void FollowPlayer()
    {
        Vector2 dirToPlayer = (playerRf.position - transform.position).normalized;
        targetPos = playerRf.position;
    }

    void FindNearstEnemy()
    {
        float distanceToEnemy = 1000;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 20);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                if (distenceTo(collider.transform) < distanceToEnemy)
                {
                    distanceToEnemy = distenceTo(collider.transform);
                    targetEnemy = collider.gameObject;
                }
            }
        }

        target = targetEnemy.transform;
        targetPos = target.position;
    }

    void FindHidable()
    {
        float distanceToHidable = 1000;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 20);
        foreach(Collider2D collider in colliders)
        {
            if (collider.CompareTag("Hide"))
            {
                if (collider.GetComponent<HidableObject>())
                {
                    if(collider.GetComponent<HidableObject>().ishideable == true)
                    {
                        if (distenceTo(collider.transform) < distanceToHidable)
                        {
                            distanceToHidable = distenceTo(collider.transform);
                            targetHideable = collider.gameObject;
                        }
                    }
                }
            }
        }

        if (targetHideable != null)
        {
            target = targetHideable.transform;
            targetPos = target.position;

            if(distenceTo(target) < 1)
            {
                Hide(targetHideable);
            }
        }
    }

    private void Hide(GameObject inObject)
    {
        Debug.Log("Companion Hide!");
        if (inObject.GetComponent<HidableObject>())
        {
            inObject.GetComponent<HidableObject>().ishideable = false;
        }
        state = CompanionState.Hidden;
        GetComponentInChildren<SpriteRenderer>().enabled = false;
        GetComponent<CapsuleCollider2D>().enabled = false;
    }

    private void UnHide(GameObject inObject)
    {
        state = CompanionState.Follow;
        if (inObject.GetComponent<HidableObject>())
        {
            inObject.GetComponent<HidableObject>().ishideable = true;
        }
        GetComponentInChildren<SpriteRenderer>().enabled = true;
        GetComponent<CapsuleCollider2D>().enabled = true;
        GetComponent<Revolver>().enabled = true;
        targetHideable = null;
    }

    void UpdatePath()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(rb.position, targetPos, OnPathComplete);
        }
    }

    void MoveAlongPath()
    {
        if (path == null)
        {
            return;
        }

        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }

        if (path != null)
        {
            Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
            Vector2 force = direction * speed * Time.deltaTime;

            rb.AddForce(force);

            float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
            if (distance < nextWayPointDistance)
            {
                currentWaypoint++;
            }
        }
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    float distenceTo(Transform target)
    {
        return Vector2.Distance(target.position, this.transform.position);
    }
}
