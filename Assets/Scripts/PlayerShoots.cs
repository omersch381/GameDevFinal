using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerShoots : MonoBehaviour
{
    public GameObject GunInHand;
    public GameObject aCamera;
    public GameObject Claire;
    public GameObject Racer;
    public GameObject Bullet;
    private AudioSource shotSound;

    float timer = 0;
    const float RELOAD_TIME = 0.1f;

    void Start()
    {
        shotSound = GunInHand.GetComponent<AudioSource>();
    }
    
    void Update()
    {
        if(timer > RELOAD_TIME/2)
            Bullet.transform.position += new Vector3(5f,5f,5f);
        if (Input.GetKeyDown("space") && GunInHand.activeSelf)
            StartCoroutine(Shoot());
    }

    IEnumerator Shoot()
    {
        RaycastHit hit;
        timer += Time.deltaTime;

        if(timer > RELOAD_TIME)
        {
            shotSound.Play();
            Physics.Raycast(aCamera.transform.position, aCamera.transform.forward, out hit);
            Bullet.transform.position = hit.point;
            timer = 0;
        }
        yield return 0;
    }
}
