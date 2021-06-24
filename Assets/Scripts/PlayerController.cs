using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    private Camera camera;

    public NavMeshAgent agent;

    private void Start()
    {
        camera = Camera.main;
        agent = gameObject.GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit))
            {
                agent.SetDestination(hit.point);

                //agent.agentTypeID = GetAgenTypeIDByName("Car");
                //agent.speed = 7;
                //https://answers.unity.com/questions/1650130/change-agenttype-at-runtime.html
                //agent.agentTypeID = NavMeshAgent.
            }
        }   
    }

    private int GetAgenTypeIDByName(string agentTypeName)
    {
        int count = NavMesh.GetSettingsCount();
        string[] agentTypeNames = new string[count + 2];
        for (var i = 0; i < count; i++)
        {
            int id = NavMesh.GetSettingsByIndex(i).agentTypeID;
            string name = NavMesh.GetSettingsNameFromID(id);
            if (name == agentTypeName)
            {
                return id;
            }
        }
        return -1;
    }

}
