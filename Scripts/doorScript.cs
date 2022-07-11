using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorScript : MonoBehaviour
{
    public bool redKeyCardNeeded;
    public bool yellowKeyCardNeeded;
    public bool blueKeyCardNeeded;
    public bool noCardNeeded;
    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void openDoor()
    {
        anim.SetBool("openDoor", true);
    }

    public void closeDoor()
    {
        anim.SetBool("closeDoor", true);
    }

    public void stopOpening()
    {
        anim.SetBool("openDoor", false);
    }

    public void stopClosing()
    {
        anim.SetBool("closeDoor", false);
    }
}
