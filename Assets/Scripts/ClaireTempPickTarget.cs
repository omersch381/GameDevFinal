using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClaireTempPickTarget : MonoBehaviour
{
    const int BIG_NUM = 200000;
    public GameObject Claire;
    private Vector3[] targetLocations = {
        new Vector3(33.06f, 1.08f, 9.7104f),
        new Vector3(-28.94f, 1.08f, 9.7104f),
        new Vector3(-25.94f, 1.08f, 76.46f),
        new Vector3(31.31f, 1.08f, 9.7104f),
    };

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (getDistanceFrom(Claire) <= 6f)
        {
            int index = Random.Range(0, targetLocations.Length);
            this.gameObject.transform.position = targetLocations[index];
        }
    }

    float getDistanceFrom(GameObject target)
    {
        return target != null ? Vector3.Distance(this.transform.position, target.transform.position) : BIG_NUM;
    }
}
