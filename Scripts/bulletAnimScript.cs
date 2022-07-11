using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletAnimScript : MonoBehaviour
{
    Animator anim;
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void reload()
    {
        anim.SetBool("reload", false);
    }
    
    public void unload()
    {
        anim.SetBool("unload", false);
    }
}
