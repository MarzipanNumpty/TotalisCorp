using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class healingScript : MonoBehaviour
{
    Animator anim;
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void heal()
    {
        Destroy(gameObject);
    }
}
