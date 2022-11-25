using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyController : MonoBehaviour
{
    public float fovRadius = 5f;
    public float fovAngle = 60;

    [SerializeField]
    private Transform Pf_fov;
    private FieldOfView fov;

    [SerializeField]
    private Transform playerRf;

    [SerializeField]
    private float speed;
    [SerializeField]
    private float hearingR;
    [SerializeField]
    private float nextWayPointDistance;
    [SerializeField]
    private List<Transform> PatrolRoute;
    private int PatrolIndex;
    [SerializeField]
    private Transform target;
    private Vector2 targetPos;

    [SerializeField]
    private float attackRange = 3;
    [SerializeField]
    private float attackTime;
    [SerializeField]
    private float attackTimer;
    [SerializeField]
    private int damage;

    private GameManager manager;


    Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath;

    Seeker seeker;
    Rigidbody2D rb;

    public enum AgentState { Patrol, Invastigat, Chase };
    AgentState state;

    bool playerDetected;
    [SerializeField]
    bool noiseDetected;
    Vector2 noiseLocation;

    HealthComponent health;

    private void Awake()
    {
        manager = GameObject.FindObjectOfType<GameManager>();
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<HealthComponent>();
    }

    void Start()
    {
        fov = Instantiate(Pf_fov, null).GetComponent<FieldOfView>();

        state = AgentState.Patrol;
        attackTimer = attackTime;


        PatrolIndex = 0;
        target = PatrolRoute[PatrolIndex];
        targetPos = target.position;
        InvokeRepeating("UpdatePath", 0f, .5f);
    }


    void Update()
    {
        Vector3 fovOri = transform.position;
        fovOri.y += 0.6f;
        fov.SetOrigin(fovOri);
        fov.SetAimDir(GetAimDirection());
        fov.SetViewAngle(fovAngle);
        fov.SetViewRadius(fovRadius);

        if (!reachedEndOfPath)
        {
            MoveAlongPath();
        }
        else
        {
            Invoke("MoveAlongPath", Random.Range(0, 3.0f));
        }

        if (health.isHealthZero())
        {
            OnDeath();
        }

        Debug.Log(playerDetected);

        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        FindTargetPlayer();

        if (state == AgentState.Patrol)
        {
            if (!playerDetected)
            {
                if (!noiseDetected)
                {
                    Patrol();
                } else if (noiseDetected)
                {
                    state = AgentState.Invastigat;
                }
            }
            else if (playerDetected)
            {
                state = AgentState.Chase;
            }
        }
        else if (state == AgentState.Chase)
        {
            if (!playerDetected)
            {
                if (reachedEndOfPath)
                {
                    state = AgentState.Patrol;
                    target = PatrolRoute[PatrolIndex];
                    targetPos = target.position;
                }
            }
            else if (playerDetected)
            {
                chasePlayer();
            }
        } else if (state == AgentState.Invastigat)
        {
            if (noiseDetected)
            {
                invastigateNoise();
            }
            else if (!noiseDetected)
            {
                if (reachedEndOfPath)
                {
                    state = AgentState.Patrol;
                    target = PatrolRoute[PatrolIndex];
                    targetPos = target.position;
                }
            }

            if (Vector3.Distance(transform.position, noiseLocation) <= 2)
            {
                noiseDetected = false;
            }

            if (playerDetected)
            {
                chasePlayer();
            }
        }
    }

    void Patrol()
    {
        if (Vector2.Distance(transform.position, PatrolRoute[PatrolIndex].transform.position) <= 1)
        {
            if (PatrolIndex < PatrolRoute.Count - 1)
            {
                PatrolIndex++;
            }
            else
            {
                PatrolIndex = 0;
            }
            reachedEndOfPath = false;
            target = PatrolRoute[PatrolIndex];
            targetPos = target.position;
        }
    }

    void chasePlayer()
    {
        target = playerRf;
        targetPos = target.position;
        float distance = Vector3.Distance(target.position, transform.position);
        if (distance > 7)
        {
            playerDetected = false;
            targetPos = target.position;
            target.GetComponent<PlayerController>().state = PlayerController.PlayerState.Undetacted;
            target = null;
            Debug.Log("Player Lost!");
        }
    }

    void invastigateNoise()
    {
        if (noiseDetected)
        {
            if (noiseLocation != null)
            {
                targetPos = noiseLocation;
            }
        }
    }

    Vector2 GetAimDirection()
    {
        return rb.velocity;
    }

    private void FindTargetPlayer()
    {
        if (Vector3.Distance(transform.position, playerRf.position) < fovRadius)
        {
            Vector3 dirToPlayer = (playerRf.position - transform.position).normalized;
            if (Vector3.Angle(GetAimDirection(), dirToPlayer) < fovAngle / 2f)
            {
                RaycastHit2D raycastHits = Physics2D.Raycast(transform.position, dirToPlayer, fovRadius);
                Debug.DrawRay(transform.position, dirToPlayer, Color.green);
                if (raycastHits.collider != null)
                {
                    if (raycastHits.collider.gameObject.GetComponent<PlayerController>() != null)
                    {
                        if (raycastHits.collider.gameObject.GetComponent<PlayerController>().state != PlayerController.PlayerState.Hidden)
                        {
                            playerDetected = true;
                            raycastHits.collider.gameObject.GetComponent<PlayerController>().state = PlayerController.PlayerState.Detected;
                            Debug.Log("Player Detected");
                        }
                    }
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Attack(collision.gameObject);
        }
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

    protected Vector2 PickRandomPoint(Vector2 center, int radius)
    {
        Vector2 point = Random.insideUnitCircle * radius;
        point += center;
        return point;
    }


    public void hearSound(Vector2 atLocation)
    {
        noiseLocation = atLocation;
        noiseDetected = true;
    }

    public void OnDeath()
    {
        if (manager)
        {
            manager.KilledEnemies += 1;
        }
        if (playerDetected)
        {
            target.GetComponent<PlayerController>().state = PlayerController.PlayerState.Undetacted;
        }
        Destroy(fov.gameObject);
        Destroy(this.gameObject);
    }

    void Attack(GameObject target)
    {
        if(attackTimer <= 0)
        {
            HealthComponent targetH = target.GetComponent<HealthComponent>();
            if (targetH)
            {
                targetH.takeDamage(damage);
                attackTimer = attackTime;
            }
        }
    }
}
