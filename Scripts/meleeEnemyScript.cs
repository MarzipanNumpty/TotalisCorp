using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class meleeEnemyScript : MonoBehaviour
{
    Transform startPos;
    public float health = 100f;
    float grenadeDamage = 125f;
    [SerializeField] GameObject dropItem;
    NavMeshAgent agent;
    GameObject player;
    Animator anim;
    bool hit;
    float timer = 5.0f;
    void Start()
    {
        player = PlayerManager.instance.player;
        startPos = transform;
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(coverController.instance.combatStartedGC && !hit)
        {
            if(Vector3.Distance(transform.position, player.transform.position) > 5)
            {
                agent.SetDestination(player.transform.position);
            }
            else
            {
                hit = true;
                anim.SetBool("run", false);
                anim.SetBool("melee", true);
                player.GetComponent<PlayerScript>().takeDamage(12);
            }
        }

        if(hit)
        {
            timer -= Time.deltaTime;
            if(timer <= 0)
            {
                anim.SetBool("melee", false);
                anim.SetBool("run", true);
                hit = false;
                timer = 5.0f;
            }
        }
    }

    public void takeGrenadeDamage(float dist, float radius)
    {
        if (dist < 0)
        {
            dist *= -1;
        }
        float damageProximity = radius - dist;
        float damage = grenadeDamage / damageProximity;
        health -= damage * 3;
    }
    public void takeDamage(int damagePercent)
    {
        float damage = 100 / damagePercent;
        health -= damage;
        if (health <= 0)
        {
            if (dropItem != null)
            {
                GameObject item = Instantiate(dropItem, transform.position, Quaternion.identity);
                if (item.CompareTag("magazine"))
                {
                    item.GetComponent<magazineScript>().bulletsInMag = Random.Range(1, 6);
                }
            }
            Destroy(gameObject);
        }
    }

}
