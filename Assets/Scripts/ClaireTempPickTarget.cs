using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClaireTempPickTarget : MonoBehaviour
{
    private Vector3[] targetLocations = {
        new Vector3(10.8418f, 1.08f, 9.7104f),
        new Vector3(33.06f, 1.08f, 9.7104f),
        new Vector3(33.06f, 1.08f, 42.6f),
        new Vector3(4.81f, 1.08f, 42.6f)
    };

    void OnTriggerEnter(Collider other)
    {
        int index = Random.Range(0, targetLocations.Length);
        this.gameObject.transform.position = targetLocations[index];
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
