using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class standingPosScript : MonoBehaviour
{
    public bool posInUse;
    bool gameObjectInStandingPosList = true;

    private void Update()
    {
        if(posInUse && gameObjectInStandingPosList)
        {
            gameObjectInStandingPosList = false;
            coverController.instance.standingPos.Remove(gameObject);
        }
        else if (!posInUse && !gameObjectInStandingPosList)
        {
            gameObjectInStandingPosList = true;
            coverController.instance.standingPos.Add(gameObject);
        }
    }
}
