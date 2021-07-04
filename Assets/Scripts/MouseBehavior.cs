using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System;


public class MouseBehavior : MonoBehaviour
{
    private Animator anim;
    private NavMeshAgent navMeshAgent;

    public GameObject Enemy1;
    public GameObject Enemy2;
    public GameObject Teammate;
    private GameObject closestTarget;
    public GameObject GunInHand;
    public GameObject Bullet;
    private GameObject[] enemies;

    const float PATROLING_SPEED = 4f;
    const float CHASING_SPEED = 2f;
    private float distanceFromPlayer;
    private int NOTIFYING_DISTANCE = 40;
    private int SHOOTING_DISTANCE = 20;
    const int PATROLING_ANIMATION = 0;
    const int SHOOTING_ANIMATION = 1;
    const int DYING_ANIMATION = 2;
    const int BIG_NUM = 200000;
    const int NUM_OF_ENEMIES = 2;
    const int HEALTH_POINTS = 100;
    const int BULLET_DAMAGE = 10;
    const int NON_SHOOTING_POSITION = 1;
    const int SHOOTING_POSITION = 2;
    public Text allyHPStatus;

    public int hp;
    private AudioSource shotSound;

    float timer = 0;
    float RELOAD_TIME = 1.125f;

    bool gunInShootingPosition = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = PATROLING_SPEED;
        hp = HEALTH_POINTS;
        shotSound = GunInHand.GetComponent<AudioSource>();
        assignEnemies(Enemy1, Enemy2);
    }

    void assignEnemies(GameObject enemy1, GameObject enemy2)
    {
        enemies = new GameObject[NUM_OF_ENEMIES];
        enemies[0] = enemy1;
        enemies[1] = enemy2;
    }

    void Update()
    {
        if (hp <= 0)
            StartCoroutine(Die());

        else // Mouse is alive
        {
            closestTarget = getClosestEnemyTarget();
            float distanceFromTarget = getDistanceFrom(closestTarget);

            // If we reached this line, it means that there are still enemies around
            if (distanceFromTarget <= NOTIFYING_DISTANCE)
                eliminateEnemy(distanceFromTarget);

            else // Target is not in range
            {
                patrol();
                return;
            }
        }
    }

    IEnumerator Die()
    {
        anim.SetInteger("State", 2); // NPC dyes
        navMeshAgent.enabled = false; // NPS stops moving
        yield return new WaitForSeconds(3);
        gameObject.SetActive(false);
    }

    GameObject getClosestEnemyTarget()
    {
        bool isFirstEnemyAlive = enemies[0].activeSelf;
        bool isSecondEnemyAlive = enemies[1].activeSelf;

        // If Enemy is alive, get distance from it, else assign distance a BIG_NUM
        float distanceFromfirstEnemy =  isFirstEnemyAlive ? getDistanceFrom(enemies[0]) : BIG_NUM;
        float distanceFromSecondEnemy =  isSecondEnemyAlive ? getDistanceFrom(enemies[1]) : BIG_NUM;

        // If both Enemy targets are dead        
        if (distanceFromfirstEnemy == distanceFromSecondEnemy && distanceFromfirstEnemy == BIG_NUM)
            return null;

        return distanceFromfirstEnemy < distanceFromSecondEnemy ? enemies[0] : enemies[1];
    }

    void eliminateEnemy(float distanceFromTarget)
    {
        if (navMeshAgent.enabled)
        {
            navMeshAgent.SetDestination(closestTarget.transform.position);
            navMeshAgent.speed = CHASING_SPEED;
        }

        if (distanceFromTarget <= SHOOTING_DISTANCE && isEnemyInShootingRange())
            handleShooting();
        
        else // Target is not in a shooting range
        {
            navMeshAgent.enabled = true;
            anim.SetInteger("State", PATROLING_ANIMATION);
            movePistol(NON_SHOOTING_POSITION);
        }
    }

    void handleShooting()
    {
        rotateNPCTowardsTarget(closestTarget);
        navMeshAgent.enabled = false;
        anim.SetInteger("State", SHOOTING_ANIMATION);
        movePistol(SHOOTING_POSITION);
        StartCoroutine(Shoot());
    }

    void rotateNPCTowardsTarget(GameObject target)
    {
        /// Rotates the NPC towards target (so it will look like the NPC looks at its target)

        Vector3 npcPos = this.transform.position;
        Vector3 targetPos = target.transform.position;
        Vector3 delta = new Vector3(targetPos.x - npcPos.x, targetPos.y - npcPos.y, targetPos.z - npcPos.z);
        Quaternion rotation = Quaternion.LookRotation(delta);
        this.transform.rotation = rotation;
    }

    void movePistol(int position)
    {
        if (position == SHOOTING_POSITION && !gunInShootingPosition)
        {
            gunInShootingPosition = true;
            GunInHand.gameObject.transform.position = new Vector3(
                GunInHand.transform.position.x,
                GunInHand.transform.position.y + 2f,
                GunInHand.transform.position.z);
        }
        else if (position == NON_SHOOTING_POSITION && gunInShootingPosition)
        {
            gunInShootingPosition = false;
            GunInHand.gameObject.transform.position = new Vector3(
                GunInHand.transform.position.x,
                GunInHand.transform.position.y - 2f,
                GunInHand.transform.position.z);
        }
    }

    IEnumerator Shoot()
    {
        RaycastHit hit;
        timer += Time.deltaTime;
        Bullet.transform.position += new Vector3(5f,5f,5f);
        if(timer > RELOAD_TIME)
        {
            shotSound.Play();
            if (Physics.Raycast(new Vector3(this.transform.position.x, this.transform.position.y + 2f, this.transform.position.z),
                            this.transform.forward, out hit, 50f))
            {
                Bullet.transform.position = hit.point;
                timer = 0;
            }
        }
        yield return 0;
    }

    bool isEnemyInShootingRange()
    {
        RaycastHit hit;
        if (Physics.Linecast (transform.position, closestTarget.transform.position, out hit)) 
            if(hit.transform.tag == closestTarget.tag)
                return true;
        
        return false;
    }

    void patrol()
    {
        navMeshAgent.enabled = true;
        navMeshAgent.SetDestination(Teammate.transform.position);
        navMeshAgent.speed = PATROLING_SPEED;
        anim.SetInteger("State", PATROLING_ANIMATION);
    }

    float getDistanceFrom(GameObject target)
    {
        return target != null ? Vector3.Distance(this.transform.position, target.transform.position) : BIG_NUM;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Contains("Bullet"))
        {    
            hp -= BULLET_DAMAGE;
            
            if (hp < 0)
                return;

            if (hp == 0)
                {
                   allyHPStatus.GetComponent<Text>().text = allyHPStatus.GetComponent<Text>().text.Replace("Mouse: 010", "Mouse: 000");
                   return; 
                }

            string oldHPString = allyHPStatus.GetComponent<Text>().text.Substring(1,10);
            string newHPString = "Mouse: 0" + hp;
            allyHPStatus.GetComponent<Text>().text = allyHPStatus.GetComponent<Text>().text.Replace(oldHPString, newHPString);
            
            
        }        
    }
}
