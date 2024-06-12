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

    ScoreManager score;

    [SerializeField] LayerMask whatIsGround;
    [SerializeField] float patrolDelay = 5f;  // Delay between patrols
    [SerializeField] float chaseDelay = 1f;   // Delay before starting to chase

     [SerializeField] float baseSpeed = 3.5f;
    [SerializeField] float speedIncreasePerLevel = 0.5f;

    // Patroling
    Vector3 walkPoint;
    bool walkPointSet;

    // State change
    bool playerInSight, playerInAttackRange;
    bool isActing;  // To control the flow of actions

    void Start()
    {
        player = GameObject.Find("Player");
        if (player == null)
            Debug.LogError("Player GameObject not found.");

        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
            Debug.LogError("NavMeshAgent component not found.");

        sensor = GetComponent<AiSensor>();
        if (sensor == null)
            Debug.LogError("AiSensor component not found.");

        attack = GetComponent<AiAttack>();
        if (attack == null)
            Debug.LogError("AiAttack component not found.");

        UpdateSpeed();
    }

    void UpdateSpeed()
    {
        if (agent != null && score != null)
        {
            agent.speed = baseSpeed + score.getScoreCount() * speedIncreasePerLevel;
        }
    }

    void Update()
    {
        if (player == null || sensor == null || attack == null || agent == null)
        {
            Debug.LogError("One or more essential components are missing.");
            return; // Skip the update cycle if something is wrong.
        }

        playerInSight = sensor.IsInSight(player);
        playerInAttackRange = attack.IsInSight(player);

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

        UpdateSpeed();
    }

    IEnumerator StartPatrolling()
    {
        isActing = true;
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
        yield return new WaitForSeconds(chaseDelay);
        agent.SetDestination(player.transform.position);
        isActing = false;
    }

    IEnumerator PerformAttack()
    {
        isActing = true;
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
