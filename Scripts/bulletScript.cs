using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletScript : MonoBehaviour
{
    Rigidbody rb;
    float timer = 2.0f;
    public bool enemyBullet;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.AddForce(transform.forward * 4.0f, ForceMode.Impulse);
        timer -= Time.deltaTime;
        if(timer < 0)
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(enemyBullet)
        {
            if (other.gameObject.layer == 9 && other.gameObject.CompareTag("bulletBound"))
            {
                PlayerManager.instance.player.GetComponent<PlayerScript>().takeDamage(15, true);
                Destroy(gameObject);
            }
            Destroy(gameObject);
        }
        else
        {
            if (other.gameObject.CompareTag("enemy"))
            {
                other.gameObject.GetComponent<enemyScript>().takeDamage(5);
            }
            if(other.gameObject.CompareTag("meleeEnemy"))
            {
                other.gameObject.GetComponent<meleeEnemyScript>().takeDamage(2);
            }
            Destroy(gameObject);
        }
    }
}
