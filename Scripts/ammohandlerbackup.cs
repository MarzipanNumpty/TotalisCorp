using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class ammohandlerbackup : MonoBehaviour
{
    //inventory related variables
    [SerializeField] GameObject[] heldMagazines;
    [SerializeField] GameObject[] invSpots;
    [SerializeField] Transform[] invTransforms;
    [SerializeField] GameObject inventoryObject;
    [SerializeField] Transform invWorldPos;
    [SerializeField] Transform selectedItemWorldPos;
    [SerializeField] Transform dropItemPos;
    int inventoryPos;
    bool invOpen;
    public bool invItemSelected;
    public GameObject selectedInvItem;
    magazineScript magScript;
    public int looseBulletCount = 0;
    bool firstItemToSwap;
    int firstItemSwapPos;
    int invSpotsTaken = 0;
    public List<int> itemSize;
    int invX = 2;
    int invY = 2;
    public bool moveInvItem;
    itemSize itemScript;


    //shooting related variables
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] GameObject bullet;
    [SerializeField] Transform bulletSpawnPos;
    [SerializeField] GameObject cam;
    bool magInGun;
    GameObject currentMag;
    bool reloading;
    Animator anim;

    //grenade related variables
    [SerializeField] GameObject grenade;
    [SerializeField] Transform grenadeSpawnPos;
    int grenadeCount;

    [SerializeField] GameObject ammoInvPrefab;
    [SerializeField] GameObject ammoBundleTen;
    public List<GameObject> ammoInInv;
    int fullLooseBulletsCount = 0;
    int currentAmmoInvPos;
    bool removeAmmo;
    bool pickingUpAmmo;
    void Start()
    {
        inventoryObject.SetActive(false);
        anim = GetComponent<Animator>();
        for (int i = 0; i < invX * invY; i++)
        {
            itemSize.Add(0);
        }
    }

    public void shoot()
    {
        if (magInGun && !reloading)
        {
            magScript = currentMag.GetComponent<magazineScript>();
            if (magScript.bulletsInMag > 0)
            {
                GameObject CurrentBullet = Instantiate(bullet, bulletSpawnPos.position, Quaternion.identity);
                CurrentBullet.transform.localEulerAngles = new Vector3(cam.transform.localEulerAngles.x, transform.localEulerAngles.y, 0);
                Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, enemyLayer))
                {
                    Debug.Log(hit.transform.gameObject.name);
                }
                magScript.bulletsInMag--;
                Debug.Log(magScript.bulletsInMag);
            }
            else
            {
                //add click sound
            }
        }
    }
    public void reloadGun()
    {
        //play mag leaving anim
        GameObject newMag = null;
        bool alreadyEmpty = false;
        if (currentMag == null)
        {
            alreadyEmpty = true;
        }
        for (int i = 0; i < heldMagazines.Length; i++)
        {
            if (heldMagazines[i] != null && heldMagazines[i] != currentMag && heldMagazines[i].CompareTag("magazine"))
            {
                if (heldMagazines[i].GetComponent<magazineScript>().bulletsInMag > 0)
                {
                    newMag = heldMagazines[i];
                    break;
                }
                else
                {
                    continue;
                }
            }
        }
        if (newMag != null && !alreadyEmpty)
        {
            anim.SetBool("fullReload", true);
            currentMag = newMag;
            magInGun = true;
        }
        else if (newMag != null && alreadyEmpty)
        {
            anim.SetBool("fillMag", true);
            currentMag = newMag;
            magInGun = true;
        }
        else if (!alreadyEmpty)
        {
            anim.SetBool("emptyMag", true);
            currentMag = newMag;
            magInGun = false;
        }
    }
    public void finishedReloading()
    {
        anim.SetBool("fullReload", false);
        anim.SetBool("fillMag", false);
        anim.SetBool("emptyMag", false);
        reloading = false;
    }


    public void throwGrenade()
    {
        GameObject currentGreande = Instantiate(grenade, grenadeSpawnPos.position, Quaternion.identity);
        currentGreande.transform.localEulerAngles = new Vector3(cam.transform.localEulerAngles.x, transform.localEulerAngles.y, 0);
    }


    public void openInv() //this spawns the inventory in the world and despawns
    {
        if (invOpen)
        {
            inventoryObject.SetActive(false);
            invOpen = false;
            firstItemToSwap = false;
            if (selectedInvItem != null)
            {
                selectedInvItem.transform.position = invTransforms[inventoryPos].position;
            }
            magScript = null;
            selectedInvItem = null;
            invItemSelected = false;
            invSpots[inventoryPos].SetActive(false);
            invSpots[0].SetActive(true);
            inventoryPos = 0;
        }
        else
        {
            inventoryObject.transform.position = invWorldPos.position;
            inventoryObject.transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
            inventoryObject.SetActive(true);
            invOpen = true;
        }
    }

    public void addMagazine(GameObject newMag) //this adds magazines to the inventory
    {
        int size = newMag.GetComponent<itemSize>().size;
        int remainingInvSpots = heldMagazines.Length - invSpotsTaken;
        Debug.Log(remainingInvSpots);
        if (remainingInvSpots >= size)
        {
            for (int i = 0; i < heldMagazines.Length; i++)
            {
                Debug.Log("in for loop");
                if (heldMagazines[i] == null)
                {
                    if (size == 1)
                    {
                        Debug.Log("size1");
                        heldMagazines[i] = newMag.transform.gameObject;
                        newMag.transform.position = invTransforms[i].position;
                        newMag.transform.localEulerAngles = new Vector3(0, 0, 0);
                        newMag.transform.SetParent(invTransforms[i]);
                        if (newMag.CompareTag("magazine"))
                        {
                            newMag.GetComponent<Rigidbody>().useGravity = false;
                        }
                        invSpotsTaken++;
                        itemSize[i] = newMag.GetComponent<itemSize>().size;
                        newMag.GetComponent<itemSize>().pos[0] = i;
                        newMag.GetComponent<Collider>().enabled = false;
                        break;
                    }
                    else if (size == 2)
                    {
                        Debug.Log("insize2");
                        int remainder1 = i / invY;
                        /*if(i == 0)
                        {
                            remainder1 = 1;
                            
                        }
                        else
                        {
                            remainder1 = i / invY;
                        }*/
                        int remainder2 = (i + 1) / invY;
                        bool addObject = false;
                        int pos1 = 0;
                        int pos2 = 0;
                        Debug.Log("remainder 1: " + remainder1 + "remainder 2: " + remainder2);
                        if (remainder1 == remainder2)
                        {
                            if (heldMagazines[i + 1] == null)
                            {
                                pos1 = i;
                                pos2 = i + 1;
                                addObject = true;
                                newMag.GetComponent<itemSize>().direction[0] = true;
                            }
                        }
                        else if (heldMagazines[i + invY] == null)
                        {
                            pos1 = i;
                            pos2 = i + invY;
                            addObject = true;
                            newMag.GetComponent<itemSize>().direction[2] = true;
                        }
                        /*else if (heldMagazines[i + invY] == null)
                        {
                            pos1 = i;
                            pos2 = i + invY;
                            addObject = true;
                        }*/
                        if (addObject)
                        {
                            heldMagazines[pos1] = newMag.transform.gameObject;
                            heldMagazines[pos2] = newMag.transform.gameObject;
                            newMag.transform.position = invTransforms[pos1].position;
                            newMag.transform.localEulerAngles = new Vector3(0, 0, 0);
                            newMag.transform.SetParent(invTransforms[pos1]);
                            invSpotsTaken += 2;
                            itemSize[pos1] = newMag.GetComponent<itemSize>().size;
                            itemSize[pos2] = newMag.GetComponent<itemSize>().size;
                            newMag.GetComponent<itemSize>().pos[0] = pos1;
                            newMag.GetComponent<itemSize>().pos[1] = pos2;
                            newMag.GetComponent<Collider>().enabled = false;
                            break;
                        }
                    }
                    else if (size == 3)
                    {
                        int remainder1 = i / invY;
                        int remainder2 = i + 1 / invY;
                        if (remainder1 == remainder2)
                        {
                            if (heldMagazines[i + 1] == null)
                            {
                                if (heldMagazines[i + invY] == null)
                                {

                                }
                            }
                        }
                    }

                }
            }
        }
    }

    public void selectInvItem() //this is used to specificy which inventory item you want to intereact with
    {
        if (heldMagazines[inventoryPos] != null)
        {
            if (invItemSelected)
            {
                selectedInvItem.transform.position = invTransforms[inventoryPos].position;
                magScript = null;
                selectedInvItem = null;
                invItemSelected = false;
            }
            else
            {
                selectedInvItem = heldMagazines[inventoryPos];
                selectedInvItem.transform.position = selectedItemWorldPos.position;
                magScript = selectedInvItem.GetComponent<magazineScript>();
                invItemSelected = true;
            }
        }
    }

    public void changeBulletsInMag(int changeBullets, bool playAnim = false) //this changes the amount of bullets in the selected magazine
    {
        int newNum = 0;
        bool affectBullets = false;
        bool magScriptExists = false;
        if (magScript != null)
        {
            magScriptExists = true;
        }
        if (magScriptExists && !pickingUpAmmo)
        {
            newNum = magScript.bulletsInMag + changeBullets;
            if (newNum < 0)
            {
                magScript.bulletsInMag = 0;
            }
            else if (newNum > magScript.maxBulletCount)
            {
                magScript.bulletsInMag = magScript.maxBulletCount;
            }
            else
            {
                affectBullets = true;
            }
        }
        if (affectBullets || pickingUpAmmo)
        {
            if (changeBullets > 0 && looseBulletCount > 0)
            {
                if (playAnim)
                {
                    Animator bulletAnim = selectedInvItem.GetComponentInChildren<Animator>();
                    bulletAnim.SetBool("reload", true);
                }
                looseBulletCount--;
                int currentBulletCount = ammoInInv[currentAmmoInvPos].GetComponent<bulletMultipleInventoryManagement>().currentBulletCount;
                if (removeAmmo)
                {
                    if (ammoInInv[currentAmmoInvPos] != selectedInvItem)
                    {
                        for (int i = 0; i < ammoInInv.Count; i++)
                        {
                            if (ammoInInv[i] == selectedInvItem)
                            {
                                currentAmmoInvPos = i;
                            }
                        }
                    }
                } //fullLooseBulletsCount != looseBulletCount / 50 && looseBulletCount / 50 != ammoInInv.Count
                else if (currentBulletCount == 0 || looseBulletCount == 0)
                {
                    fullLooseBulletsCount = looseBulletCount / 50;
                    for (int i = 0; i < heldMagazines.Length; i++)
                    {
                        if (heldMagazines[i] == ammoInInv[currentAmmoInvPos])
                        {
                            heldMagazines[i] = null;
                        }
                    }
                    Destroy(ammoInInv[currentAmmoInvPos]);
                    ammoInInv.RemoveAt(currentAmmoInvPos);
                    invSpotsTaken--;
                    if (looseBulletCount != 0)
                    {
                        for (int i = ammoInInv.Count - 1; i > -1; i--) //need to fix
                        {
                            if (ammoInInv[i].GetComponent<bulletMultipleInventoryManagement>().isFull == true)
                            {
                                currentAmmoInvPos = i;
                                break;
                            }
                        }
                    }
                }
                magScript.bulletsInMag = newNum;
                ammoInInv[currentAmmoInvPos].GetComponent<bulletMultipleInventoryManagement>().changeBullets(looseBulletCount, true);
            }
            else if (changeBullets < 0)
            {
                bool stopTakingAmmo = false;
                if (magScriptExists)
                {
                    if (magScript.bulletsInMag <= 0)
                    {
                        stopTakingAmmo = true;
                    }
                }
                if (invSpotsTaken == heldMagazines.Length)
                {
                    stopTakingAmmo = ammoInInv[currentAmmoInvPos].GetComponent<bulletMultipleInventoryManagement>().isFull;
                }
                if (!stopTakingAmmo)
                {
                    if (playAnim)
                    {
                        Animator bulletAnim = selectedInvItem.GetComponentInChildren<Animator>();
                        bulletAnim.SetBool("unload", true);
                    }
                    if (looseBulletCount == 0 || fullLooseBulletsCount != looseBulletCount / 50)
                    {
                        fullLooseBulletsCount = looseBulletCount / 50;
                        GameObject newAmmo = Instantiate(ammoInvPrefab);
                        addMagazine(newAmmo);
                        ammoInInv.Add(newAmmo);
                        newAmmo.transform.localEulerAngles = new Vector3(0, 0, 0);
                        for (int i = 0; i < ammoInInv.Count; i++)
                        {
                            if (ammoInInv[i].GetComponent<bulletMultipleInventoryManagement>().isFull == false)
                            {
                                currentAmmoInvPos = i;
                                break;
                            }
                        }
                    }
                    looseBulletCount++;
                    if (magScriptExists)
                    {
                        magScript.bulletsInMag = newNum;
                    }
                    ammoInInv[currentAmmoInvPos].GetComponent<bulletMultipleInventoryManagement>().changeBullets(looseBulletCount, false);
                }
            }
        }
    }

    public void inventoryChange(int posChange, int rotation) //this changes which inventory slot is highlighted
    {
        //Debug.Log("changein" + posChange);
        if (itemScript != null)
        {
            if ((itemScript.tempPos[1] < invSpots.Length) && (itemScript.tempPos[1] >= 0))
            {
                invSpots[itemScript.tempPos[1]].SetActive(false);
                invSpots[itemScript.tempPos[1]].GetComponent<Image>().color = Color.white;
            }
        }
        int previousNum = inventoryPos;
        int newNum = inventoryPos + posChange;
        int posDiff = 0;
        if (!moveInvItem)
        {
            if (newNum < 0)
            {
                inventoryPos = 0;
                newNum = 0;
            }
            else if (newNum > heldMagazines.Length - 1)
            {
                inventoryPos = heldMagazines.Length - 1;
                newNum = heldMagazines.Length - 1;
            }
            else
            {
                inventoryPos = newNum;
            }
            invSpots[previousNum].SetActive(false);
            invSpots[newNum].SetActive(true);
        }
        else
        {
            itemScript = selectedInvItem.GetComponent<itemSize>();
            Debug.Log("rotation" + rotation);
            if (rotation != 0)
            {
                Debug.Log("We in");
                int currentDir = 0;
                for (int i = 0; i < itemScript.direction.Length; i++)
                {
                    if (itemScript.direction[i] == true)
                    {
                        itemScript.direction[i] = false;
                        currentDir = i;
                        break;
                    }
                }
                if (currentDir + rotation < 0)
                {
                    currentDir = 3;
                }
                else if (currentDir + rotation > 3)
                {
                    currentDir = 0;
                }
                else
                {
                    currentDir += rotation;
                }
                itemScript.direction[currentDir] = true;
            }
            //Debug.Log("pos" + newNum + "inv" + inventoryPos + "newnum2" + (newNum + invY));
            if (newNum > heldMagazines.Length - 1)
            {
                newNum = heldMagazines.Length - 1;
            }
            else if (newNum < 0)
            {
                newNum = 0;
            }
            // Debug.Log("pos" + newNum + "inv" + inventoryPos + "newnum2" + (newNum + invY));
            inventoryPos = newNum;
            if (posChange == 1 || posChange == -1)
            {
                posDiff = 1;
            }
            else
            {
                posDiff = invY;
            }
            Debug.Log(posDiff);
            selectedInvItem.transform.position = invTransforms[newNum].position;
            itemScript.tempPos[0] = newNum;
            bool showSquareFirstCheck = false;
            bool showSquareSecondCheck = false;
            int axis = 0;
            if (itemScript.size == 2)
            {
                if (itemScript.direction[0])
                {
                    itemScript.tempPos[1] = newNum + 1;
                    axis = invY;
                }
                else if (itemScript.direction[1])
                {
                    itemScript.tempPos[1] = newNum - 1;
                    axis = invY;
                }
                else if (itemScript.direction[2])
                {
                    itemScript.tempPos[1] = newNum + 2;
                    axis = invX;
                }
                else if (itemScript.direction[3])
                {
                    itemScript.tempPos[1] = newNum - 2;
                    axis = invX;
                }
                Debug.Log("1" + itemScript.tempPos[0] / axis);
                Debug.Log("2" + itemScript.tempPos[1] / axis);
                if (itemScript.tempPos[1] / axis == itemScript.tempPos[0] / axis || itemScript.direction[2] || itemScript.direction[3])
                {
                    showSquareFirstCheck = true;
                    if ((itemScript.tempPos[1] < invSpots.Length) && (itemScript.tempPos[1] >= 0))
                    {
                        if (heldMagazines[itemScript.tempPos[1]] != null && heldMagazines[itemScript.tempPos[1]] != selectedInvItem)
                        {
                            showSquareSecondCheck = true;
                        }
                    }
                }
            }
            invSpots[previousNum].SetActive(false);
            invSpots[newNum].SetActive(true);
            if (heldMagazines[inventoryPos] == null || heldMagazines[itemScript.tempPos[0]] == selectedInvItem)
            {
                invSpots[newNum].GetComponent<Image>().color = Color.green;
            }
            else
            {
                invSpots[newNum].GetComponent<Image>().color = Color.red;
            }
            //Debug.Log("boiiiis" + (newNum + posDiff) + "length" + invSpots.Length);
            if (itemScript.size == 2)
            {
                if ((itemScript.tempPos[1] < invSpots.Length) && (itemScript.tempPos[1] >= 0) && showSquareFirstCheck && !showSquareSecondCheck)
                {
                    invSpots[itemScript.tempPos[1]].SetActive(true);
                    invSpots[itemScript.tempPos[1]].GetComponent<Image>().color = Color.green;
                }
                else if ((itemScript.tempPos[1] < invSpots.Length) && (itemScript.tempPos[1] >= 0) && showSquareSecondCheck)
                {
                    invSpots[itemScript.tempPos[1]].SetActive(true);
                    invSpots[itemScript.tempPos[1]].GetComponent<Image>().color = Color.red;
                }
                //Debug.Log("we in");
            }

            /*if(itemScript.tempPos[1] < invSpots.Length - 1 || itemScript.tempPos[1] >= 0)
            {
                invSpots[itemScript.tempPos[1]].SetActive(false);
                invSpots[itemScript.tempPos[1]].GetComponent<Image>().color = Color.white;
            }*/

        }
    }

    public void swapInventorySlots() //swaps magazine spaces in inventory
    {
        if (!moveInvItem && heldMagazines[inventoryPos] != null)
        {
            selectedInvItem = heldMagazines[inventoryPos];
            //invItemSelected = true;
            moveInvItem = true;
            itemScript = selectedInvItem.GetComponent<itemSize>();
            selectedInvItem.transform.parent = null;
            for (int i = 0; i < itemScript.size; i++)
            {
                itemScript.tempPos[i] = itemScript.pos[i];
            }

        }
        else if ((heldMagazines[inventoryPos] == null || (heldMagazines[itemScript.pos[0]] == selectedInvItem && heldMagazines[itemScript.pos[1]] == selectedInvItem)) && itemScript != null)
        {
            if (itemScript.size == 1)
            {
                if (heldMagazines[itemScript.tempPos[0]] == null)
                {
                    heldMagazines[itemScript.pos[0]] = null;
                    itemScript.pos[0] = itemScript.tempPos[0];
                    heldMagazines[itemScript.pos[0]] = selectedInvItem;
                }
            }
            else if (itemScript.size == 2)
            {
                int size = 0;
                for (int i = 0; i < itemScript.size; i++)
                {
                    if (itemScript.tempPos[i] < invSpots.Length && itemScript.tempPos[i] >= 0)
                    {
                        if (heldMagazines[itemScript.tempPos[i]] == null || heldMagazines[itemScript.tempPos[i]] == selectedInvItem)
                        {
                            size++;
                        }
                    }
                }
                Debug.Log("size " + size);
                int axis = 0;
                if (itemScript.direction[0] || itemScript.direction[1])
                {
                    axis = invY;
                }
                else if (itemScript.direction[2] || itemScript.direction[3])
                {
                    axis = invX;
                }
                bool moveItem = false;
                if (size == itemScript.size && itemScript.tempPos[1] / axis == itemScript.tempPos[0] / axis)
                {
                    moveItem = true;
                }
                else if (size == itemScript.size && (itemScript.direction[3] || itemScript.direction[2]))
                {
                    moveItem = true;
                }
                if (moveItem)
                {
                    heldMagazines[itemScript.pos[0]] = null;
                    heldMagazines[itemScript.pos[1]] = null;
                    for (int i = 0; i <= size; i++)
                    {
                        itemScript.pos[i] = itemScript.tempPos[i];
                    }
                    heldMagazines[itemScript.pos[0]] = selectedInvItem;
                    heldMagazines[itemScript.pos[1]] = selectedInvItem;
                }
            }
            selectedInvItem.transform.parent = invTransforms[itemScript.pos[0]].transform;
            selectedInvItem.transform.position = invTransforms[itemScript.pos[0]].transform.position;
            selectedInvItem = null;
            //invItemSelected = true;
            moveInvItem = false;
            itemScript = null;
            for (int i = 0; i < invSpots.Length; i++)
            {
                invSpots[i].GetComponent<Image>().color = Color.white;
                if (i != inventoryPos)
                {
                    invSpots[i].SetActive(false);
                }
            }
        }

        /*
                     if (posChange > 0)
            {
                selectedInvItem.transform.position = invTransforms[newNum].position;
                itemScript.tempPos[0] = newNum;
                if(itemScript.size == 2)
                {
                    if(itemScript.direction[0])
                    {
                        itemScript.tempPos[1] = newNum + 1;
                    }
                    else if(itemScript.direction[1])
                    {
                        itemScript.tempPos[1] = newNum - 1;
                    }
                    else if (itemScript.direction[2])
                    {
                        itemScript.tempPos[1] = newNum + 2;
                    }
                    else if (itemScript.direction[3])
                    {
                        itemScript.tempPos[1] = newNum - 2;
                    }
                }
            }
            else if (posChange < 0)
            {
                selectedInvItem.transform.position = invTransforms[newNum].position;
                itemScript.tempPos[0] = newNum;
                if (itemScript.size == 2)
                {
                    if (posChange == -1)
                    {
                        itemScript.tempPos[1] = newNum + posDiff;
                    }
                    else
                    {
                        itemScript.tempPos[1] = newNum + posDiff;
                    }
                }
            }*/





        /* if(!invItemSelected)
         {
             if (!firstItemToSwap)
             {
                 firstItemToSwap = true;
                 firstItemSwapPos = inventoryPos;
             }
             else
             {
                 GameObject item1;
                 GameObject item2;
                 if (heldMagazines[firstItemSwapPos] != null)
                 {
                     item1 = heldMagazines[firstItemSwapPos];
                 }
                 else
                 {
                     item1 = null;
                 }
                 if(heldMagazines[inventoryPos] != null)
                 {
                     item2 = heldMagazines[inventoryPos];
                 }
                 else
                 {
                     item2 = null;
                 }
                 if(item1 != null)
                 {
                     item1.transform.position = invTransforms[inventoryPos].position;
                     item1.transform.SetParent(invTransforms[inventoryPos]);
                 }
                 if(item2 != null)
                 {
                     item2.transform.position = invTransforms[firstItemSwapPos].position;
                     item2.transform.SetParent(invTransforms[firstItemSwapPos]);
                 }
                 heldMagazines[firstItemSwapPos] = item2;
                 heldMagazines[inventoryPos] = item1;
                 firstItemToSwap = false;
             }
         }*/

    }

    public void dropInvItem() //removes item from inventory
    {
        if (selectedInvItem.CompareTag("magazine"))
        {
            selectedInvItem.transform.position = dropItemPos.position;
            selectedInvItem.transform.parent = null;
            selectedInvItem.GetComponent<Rigidbody>().useGravity = true;
            selectedInvItem.GetComponent<Collider>().enabled = true;
            heldMagazines[inventoryPos] = null;
            magScript = null;
            selectedInvItem = null;
            invItemSelected = false;
            invSpotsTaken--;
        }
        else if (selectedInvItem.CompareTag("b50"))
        {
            int bulletCount = selectedInvItem.GetComponent<bulletMultipleInventoryManagement>().currentBulletCount;
            int bulletDiv = bulletCount % 10;
            if (bulletDiv == 0)
            {
                looseBulletCount -= 10;
                selectedInvItem.GetComponent<bulletMultipleInventoryManagement>().removeTen();
                GameObject instBullet = Instantiate(ammoBundleTen, dropItemPos.position, Quaternion.identity);
                instBullet.GetComponent<Rigidbody>().useGravity = true;
                bulletCount -= 10;
            }
            else
            {
                looseBulletCount -= bulletCount % 10;
                selectedInvItem.GetComponent<bulletMultipleInventoryManagement>().removeTen();
                GameObject instBullet = Instantiate(ammoBundleTen, dropItemPos.position, Quaternion.identity);
                instBullet.GetComponent<bulletPickup>().changeBullets(bulletDiv);
                bulletCount -= bulletCount % 10;
            }
            if (bulletCount <= 0)
            {
                heldMagazines[inventoryPos] = null;
                selectedInvItem = null;
                invItemSelected = false;
                Destroy(ammoInInv[currentAmmoInvPos]);
                ammoInInv.RemoveAt(currentAmmoInvPos);
                invSpotsTaken--;
            }
        }
    }

    public void bulletPickupCollected(int bullets)
    {
        for (int i = 0; i < bullets; i++)
        {
            pickingUpAmmo = true;
            changeBulletsInMag(-1);
        }
        pickingUpAmmo = false;
    }
}

