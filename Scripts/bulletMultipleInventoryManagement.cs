using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletMultipleInventoryManagement : MonoBehaviour
{
    [SerializeField]
    GameObject[] bullets;
    public bool isFull;
    public int currentBulletCount;
    int arrayPos;
    int childPos = 0;
    bool removeTwoBullets;
    bool addTwoBullets;
    int tenMultiple;
    public void changeBullets(int bulletCount, bool removeBullet)
    {
        tenMultiple = currentBulletCount / 10;
        int bulletRemainder = bulletCount % 50;
        if(removeBullet)
        {
            arrayPos = bulletRemainder / 10;
        }
        else
        {
            childPos = bulletRemainder % 10;
        }
        //Debug.Log(arrayPos);
        //Debug.Log(childPos);
        /*if (!removeBullet)
        {
            childPos = childPos - 1;
            if(childPos < 0)
            {
                childPos = 9;
            }
        }
        else
        {
            childPos = childPos + 1;
            if (childPos > 9)
            {
                childPos = 0;
            }
        }*/
        if(arrayPos < 0)
        {
            arrayPos = 0;
        }
        if(!removeBullet)
        {
            /*if(!addTwoBullets)
            {
                bullets[arrayPos].gameObject.transform.GetChild(childPos - 1).gameObject.SetActive(true);
                addTwoBullets = true;
            }*/
            removeTwoBullets = false;
            //Debug.Log(arrayPos);
            //Debug.Log(childPos);
            //Debug.Log(bullets[arrayPos]);
            //Debug.Log(bullets[arrayPos].gameObject.transform.GetChild(childPos).gameObject);
            bullets[arrayPos].gameObject.transform.GetChild(childPos).gameObject.SetActive(true);
            currentBulletCount++;
            //Debug.Log("Add");
        }
        else
        {
            /*if(!removeTwoBullets)
            {
                bullets[arrayPos].gameObject.transform.GetChild(childPos + 1).gameObject.SetActive(false);
                removeTwoBullets = true;
            }*/
            addTwoBullets = false;
            bullets[arrayPos].gameObject.transform.GetChild(childPos).gameObject.SetActive(false);
            currentBulletCount--;
            //Debug.Log("Remove");
        }
        if(currentBulletCount >= 50)
        {
            //Debug.Log("1");
            isFull = true;
        }
        else
        {
            //Debug.Log("2");
            isFull = false;
        }
        if(!removeBullet)
        {
            arrayPos = bulletRemainder / 10;
        }
        else
        {
            childPos = bulletRemainder % 10;
        }
    }

    public void removeTen()
    {
        int endPoint = 0;
        tenMultiple = currentBulletCount / 10;
        int preEP = 0;
        if (currentBulletCount % 10 == 0)
        {
            tenMultiple--;
            endPoint = 10;
            preEP = endPoint;
        }
        else
        {
            endPoint = currentBulletCount % 10;
            endPoint++;
            preEP = endPoint - 1;
        }
        /*Debug.Log(tenMultiple + "multiple");
        Debug.Log(currentBulletCount % 10 + "remainder");*/
        for (int i = 0; i < endPoint; i++)
        {
            bullets[tenMultiple].gameObject.transform.GetChild(i).gameObject.SetActive(false);
            if(i < preEP)
            {
                currentBulletCount--;
            }
        }
        Debug.Log(currentBulletCount + "bullets");
    }

    public void addMultipleBullets(int bullet)
    {
        for(int i = 0; i < bullet; i++)
        {
            bullets[i].SetActive(true);
        }
    }
}
