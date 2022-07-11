using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyScript : MonoBehaviour
{
    public float health = 100f;
    public float grenadeDamage = 10f;
    NavMeshAgent agent;
    public bool goingToCover;
    bool findingCover;
    public bool wantingToFindCover;
    public bool inCover;
    public Transform target;
    float timerActual;
    float timeToWait = 0.5f;
    coverScript ccScript;
    int coverPosInArray;
    public bool coverPosNotFree;

    bool wantStandingPos;
    bool wantBehindCover;
    standingPosScript spScript;
    bool standingPosNotFree;
    public List<GameObject> coverGameObjects;
    public List<GameObject> standingGameObjects;
    float range = 40;
    bool firstTime = true;
    public bool findNewCover;
    bool startNewCoverCheck;
    GameObject Player;
    BoxCollider bulletBounds;
    public bool lookAtPlayer;
    public bool getBulletTarget = true;
    [SerializeField]
    GameObject bulletSpawnObject;
    [SerializeField]
    GameObject bullet;
    public int magazineCount;
    int magazineMax = 5;
    Animator anim;
    bool startShoot = true;
    bool shooting;
    [SerializeField] GameObject dropItem;
    public bool combatStartedwh1;
    public bool combatStartedwh2;
    public bool combatStartedwh3;
    public bool combatStartedl1;
    public bool combatStartedl2;
    bool combatStart;
    bool itemDropped;
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        magazineCount = magazineMax;
        Player = PlayerManager.instance.player;
        bulletBounds = Player.transform.GetChild(0).gameObject.GetComponent<BoxCollider>();
        agent = GetComponent<NavMeshAgent>();
        coverType();
        range = Mathf.Pow(range, 2);
        StartCoroutine(getCover(0.5f));
    }

    // Update is called once per frame
    void Update()
    {
        if(findNewCover)
        {
            findNewCover = false;
            coverType();
        }
        if(combatStartedl1)
        {
            combatStart = coverController.instance.combatStartedl1;
        }
        else if(combatStartedl2)
        {
            combatStart = coverController.instance.combatStartedl2;
        }
        else if (combatStartedwh1)
        {
            combatStart = coverController.instance.combatStartedwh1;
        }
        else if (combatStartedwh2)
        {
            combatStart = coverController.instance.combatStartedwh2;
        }
        else if (combatStartedwh3)
        {
            combatStart = coverController.instance.combatStartedwh3;
        }
        if (combatStart)
        {
            if(!wantingToFindCover)
            {
                anim.SetBool("running", true);
                wantingToFindCover = true;
                timerActual = timeToWait;
            }
            if(!findingCover && timerActual <= 0)
            {
                if(wantBehindCover)
                {
                    findCover();
                }
                else if (wantStandingPos)
                {
                    findstandingPos();
                }
            }
        }

        if(timerActual > 0)
        {
            timerActual -= Time.deltaTime;
        }

        if(goingToCover)
        {
            if (wantBehindCover)
            {
                coverPosNotFree = ccScript.coverPos[coverPosInArray];
                if (!coverPosNotFree)
                {
                    float dist = Vector3.Distance(target.position, transform.position);
                    if (dist <= 1)
                    {
                        Debug.Log("Made it");
                        ccScript.coverPos[coverPosInArray] = true;
                        goingToCover = false;
                        startNewCoverCheck = true;
                        lookAtPlayer = true;
                        inCover = true;
                        anim.SetBool("running", false);
                    }
                    agent.SetDestination(target.position);
                }
                else if (!ccScript.coverNotInUse)
                {
                    if (ccScript.goLeft)
                    {
                        findNewCoverMidRun(0);
                    }
                    else
                    {
                        findNewCoverMidRun(3);
                    }
                }
                else
                {
                    findCover();
                }
            }
            else if(wantStandingPos)
            {
                standingPosNotFree = spScript.posInUse;
                if(!standingPosNotFree)
                {
                    float dist = Vector3.Distance(target.position, transform.position);
                    if (dist <= 1)
                    {
                        Debug.Log("Made it");
                        spScript.posInUse = true;
                        goingToCover = false;
                        startNewCoverCheck = true;
                        lookAtPlayer = true;
                        inCover = true;
                        anim.SetBool("running", false);
                    }
                    agent.SetDestination(target.position);
                }
                else if (standingPosNotFree)
                {
                    findstandingPos();
                }
            }
        }
        
        if (startNewCoverCheck)
        {
            startNewCoverCheck = false;
            StartCoroutine(thinkAboutNewCover(10.0f));
        }

        /*if(lookAtPlayer)
        {
            Vector3 targetDir = Player.transform.position - transform.position;

            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, 0.04f, 0.0f);

            transform.localRotation = Quaternion.LookRotation(newDir);
            if(startShoot)
            {
                startShoot = false;
                StartCoroutine(startShooting(1.0f));
            }
        }*/

        if(inCover && getBulletTarget && magazineCount > 0)
        {
            anim.SetBool("shooting", true);
            StartCoroutine(shootBullet(1.0f));
            getBulletTarget = false;
        }
        else if(inCover && getBulletTarget && magazineCount <= 0)
        {
            lookAtPlayer = true;
            anim.SetBool("shooting", false);
            StartCoroutine(reload(5.0f));
            getBulletTarget = false;
        }
    }

    private void FixedUpdate()
    {
        if(inCover)
        {
            Vector3 targetDir = Player.transform.position - transform.position;

            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, 0.04f, 0.0f);

            transform.localRotation = Quaternion.LookRotation(newDir);
        }
    }

    public void takeDamage(int damagePercent)
    {
        float damage = 100 / damagePercent;
        health -= damage;
        if(health <= 0)
        {
            if(dropItem != null && !itemDropped)
            {
                itemDropped = true;
                GameObject item = Instantiate(dropItem, transform.position, Quaternion.identity);
                if (item.CompareTag("magazine"))
                {
                    item.GetComponent<magazineScript>().bulletsInMag = Random.Range(1, 6);
                }
            }
            Destroy(gameObject);
        }
    }

    public static Vector3 ShootCoordinates(BoxCollider collider)
    {
        return new Vector3(
            Random.Range(collider.bounds.min.x, collider.bounds.max.x),
            Random.Range(collider.bounds.min.y, collider.bounds.max.y),
            Random.Range(collider.bounds.min.z, collider.bounds.max.z)
            );
    }

    IEnumerator startShooting(float time)
    {
        yield return new WaitForSeconds(time);
        lookAtPlayer = false;
        startShoot = true;
    }

    IEnumerator shootBullet(float time)
    {
        yield return new WaitForSeconds(time);
        magazineCount--;
        Vector3 bulletLocation = ShootCoordinates(bulletBounds);
        bulletSpawnObject.transform.LookAt(bulletLocation);
        GameObject currentBullet = Instantiate(bullet, bulletSpawnObject.transform.position, Quaternion.identity);
        Debug.Log(currentBullet.name);
        currentBullet.transform.eulerAngles = new Vector3(bulletSpawnObject.transform.eulerAngles.x, bulletSpawnObject.transform.eulerAngles.y, 0);
        getBulletTarget = true;
    }

    IEnumerator reload(float time)
    {
        yield return new WaitForSeconds(time);
        magazineCount = magazineMax;
        getBulletTarget = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("bullet") && other.gameObject.layer != 11)
        {
            takeDamage(5);
            Debug.Log(health);
        }
    }




    //cover methods
    void coverType()
    {
        if (!firstTime)
        {
            if (wantStandingPos)
            {
                spScript.posInUse = false;
            }
            else
            {
                ccScript.coverPos[coverPosInArray] = false;
            }
        }
        else
        {
            firstTime = false;
        }
        int standOrCover = Random.Range(0, 2);
        if (standOrCover == 0)
        {
            anim.SetBool("standing", true);
            wantStandingPos = true;
            wantBehindCover = false;
        }
        else
        {
            anim.SetBool("standing", false);
            wantBehindCover = true;
            wantStandingPos = false;
        }
        Debug.Log(wantStandingPos + name);
        findingCover = false;
        wantingToFindCover = false;
        inCover = false;
        lookAtPlayer = false;
    }
    IEnumerator getCover(float time)
    {
        yield return new WaitForSeconds(time);
        for (int i = 0; i < coverController.instance.coverObjs.Count; i++)
        {
            float distance = (transform.position - coverController.instance.coverObjs[i].transform.position).sqrMagnitude;
            if (distance < range)
            {
                coverGameObjects.Add(coverController.instance.coverObjs[i]);
            }
        }
        for (int i = 0; i < coverController.instance.standingPos.Count; i++)
        {
            float distance = (transform.position - coverController.instance.standingPos[i].transform.position).sqrMagnitude;
            if (distance < range)
            {
                standingGameObjects.Add(coverController.instance.standingPos[i]);
            }
        }
    }
    IEnumerator thinkAboutNewCover(float time)
    {
        int coverChance = Random.Range(0, 2);
        yield return new WaitForSeconds(time);
        if (coverChance == 0)
        {
            coverType();
        }
        else
        {
            startNewCoverCheck = true;
        }
    }
    void findNewCoverMidRun(int additive)
    {
        if (!ccScript.coverPos[additive + 0])
        {
            coverPosInArray = additive + 0;
            target = ccScript.coverTransforms[additive + 0];
        }
        else if (!ccScript.coverPos[additive + 1])
        {
            coverPosInArray = additive + 1;
            target = ccScript.coverTransforms[additive + 1];
        }
        else
        {
            coverPosInArray = additive + 2;
            target = ccScript.coverTransforms[additive + 2];
        }
    }
    void findCover() //need to give cover some time to calculate go left or go right
    {
        findingCover = true;
        int arrayAim = Random.Range(0, coverGameObjects.Count);
        int coverSectionAim = Random.Range(0, 2);
        ccScript = coverGameObjects[arrayAim].GetComponent<coverScript>();
        Debug.Log(arrayAim);
        if (ccScript.goLeft)
        {
            target = ccScript.coverTransforms[coverSectionAim];
            goingToCover = true;
            coverPosInArray = coverSectionAim;
        }
        else if(ccScript.goRight)
        {
            coverSectionAim += 3;
            target = ccScript.coverTransforms[coverSectionAim];
            goingToCover = true;
            coverPosInArray = coverSectionAim;
        }
    }
    void findstandingPos()
    {
        findingCover = true;
        int arrayAim = Random.Range(0, standingGameObjects.Count);
        spScript = standingGameObjects[arrayAim].GetComponent<standingPosScript>();
        if(!spScript.posInUse)
        {
            target = standingGameObjects[arrayAim].transform;
            goingToCover = true;
        }
    }
    public void takeGrenadeDamage(float dist, float radius)
    {
        if(dist < 0)
        {
            dist *= -1;
        }
        float damageProximity = radius - dist;
        float damage = grenadeDamage / damageProximity;
        health -= damage * 3;
    }
}
