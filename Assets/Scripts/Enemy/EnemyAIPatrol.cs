using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class EnemyAIPatrol : MonoBehaviour
{
    GameObject player;
    NavMeshAgent agent;

     // Animation
    Animator animator;
    int isPatrollingHash;
    int isChasingHash;
    int isAttackingHash;
    bool isPatrolling;
    bool isChasing;
    bool isAttacking;
    
    [SerializeField] LayerMask whatIsGround, whatIsPlayer;
    [SerializeField] float patrolDelay = 1f;  // Delay between patrols
    [SerializeField] float chaseDelay = 0.2f;   // Delay before starting to chase

    [SerializeField] float baseSpeed = 7f;
    [SerializeField] float speedIncreasePerLevel = 1f;

    // Patroling
    Vector3 walkPoint;
    bool walkPointSet;

    // State change
    bool isActing;  // To control the flow of actions

    public float distance = 30;
    public float attackDistance = 4.5f;
    public float angle = 30;
    public float height = 5f;
    public Color meshColor = Color.green;
    public int scanFrequency = 40;
    public List<GameObject> Objects = new List<GameObject>();
    public List<GameObject> attackPlayer = new List<GameObject>();

    Collider[] colliders = new Collider[50];
    Mesh mesh, meshAttack;
    int count;
    float scanInterval;
    float scanTimer;

    bool playerInSight, playerInAttackRange;

    void Start()
    {
        scanInterval = 1.0f / scanFrequency;
        player = GameObject.Find("Player");
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        isPatrollingHash = Animator.StringToHash("isPatrolling");
        isChasingHash = Animator.StringToHash("isChasing");
        isAttackingHash = Animator.StringToHash("isAttacking");
        UpdateSpeed();
    }

    void Update()
    {
        scanTimer -= Time.deltaTime;
        if (scanTimer < 0)
        {
            scanTimer += scanInterval;
            Scan();
            AttackScan();
        }
        
        isPatrolling = animator.GetBool(isPatrollingHash);
        isChasing = animator.GetBool(isChasingHash);
        isAttacking = animator.GetBool(isAttackingHash);

        if (!isActing)
        {
            if (!playerInSight && !playerInAttackRange)
            {
                StartCoroutine(StartPatrolling());
            }
            if (playerInSight && !playerInAttackRange)
            {
                StartCoroutine(StartChase());
            }
            if (playerInSight && playerInAttackRange)
            {
                StartCoroutine(PerformAttack());
            }
        }
    }

    void UpdateSpeed()
    {
        if (agent != null)
        {
            agent.speed = baseSpeed + ScoreManager.scoreCount * speedIncreasePerLevel;
        }
    }

    private void Scan() {
        count = Physics.OverlapSphereNonAlloc(transform.position, distance, colliders, whatIsPlayer, QueryTriggerInteraction.Collide);

        playerInSight = false;

        Objects.Clear();
        for (int i = 0; i < count; ++i)
        {
            GameObject obj = colliders[i].gameObject;
            if (IsInSight(obj)) 
            {
                Objects.Add(obj);
            }
        }
        if (Objects.Contains(player))
        {
            playerInSight = true;
        }
    }

    private void AttackScan() {
        count = Physics.OverlapSphereNonAlloc(transform.position, attackDistance, colliders, whatIsPlayer, QueryTriggerInteraction.Collide);


        playerInAttackRange = false;

        attackPlayer.Clear();
        for (int i = 0; i < count; ++i)
        {
            GameObject obj = colliders[i].gameObject;
            if (IsInAttackRange(obj)) 
            {
                attackPlayer.Add(obj);
            }
        }
        if (attackPlayer.Contains(player))
        {
            playerInAttackRange = true;
        }
    }

    public bool IsInSight(GameObject obj)
    {
        Vector3 origin = transform.position;
        Vector3 dest = obj.transform.position;
        Vector3 direction = dest - origin;

        if (direction.y < 0 || direction.y > height)
        {
            return false;
        }

        direction.y = 0;
        float deltaAngle = Vector3.Angle(direction, transform.forward);
        if (deltaAngle > angle) 
        {
            return false;
        }

        origin.y += height / 2;
        dest.y = origin.y;
        if (Physics.Linecast(origin, dest, whatIsGround))
        {
            return false;
        }
        return true;
    }

    public bool IsInAttackRange(GameObject obj)
    {
        Vector3 origin = transform.position;
        Vector3 dest = obj.transform.position;
        Vector3 direction = dest - origin;

        if (direction.y < 0 || direction.y > height)
        {
            return false;
        }

        direction.y = 0;
        float deltaAngle = Vector3.Angle(direction, transform.forward);
        if (deltaAngle > angle) 
        {
            return false;
        }

        origin.y += height / 2;
        dest.y = origin.y;
        if (Physics.Linecast(origin, dest, whatIsGround))
        {
            return false;
        }
        return true;
    }

    Mesh CreateWedgeMesh()
    {
        Mesh mesh = new Mesh();

        int segments = 10;
        int numTriangles = (segments * 4) + 2 + 2;
        int numVerticies = numTriangles * 3;

        Vector3[] vertices = new Vector3[numVerticies];
        int[] triangles = new int[numVerticies];

        Vector3 bottomCenter = Vector3.zero;
        Vector3 bottomLeft = Quaternion.Euler(0, -angle, 0) * Vector3.forward * distance;
        Vector3 bottomRight = Quaternion.Euler(0, angle, 0) * Vector3.forward * distance;

        Vector3 topCenter = bottomCenter + Vector3.up * height;
        Vector3 topRight = bottomRight + Vector3.up * height;
        Vector3 topLeft = bottomLeft + Vector3.up * height;

        int vert = 0;

        // left side
        vertices[vert++] = bottomCenter;
        vertices[vert++] = bottomLeft;
        vertices[vert++] = topLeft;

        vertices[vert++] = topLeft;
        vertices[vert++] = topCenter;
        vertices[vert++] = bottomCenter;

        // right side
        vertices[vert++] = bottomCenter;
        vertices[vert++] = topCenter;
        vertices[vert++] = topRight;

        vertices[vert++] = topRight;
        vertices[vert++] = bottomRight;
        vertices[vert++] = bottomCenter;

        float currentAngle = -angle;
        float deltaAngle = (angle * 2) / segments;
        for (int i = 0; i < segments; ++i)
        {
            bottomLeft = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * distance;
            bottomRight = Quaternion.Euler(0, currentAngle + deltaAngle, 0) * Vector3.forward * distance;            

            topRight = bottomRight + Vector3.up * height;
            topLeft = bottomLeft + Vector3.up * height;

             // far side
            vertices[vert++] = bottomLeft;
            vertices[vert++] = bottomRight;
            vertices[vert++] = topRight;

            vertices[vert++] = topRight;
            vertices[vert++] = topLeft;
            vertices[vert++] = bottomLeft;

            // top
            vertices[vert++] = topCenter;
            vertices[vert++] = topLeft;
            vertices[vert++] = topRight;

            // bottom

            vertices[vert++] = bottomCenter;
            vertices[vert++] = bottomRight;
            vertices[vert++] = bottomLeft;

            currentAngle += deltaAngle;
        }

        for (int i = 0; i < numVerticies; i++)
        {
            triangles[i] = i;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

    Mesh CreateWedgeMeshAttack()
    {
        Mesh mesh = new Mesh();

        int segments = 10;
        int numTriangles = (segments * 4) + 2 + 2;
        int numVerticies = numTriangles * 3;
        

        Vector3[] vertices = new Vector3[numVerticies];
        int[] triangles = new int[numVerticies];

        Vector3 bottomCenter = Vector3.zero;
        Vector3 bottomLeft = Quaternion.Euler(0, -angle, 0) * Vector3.forward * attackDistance;
        Vector3 bottomRight = Quaternion.Euler(0, angle, 0) * Vector3.forward * attackDistance;

        Vector3 topCenter = bottomCenter + Vector3.up * height;
        Vector3 topRight = bottomRight + Vector3.up * height;
        Vector3 topLeft = bottomLeft + Vector3.up * height;

        int vert = 0;

        // left side
        vertices[vert++] = bottomCenter;
        vertices[vert++] = bottomLeft;
        vertices[vert++] = topLeft;

        vertices[vert++] = topLeft;
        vertices[vert++] = topCenter;
        vertices[vert++] = bottomCenter;

        // right side
        vertices[vert++] = bottomCenter;
        vertices[vert++] = topCenter;
        vertices[vert++] = topRight;

        vertices[vert++] = topRight;
        vertices[vert++] = bottomRight;
        vertices[vert++] = bottomCenter;

        float currentAngle = -angle;
        float deltaAngle = (angle * 2) / segments;
        for (int i = 0; i < segments; ++i)
        {
            bottomLeft = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * attackDistance;
            bottomRight = Quaternion.Euler(0, currentAngle + deltaAngle, 0) * Vector3.forward * attackDistance;            

            topRight = bottomRight + Vector3.up * height;
            topLeft = bottomLeft + Vector3.up * height;

             // far side
            vertices[vert++] = bottomLeft;
            vertices[vert++] = bottomRight;
            vertices[vert++] = topRight;

            vertices[vert++] = topRight;
            vertices[vert++] = topLeft;
            vertices[vert++] = bottomLeft;

            // top
            vertices[vert++] = topCenter;
            vertices[vert++] = topLeft;
            vertices[vert++] = topRight;

            // bottom

            vertices[vert++] = bottomCenter;
            vertices[vert++] = bottomRight;
            vertices[vert++] = bottomLeft;

            currentAngle += deltaAngle;
        }

        for (int i = 0; i < numVerticies; i++)
        {
            triangles[i] = i;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

    private void OnValidate()
    {
        mesh = CreateWedgeMesh();
        meshAttack = CreateWedgeMeshAttack();
        scanInterval = 1.0f / scanFrequency;
    }

    IEnumerator StartPatrolling()
    {
        isActing = true;
        //Handle Animation
        if (!isPatrolling)
        {
            animator.SetBool(isChasingHash, false);
            animator.SetBool(isPatrollingHash, true);
        }
        
        if (!walkPointSet) SearchWalkPoint();
        if (walkPointSet) agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        if (distanceToWalkPoint.magnitude < 1f) walkPointSet = false;

        yield return new WaitForSeconds(patrolDelay);
        isActing = false;
       
    }

    IEnumerator StartChase()
    {
        isActing = true;
        //Handle animation
        if (!isChasing)
            animator.SetBool(isChasingHash, true);
        yield return new WaitForSeconds(chaseDelay);
        agent.SetDestination(player.transform.position);
        isActing = false;
    }

    IEnumerator PerformAttack()
    {
        isActing = true;
        //Handle animation
        if(!isAttacking)
            animator.SetBool(isAttackingHash, true);
        yield return new WaitForSeconds(5f);
        // Grab player
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        yield return null;  // Placeholder since the scene will change
    }

    private void SearchWalkPoint()
    {
        MapBounds mapBounds = GameObject.FindObjectOfType<MapBounds>();
        Vector3 min = mapBounds.min.position;
        Vector3 max = mapBounds.max.position;

        walkPoint = new Vector3(Random.Range(min.x, max.x), transform.position.y, Random.Range(min.z, max.z));
        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround)) walkPointSet = true;
    }

    void OnDrawGizmos()
{
    if (mesh == null || meshAttack == null)
    {
        mesh = CreateWedgeMesh();
        meshAttack = CreateWedgeMeshAttack();
    }

    // Draw view cone
    Gizmos.color = meshColor;
    Gizmos.DrawMesh(mesh, transform.position, transform.rotation);

    // Draw attack cone
    Gizmos.color = Color.blue;
    Gizmos.DrawMesh(meshAttack, transform.position, transform.rotation);
}
}