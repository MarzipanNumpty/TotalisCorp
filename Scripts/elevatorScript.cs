using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class elevatorScript : MonoBehaviour
{
    [SerializeField]
    Transform topTransform;
    [SerializeField]
    Transform bottomTransform;
    [SerializeField]
    float speed;
    bool move;
    [SerializeField]
    bool atTop;
    Transform targetTransform;
    [SerializeField]
    bool topFront;
    [SerializeField]
    bool bottomFront;
    Animator anim;
    public bool moveElevator = false;
    public bool moveElevator2 = false;
    Rigidbody rb;
    private void Start()
    {
        anim = GetComponent<Animator>();
        OpenDoor(true);
        moveElevator2 = moveElevator;
    }
    private void FixedUpdate()
    {
        if(move)
        {
            float dist = Vector3.Distance(transform.position, targetTransform.position);
            transform.position = Vector3.MoveTowards(transform.position, targetTransform.position, speed);
            if(dist <= 0.1)
            {
                if (!moveElevator2)
                {
                    PlayerManager.instance.player.GetComponent<CharacterController>().enabled = true;
                    PlayerManager.instance.player.GetComponent<PlayerScript>().gamePaused = false;
                    moveElevator2 = false;
                }
                move = false;
                OpenDoor(true);
            }
        }
    }

    public void StartMoving()
    {
        move = true;
    }

    public void resetValues()
    {
        anim.SetBool("openFront", false);
        anim.SetBool("closeFront", false);
        anim.SetBool("openBack", false);
        anim.SetBool("closeBack", false);
    }

    void moveToTop()
    {
        targetTransform = topTransform;
    }

    void moveToBottom()
    {
        targetTransform = bottomTransform;
    }

    public void setTransform()
    {
        if(atTop)
        {
            moveToBottom();
        }
        else
        {
            moveToTop();
        }
        OpenDoor(false);
        atTop = !atTop;
        if(!moveElevator2)
        {
            PlayerManager.instance.player.GetComponent<CharacterController>().enabled = false;
            PlayerManager.instance.player.GetComponent<PlayerScript>().gamePaused = true;
        }
    }

    void OpenDoor(bool open)
    {
        bool useFront = false;
        if (atTop)
        {
            useFront = topFront;
        }
        else
        {
            useFront = bottomFront;
        }
        if (useFront)
        {
            if(open)
            {
                anim.SetBool("openFront", true);
            }
            else
            {
                anim.SetBool("closeFront", true);
            }
        }
        else
        {
            if(open)
            {
                anim.SetBool("openBack", true);
            }
            else
            {
                anim.SetBool("closeBack", true);
            }
        }
        if (moveElevator)
        {
            moveElevator = false;
            setTransform();
        }
    }
}
