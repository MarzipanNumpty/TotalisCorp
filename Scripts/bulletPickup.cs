using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletPickup : MonoBehaviour
{
    public int bulletCount;
    [SerializeField]
    GameObject[] bullets;

    public void changeBullets(int bulletsTotal)
    {
        for(int i = 9; i > bulletsTotal - 1; i--)
        {
            bullets[i].SetActive(false);
            Debug.Log("it happened" + bullets[i]);
        }
    }
}
