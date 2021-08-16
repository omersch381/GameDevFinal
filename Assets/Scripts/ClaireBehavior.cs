using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System;

public class ClaireBehavior : MonoBehaviour
{
    private Animator anim;
    private NavMeshAgent navMeshAgent;

    public GameObject Enemy1;
    public GameObject Enemy2;
    public GameObject Teammate;
    public GameObject PatrolTarget;
    public GameObject GunInHand;
    public GameObject GunOnGround;
    public GameObject Bullet;
    private GameObject closestTarget;
    public GameObject explosion;
    public GameObject GrenadeInDrawers;
    public GameObject GrenadeInHand;
    public GameObject part1;
    public GameObject part2;
    public GameObject ChestOfDrawers;
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

    public static int hp;
    private AudioSource shotSound;
    public Text enemyHPStatus;
    private bool damageReceived = false;

    float ShootingTimer = 0;
    float GrenadeTimer = 0;
    float RELOAD_TIME = 1.125f;
    float RECEIVE_GRANADE_TIME = 1.5f;
    float GRENADE_THROW_GAP_TIME = 5f;
    const int THROW_DISTANCE = 12;
    const int THROWING_HEIGHT = 5;
    const int GRANADE_EXPLOSION_DISTANCE = 10;
    const float GRANADE_POWER = 1000.0f;
    const float TIME_UNTIL_EXPLODE = 1.5f;

    bool gunInShootingPosition = false;
    private string oldHPString;

    void Start()
    {
        PlaceChestOfDrawersWithPistol();
        anim = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = PATROLING_SPEED;
        hp = HEALTH_POINTS;
        shotSound = GunInHand.GetComponent<AudioSource>();
        assignEnemies(Enemy1, Enemy2);
    }

    void PlaceChestOfDrawersWithPistol()
    {
        Vector3[] chestLocations = {
            new Vector3(-30f, 0.5f, 18.949f),
            new Vector3(-30f, 0.5f, 62.45f)
        };

        int index = UnityEngine.Random.Range(0, chestLocations.Length);
        ChestOfDrawers.transform.position = chestLocations[index];
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

        else // Claire is alive
        {
            if (!GunInHand.activeSelf)
            {
                GetPistol();
                return;
            }

            if (!GrenadeInHand.activeSelf)
            {
                GetGrenade();
                return;
            }

            if (GetComponent<Rigidbody>().velocity.magnitude > 0)
                StartCoroutine(ReceiveGrenadeDamage());

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

    void GetPistol()
    {
        navMeshAgent.enabled = true;
        navMeshAgent.SetDestination(GunOnGround.transform.position);
        if (getDistanceFrom(GunOnGround) <= 10f)
        {
            GunOnGround.SetActive(false);
            GunInHand.SetActive(true);
        }
    }

    void GetGrenade()
    {
        navMeshAgent.enabled = true;
        navMeshAgent.SetDestination(GrenadeInDrawers.transform.position);
        if (getDistanceFrom(GrenadeInDrawers) <= 10f)
        {
            GrenadeInDrawers.SetActive(false);
            GrenadeInHand.SetActive(true);
        }
    }

    IEnumerator Die()
    {
        print("Claire Died, good for you!!!");
        hp = hp <= 0 ? 0 : hp;
        anim.SetInteger("State", 2); // NPC dyes
        navMeshAgent.enabled = false; // NPS stops moving
        yield return new WaitForSeconds(3);
        gameObject.SetActive(false);
        string oldHPString = enemyHPStatus.GetComponent<Text>().text.Substring(0,11);
        enemyHPStatus.GetComponent<Text>().text = enemyHPStatus.GetComponent<Text>().text.Replace(oldHPString, "Claire: 000");
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
        StartCoroutine(ThrowGrenade());
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
        ShootingTimer += Time.deltaTime;
        Bullet.transform.position += new Vector3(5f,5f,5f);
        if(ShootingTimer > RELOAD_TIME)
        {
            shotSound.Play();
            if (Physics.Raycast(new Vector3(this.transform.position.x, this.transform.position.y + 2f, this.transform.position.z),
                            this.transform.forward, out hit, 50f))
            {
                Bullet.transform.position = hit.point;
                ShootingTimer = 0;
            }
        }
        yield return 0;
    }

    IEnumerator ThrowGrenade()
    {
        GrenadeTimer += Time.deltaTime;
        if(GrenadeTimer > GRENADE_THROW_GAP_TIME)
        {
            throwGrenade();
            GrenadeTimer = 0;
        }
        yield return 0;
    }

    void throwGrenade()
    {
        Vector3 direction = this.transform.forward * THROW_DISTANCE;
        direction.y = THROWING_HEIGHT;
        Rigidbody rb = GrenadeInHand.GetComponent<Rigidbody>();
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
        AudioSource grenadeExplosion = GrenadeInHand.GetComponent<AudioSource>();
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

    void updateHPToUI()
    {
        hp = hp <= 0 ? 0 : hp;
        string myHP = hp <= 10 ? "00" + hp : "0" + hp;
        oldHPString = "Claire: " + myHP;
        enemyHPStatus.GetComponent<Text>().text = oldHPString + enemyHPStatus.GetComponent<Text>().text.Substring(oldHPString.Length);
    }

    IEnumerator ReceiveGrenadeDamage()
    {
        if (!damageReceived)
        {
            damageReceived = true;
            hp -= (int) GetComponent<Rigidbody>().velocity.magnitude * 2;
            updateHPToUI();
        }
        GrenadeTimer += Time.deltaTime;
        if(GrenadeTimer > RECEIVE_GRANADE_TIME)
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            GrenadeTimer = 0;
            damageReceived = false;
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
        navMeshAgent.SetDestination(PatrolTarget.transform.position);
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
                enemyHPStatus.GetComponent<Text>().text = enemyHPStatus.GetComponent<Text>().text.Replace("Claire: 010", "Claire: 000");
                return; 
            }

            string oldHPString = enemyHPStatus.GetComponent<Text>().text.Substring(0,11);
            int newHP;
            try {
                newHP = int.Parse(oldHPString.Substring(8, 3)) - BULLET_DAMAGE;
            } catch {
                return;
            }
            updateHPToUI();
        }
    }
}
