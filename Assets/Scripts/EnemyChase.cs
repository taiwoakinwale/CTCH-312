using UnityEngine;
using UnityEngine.AI;

public class EnemyChase : MonoBehaviour
{
    private NavMeshAgent agent;
    private FieldOfView fov;
    public Transform player;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        fov = GetComponent<FieldOfView>();
    }

    void Update()
    {
        if (fov.CanSeePlayer)
        {
            agent.SetDestination(player.position);
        }
        else
        {
            // Optionally, return to wandering or another state here
        }
    }
}
