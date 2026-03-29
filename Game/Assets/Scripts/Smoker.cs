using Game;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Smoker : MonoBehaviour
{
    public Transform centrePoint;
    public float movementRange;
    public float secondhandSmokeRange;

    private NavMeshAgent agent;
    private Player player;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameManager.Instance.player;
    }

    void Update()
    {
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            Vector3 point;
            if (RandomPoint(centrePoint.position, movementRange, out point))
            {
                // Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f);
                agent.SetDestination(point);
            }
        }

        var distanceFromPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceFromPlayer <= secondhandSmokeRange)
        {
            player.playerMovementScript.reverseControls = true;
        }
        else
        {
            player.playerMovementScript.reverseControls = false;
        }
    }

    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        Vector3 randomPoint = center + Random.insideUnitSphere * range;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }
}
