using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerThrowGrenade : MonoBehaviour
{
    private bool isThrown = false;
    public GameObject player;
    public GameObject explosion;
    public GameObject Grenade;
    public GameObject part1;
    public GameObject part2;
    private Transform grenadeTransform;

    const int THROW_DISTANCE = 12;
    const int THROWING_HEIGHT = 5;
    const int GRANADE_EXPLOSION_DISTANCE = 10;
    const float GRANADE_POWER = 1000.0f;
    const float TIME_UNTIL_EXPLODE = 1.5f;

    void Start()
    {
    }

    void Update()
    {
        if (Input.GetKey("q") && !isThrown)
        {
                isThrown = true;
                ThrowGrenade();
        }
    }

    void ThrowGrenade()
    {
        Vector3 direction = player.transform.forward * THROW_DISTANCE;
        direction.y = THROWING_HEIGHT;
        Rigidbody rb = Grenade.GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.AddForce(direction,ForceMode.Impulse);
        StartCoroutine(Explode());
    }

    IEnumerator Explode()
    {
        yield return new WaitForSeconds(TIME_UNTIL_EXPLODE);
        explosion.SetActive(true);
        part1.SetActive(false);
        part2.SetActive(false);
        AudioSource grenadeExplosion = Grenade.GetComponent<AudioSource>();
        grenadeExplosion.Play();

        // Apply explosion on nearby object 
        Collider[] objectsCollider = Physics.OverlapSphere(transform.position, GRANADE_EXPLOSION_DISTANCE);
        
        for(int i = 0; i<objectsCollider.Length;i++)
        {
            if(objectsCollider[i]!=null)
            {
                Rigidbody rbo = objectsCollider[i].GetComponent<Rigidbody>();
                if(rbo!=null)
                    rbo.AddExplosionForce(GRANADE_POWER, transform.position, GRANADE_EXPLOSION_DISTANCE);
            }
        }
    }

    float CalculateExplosionValue(Vector3 aSource, Vector3 aTarget, float aRange)
    {
        float dist = (aTarget - aSource).magnitude;
        if (dist > aRange)
            return 0.0f;
        return 1.0f - dist / aRange; 
    }
}
