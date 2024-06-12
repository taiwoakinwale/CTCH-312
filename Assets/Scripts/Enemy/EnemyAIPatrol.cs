using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class EnemyAIPatrol : MonoBehaviour
{
    [HideInInspector] public AiSensor sensor;
    GameObject player;
    NavMeshAgent agent;

    [SerializeField] LayerMask whatIsGround, whatIsPlayer;

    // Patroling
    Vector3 walkPoint;
    bool walkPointSet;

    // State change
    bool playerInSight, playerInAttackRange;

    void Start()
    {
        player = GameObject.Find("Player");
        agent = GetComponent<NavMeshAgent>();
        sensor = GetComponent<AiSensor>();
    }

    void Update()
    {
        playerInSight = sensor.IsInSight(player);
        Debug.Log("This is returning " + playerInSight);
        // playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
        
        if(!playerInSight) 
        {
            Patroling();
        }
        if(playerInSight) 
        {
            Chase();
        }
        if(playerInSight && playerInAttackRange)Attack();
    }

    void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        // Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    void Chase()
    {
        agent.SetDestination(player.transform.position);
    }

    void Attack()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    
    private void SearchWalkPoint()
    {
        MapBounds mapBounds = GameObject.FindObjectOfType<MapBounds>();
        Vector3 min = mapBounds.min.position;
        Vector3 max = mapBounds.max.position;

        walkPoint = new Vector3( Random.Range(min.x, max.x), transform.position.y, Random.Range(min.z, max.z));

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }
}
