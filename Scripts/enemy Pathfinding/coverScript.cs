using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coverScript : MonoBehaviour
{
    public bool goLeft;
    public bool goRight;
    public bool coverNotInUse;
    Transform Player;
    public bool[] coverPos; //left: left center right right: left center right
    public Transform[] coverTransforms;
    bool gameObjectInCoverList = true;
    public bool combatStartedwh1;
    public bool combatStartedwh2;
    public bool combatStartedwh3;
    public bool combatStartedl1;
    public bool combatStartedl2;
    bool combatStart;

    private void Start()
    {
        Player = PlayerManager.instance.player.transform;
    }


    void Update()
    {
        if (combatStartedl1)
        {
            combatStart = coverController.instance.combatStartedl1;
        }
        else if (combatStartedl2)
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
            float zdist = Mathf.Abs(transform.position.z) - Mathf.Abs(Player.transform.position.z);
            if((zdist < 5 && zdist > -5) || (coverPos[0] && coverPos[1] && coverPos[2]) || (coverPos[3] && coverPos[4] && coverPos[5]))
            {
                coverNotInUse = true;
                goLeft = false;
                goRight = false;
            }
            else
            {
                if (Player.position.z > transform.position.z)
                {
                    goLeft = true;
                    goRight = false;
                    coverNotInUse = false;
                }
                else
                {
                    goLeft = false;
                    goRight = true;
                    coverNotInUse = false;
                }
            }

            if(coverNotInUse && gameObjectInCoverList)
            {
                gameObjectInCoverList = false;
                coverController.instance.coverObjs.Remove(gameObject);
            }
            else if (!coverNotInUse && !gameObjectInCoverList)
            {
                gameObjectInCoverList = true;
                coverController.instance.coverObjs.Add(gameObject);
            }
        }
    }
}
