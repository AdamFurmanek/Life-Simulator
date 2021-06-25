using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    public NavMeshAgent agent;

    private void Start()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();

    }

    private void Update()
    {
            agent.SetDestination(City.Houses[Random.Range(0, City.Houses.Count)].transform.position);
    }

}
