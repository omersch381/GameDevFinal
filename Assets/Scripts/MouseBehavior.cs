using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MouseBehavior : MonoBehaviour
{
    private Animator anim;
    private NavMeshAgent agent;
    public GameObject target;
    private float speed = 1f;
    private bool isIncreasing = false;
    private float upperRange = 10f;
    private float bottomRange = 1f;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        InvokeRepeating("changeSpeedToANumberWhichGoesUpAndDown", 0, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(target.transform.position);
    }

    void changeSpeedToANumberWhichGoesUpAndDown()
    {
        agent.speed = speed;
        if (0.1 > Mathf.Abs(speed - upperRange) || 0.1 > Mathf.Abs(speed - bottomRange))
            isIncreasing = !isIncreasing;
        speed = isIncreasing ? speed + 1f : speed - 1f;
    }
}
