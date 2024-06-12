using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using System.Collections;

public class EnemyAIPatrol : MonoBehaviour
{
    AiSensor sensor;
    AiAttack attack;
    GameObject player;
    NavMeshAgent agent;

    // Animation
    Animator animator;
    int isPatrollingHash;
    int isChasingHash;
    int isAttackingHash;

    [SerializeField] LayerMask whatIsGround;
    [SerializeField] float patrolDelay = 5f;  // Delay between patrols
    [SerializeField] float chaseDelay = 1f;   // Delay before starting to chase

    // Patroling
    Vector3 walkPoint;
    bool walkPointSet;

    // State change
    bool playerInSight, playerInAttackRange;
    bool isActing;  // To control the flow of actions

    void Start()
    {
        player = GameObject.Find("Player");
        agent = GetComponent<NavMeshAgent>();
        sensor = GetComponent<AiSensor>();
        attack = GetComponent<AiAttack>();
        isPatrollingHash = Animator.StringToHash("isPatrolling");
        isChasingHash = Animator.StringToHash("isChasing");
        isAttackingHash = Animator.StringToHash("isAttacking");
    }

    void Update()
    {
        //animator.SetBool(isPatrollingHash, true);
        playerInSight = sensor.IsInSight(player);
        playerInAttackRange = attack.IsInSight(player);
        bool isPatrolling = animator.GetBool(isPatrollingHash);
        bool isChasing = animator.GetBool(isChasingHash);
        bool isAttacking = animator.GetBool(isAttackingHash);

        if (!isActing)
        {
            if (playerInSight && playerInAttackRange)
            {
                StartCoroutine(PerformAttack());
            }
            else if (playerInSight)
            {
                StartCoroutine(StartChase());
            }
            else
            {
                StartCoroutine(StartPatrolling());
            }
        }
    }

    IEnumerator StartPatrolling()
    {
        isActing = true;
        animator.SetBool(isPatrollingHash, true);
        if (!walkPointSet) SearchWalkPoint();
        if (walkPointSet) agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        if (distanceToWalkPoint.magnitude < 1f) walkPointSet = false;

        yield return new WaitForSeconds(patrolDelay);
        isActing = false;
       // animator.SetBool(isPatrollingHash, false);
    }

    IEnumerator StartChase()
    {
        isActing = true;
        animator.SetBool(isChasingHash, true);
        yield return new WaitForSeconds(chaseDelay);
        agent.SetDestination(player.transform.position);
        isActing = false;
       // animator.SetBool(isChasingHash, false);
    }

    IEnumerator PerformAttack()
    {
        isActing = true;
        animator.SetBool(isAttackingHash, true);
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
}
