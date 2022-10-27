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
    private float nextWayPointDistance;
    [SerializeField]
    private List<Transform> PatrolRoute;
    private int PatrolIndex;
    [SerializeField]
    private Transform target;
    private Vector2 targetPos;

    Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath;

    Seeker seeker;
    Rigidbody2D rb;

    BehaviourTree tree;
    Node.Status treeStatus;

    public enum ActionState { IDLE, WORKING};
    ActionState state = ActionState.IDLE;

    bool playerDetected;
    float searchTime = 5;
    float searchTimer;

    void Start()
    {
        fov = Instantiate(Pf_fov, null).GetComponent<FieldOfView>();
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        searchTimer = searchTime;

        BuildTree();
        StartCoroutine("Behave");


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
        FindTargetPlayer();

        if (!reachedEndOfPath)
        {
            MoveAlongPath();
        }
        else
        {
            Invoke("MoveAlongPath", Random.Range(0, 3.0f));
        }

        Debug.Log(playerDetected);
    }

    void BuildTree()
    {
        tree = new BehaviourTree("Root");
        Leaf isPlayerDetected = new Leaf("isPlayerDetected", PlayerDetected);
        Inverter playerNotDetected = new Inverter("playerNotDetected");
        playerNotDetected.AddChild(isPlayerDetected);

        Leaf PatrolBehaviour = new Leaf("Patrol", Patrol);
        BehaviourTree PatrolDep = new BehaviourTree("PatrolDependence");
        PatrolDep.AddChild(playerNotDetected);
        DpSequence PatrolSeq = new DpSequence("PatrolSequence", PatrolDep);
        PatrolSeq.AddChild(PatrolBehaviour);

        Leaf ChaseBehaviour = new Leaf("Chase", chasePlayer);
        BehaviourTree ChaseDep = new BehaviourTree("ChaseDependence");
        ChaseDep.AddChild(isPlayerDetected);
        DpSequence ChaseSeq = new DpSequence("ChaseSequence", ChaseDep);
        ChaseSeq.AddChild(ChaseBehaviour);

        Leaf SearchTarget = new Leaf("Search", SearchAround);


        Selector rootSelector = new Selector("rootSelector");

        rootSelector.AddChild(PatrolSeq);
        rootSelector.AddChild(ChaseSeq);
        tree.AddChild(rootSelector);
    }

    Node.Status Patrol()
    {
        if (reachedEndOfPath)
        {
                if (PatrolIndex < PatrolRoute.Count - 1)
                {
                    PatrolIndex++;
                }
                else
                {
                    PatrolIndex = 0;
                }
                target = PatrolRoute[PatrolIndex];
                targetPos = target.position;
        }
        return Node.Status.RUNNING;
    }

    Node.Status chasePlayer()
    {
        
        target = playerRf;
        targetPos = target.position;
        Vector3 dirToPlayer = (playerRf.position - transform.position).normalized;
        RaycastHit2D raycastHits = Physics2D.Raycast(transform.position, dirToPlayer);
        if (raycastHits.collider != null)
        {
            if (raycastHits.collider.gameObject.GetComponent<PlayerController>() == null)
            {
                playerDetected = false;
                target = null;
                targetPos = target.position;
                Debug.Log("Player Lost");
                return Node.Status.FAILURE;
            }
        }

        return Node.Status.RUNNING;
    }

    Node.Status SearchAround()
    {
        while(searchTimer > 0)
        {
            targetPos = PickRandomPoint(transform.position, 5);
            searchTimer--;
            return Node.Status.RUNNING;
        }
        return Node.Status.SUCCESS;
    }

    Node.Status PlayerDetected()
    {
        if (playerDetected)
        {
            return Node.Status.SUCCESS;
        }
        else
        {
            return Node.Status.FAILURE;
        }

    }

    Vector2 GetAimDirection()
    {
        return rb.velocity;
    }

    private void FindTargetPlayer()
    {
        if(Vector3.Distance(transform.position, playerRf.position) < fovRadius)
        {
            Vector3 dirToPlayer = (playerRf.position - transform.position).normalized;
            if(Vector3.Angle(GetAimDirection(), dirToPlayer) < fovAngle/2f)
            {
                RaycastHit2D raycastHits= Physics2D.Raycast(transform.position, dirToPlayer, fovRadius);
                Debug.DrawRay(transform.position, dirToPlayer, Color.green);
                if (raycastHits.collider != null)
                {
                    if (raycastHits.collider.gameObject.GetComponent<PlayerController>() != null)
                    {
                        playerDetected = true;
                        Debug.Log("Player Detected");
                    }
                }
            }
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
        if(path == null)
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

    IEnumerator Behave()
    {
        while (true)
        {
            treeStatus = tree.Process();
            yield return new WaitForSeconds(0.5f);
        }
    }
}
