using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RacerBehavior : MonoBehaviour
{
    private NavMeshAgent agent;
    public GameObject target;
    private float upperRange = 10f;
    private float bottomRange = 1f;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        InvokeRepeating("changeSpeedToARandomNumber", 0, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(target.transform.position);
    }

    void changeSpeedToARandomNumber()
    {
        agent.speed = Random.Range(bottomRange, upperRange);
    }
}
