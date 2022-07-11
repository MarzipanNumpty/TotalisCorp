using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enterCombat : MonoBehaviour
{
    [SerializeField] bool combatStartedwh1;
    [SerializeField] bool combatStartedwh2;
    [SerializeField] bool combatStartedwh3;
    [SerializeField] bool combatStartedl1;
    [SerializeField] bool combatStartedl2;
    [SerializeField] bool combatStartedGC;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            //coverController.instance.combatStarted = true;
            if (combatStartedwh1)
            {
                coverController.instance.combatStartedwh1 = true;
            }
            else if(combatStartedwh2)
            {
                coverController.instance.combatStartedwh2 = true;
            }
            else if (combatStartedwh3)
            {
                coverController.instance.combatStartedwh3 = true;
            }
            else if (combatStartedl1)
            {
                coverController.instance.combatStartedl1 = true;
            }
            else if (combatStartedl2)
            {
                coverController.instance.combatStartedl2 = true;
            }
            else if (combatStartedGC)
            {
                coverController.instance.combatStartedGC = true;
            }
        }
    }
}
