using UnityEngine;
using UnityEngine.AI;

public class EnemyWander : MonoBehaviour
{
    public NavMeshAgent agent;
    public Vector3 walkBoundsMin;
    public Vector3 walkBoundsMax;
    private Vector3 targetPosition;

    private FieldOfView fov;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        fov = GetComponent<FieldOfView>();
        SetNewDestination();
    }

    void Update()
    {
        if (!fov.CanSeePlayer)
        {
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                SetNewDestination();
            }
        }
    }

    void SetNewDestination()
    {
        float x = Random.Range(walkBoundsMin.x, walkBoundsMax.x);
        float z = Random.Range(walkBoundsMin.z, walkBoundsMax.z);
        targetPosition = new Vector3(x, transform.position.y, z);
        agent.SetDestination(targetPosition);
    }
}

