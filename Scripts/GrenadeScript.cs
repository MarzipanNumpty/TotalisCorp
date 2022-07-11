using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeScript : MonoBehaviour
{
    Rigidbody rb;
    float timer = 5f;
    float radius = 10f;
    Collider gameObjectcollider;
    [SerializeField] GameObject explosion;
    bool explode = false;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Vector3 angle = (transform.forward + transform.up).normalized;
        rb.AddForce(angle * 12.5f, ForceMode.Impulse);
        gameObjectcollider = GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if(timer > 0)
        {
            timer -= Time.deltaTime;
            if(timer <= 0.5f && !explode)
            {
                explode = true;
                explosion.GetComponent<ParticleSystem>().Play(true);
            }
        }
        else
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
            foreach(var obj in hitColliders)
            {
                if(obj.CompareTag("enemy"))
                {
                    float dist = Vector3.Distance(obj.transform.position, transform.position);
                    obj.GetComponent<enemyScript>().takeGrenadeDamage(dist, radius);
                }
                if (obj.CompareTag("meleeEnemy"))
                {
                    float dist = Vector3.Distance(obj.transform.position, transform.position);
                    obj.GetComponent<meleeEnemyScript>().takeGrenadeDamage(dist, radius);
                }
            }
            Destroy(gameObject);
        }
    }
}
