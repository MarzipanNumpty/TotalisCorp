using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class AmmoHandler : MonoBehaviour
{
    //inventory related variables
    [SerializeField] GameObject[] heldMagazines;
    [SerializeField] GameObject[] invSpots;
    [SerializeField] GameObject[] largerInvSpots;
    [SerializeField] Transform[] invTransforms;
    [SerializeField] Transform[] largeInvTransforms;
    [SerializeField] GameObject inventoryObject;
    [SerializeField] Transform invWorldPos;
    [SerializeField] Transform selectedItemWorldPos;
    [SerializeField] Transform dropItemPos;
    [SerializeField] GameObject healPos;
    int inventoryPos;
    bool invOpen;
    public bool invItemSelected;
    public GameObject selectedInvItem;
    magazineScript magScript;
    public int looseBulletCount = 0;
    public int currentMagSize = 0;
    bool firstItemToSwap;
    int firstItemSwapPos;
    int invSpotsTaken = 0;
    int invX = 3;
    int invY = 3;
    public bool moveInvItem;
    itemSize itemScript;
    public bool largeInv;
    PlayerScript playerScript;
    [SerializeField] GameObject pistolPrefab;
    [SerializeField] GameObject meleePrefab;
    [SerializeField] Material greenMat;
    [SerializeField] Material redMat;
    [SerializeField] Material whiteMat;
    [SerializeField] GameObject[] invPrompts; //0 = default //1 = swap prompts //2 = select prompts


    //shooting related variables
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] GameObject bullet;
    [SerializeField] Transform bulletSpawnPos;
    [SerializeField] Transform[] bulletAlternatingSpawns; //1 waist pos, 2 aimed pos
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

    [SerializeField] Text ammoCountText;
    [SerializeField] Text reserveAmmoText;
    [SerializeField] Text grenadeCountText;
    [SerializeField] LayerMask hitLayer;
    void Start()
    {
        ammoCountText.text = currentMagSize.ToString();
        reserveAmmoText.text = looseBulletCount.ToString();
        grenadeCountText.text = grenadeCount.ToString();
        inventoryObject.SetActive(false);
        anim = GetComponent<Animator>();
        playerScript = GetComponent<PlayerScript>();
        if(largeInv)
        {
            invY = 3;
            invX = 3;
        }
        else
        {
            invY = 2;
            invX = 2;
        }
    }

    public void changeFirePos(int arrayPos)
    {
        bulletSpawnPos = bulletAlternatingSpawns[arrayPos];
    }

    public void shoot()
    {
        if(magInGun && !reloading)
        {
            magScript = currentMag.GetComponent<magazineScript>();
            if(magScript.bulletsInMag > 0)
            {
                Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, hitLayer))
                {
                    GameObject CurrentBullet = Instantiate(bullet, bulletSpawnPos.position, Quaternion.identity);
                    CurrentBullet.transform.localEulerAngles = new Vector3(cam.transform.localEulerAngles.x, transform.localEulerAngles.y, 0);
                    Debug.Log(hit.transform.gameObject.name + "object name");
                    CurrentBullet.transform.LookAt(hit.point, Vector3.forward);
                }
                magScript.bulletsInMag--;
                currentMagSize = magScript.bulletsInMag;
                ammoCountText.text = currentMagSize.ToString();
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
        if(currentMag == null)
        {
            alreadyEmpty = true;
        }
        for (int i = 0; i < heldMagazines.Length; i++)
        {
            if(heldMagazines[i] != null && heldMagazines[i] != currentMag && heldMagazines[i].CompareTag("magazine"))
            {
                if(heldMagazines[i].GetComponent<magazineScript>().bulletsInMag > 0)
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
        if(newMag != null && !alreadyEmpty)
        {
            looseBulletCount += currentMagSize;
            anim.SetBool("fullReload", true);
            currentMag = newMag;
            magInGun = true;
            currentMagSize = currentMag.GetComponent<magazineScript>().bulletsInMag;
            looseBulletCount -= currentMagSize;
        }
        else if(newMag != null && alreadyEmpty)
        {
            anim.SetBool("fillMag", true);
            currentMag = newMag;
            magInGun = true;
            currentMagSize = currentMag.GetComponent<magazineScript>().bulletsInMag;
            looseBulletCount -= currentMagSize;
        }
        else if(!alreadyEmpty)
        {
            anim.SetBool("emptyMag", true);
            magInGun = false;
            if(currentMag != null)
            {
                currentMagSize = currentMag.GetComponent<magazineScript>().bulletsInMag;
                looseBulletCount += currentMag.GetComponent<magazineScript>().bulletsInMag;
                currentMagSize = 0;
            }
            currentMag = newMag;
        }
        ammoCountText.text = currentMagSize.ToString();
        reserveAmmoText.text = looseBulletCount.ToString();
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
        if(grenadeCount > 0)
        {
            GameObject currentGreande = Instantiate(grenade, grenadeSpawnPos.position, Quaternion.identity);
            currentGreande.transform.localEulerAngles = new Vector3(cam.transform.localEulerAngles.x, transform.localEulerAngles.y, 0);
            for(int i = 0; i < heldMagazines.Length; i++)
            {
                if(heldMagazines[i].CompareTag("grenade"))
                {
                    Destroy(heldMagazines[i]);
                    heldMagazines[i] = null;
                    selectedInvItem = null;
                    invItemSelected = false;
                    invSpotsTaken--;
                    grenadeCount--;
                    grenadeCountText.text = grenadeCount.ToString();
                    break;
                }
            }
        }
    }

    public void selectinvPrompts(int selectedPrompt)
    {
        for(int i = 0; i < invPrompts.Length; i++)
        {
            if(i != selectedPrompt)
            {
                invPrompts[i].SetActive(false);
            }
            else
            {
                invPrompts[i].SetActive(true);
            }
        }
    }

    public void openInv() //this spawns the inventory in the world and despawns
    {
        GameObject[] invLights;
        Transform[] invPositions;
        if (largeInv)
        {
            invLights = largerInvSpots;
            invPositions = largeInvTransforms;
        }
        else
        {
            invLights = invSpots;
            invPositions = invTransforms;
        }
        if (invOpen)
        {
            inventoryObject.SetActive(false);
            invOpen = false;
            firstItemToSwap = false;
            if(selectedInvItem != null)
            {
                selectedInvItem.transform.position = invPositions[inventoryPos].position;
            }
            if(invItemSelected)
            {
                selectInvItem(true);
            }
            if(moveInvItem)
            {
                swapInventorySlots(true);
            }
            magScript = null;
            selectedInvItem = null;
            invItemSelected = false;
            invLights[inventoryPos].SetActive(false);
            invLights[0].SetActive(true);
            inventoryPos = 0;
        }
        else
        {
            selectinvPrompts(0);
            inventoryObject.transform.position = invWorldPos.position;
            inventoryObject.transform.localEulerAngles = new Vector3(270, transform.localEulerAngles.y, 0);
            inventoryObject.SetActive(true);
            invOpen = true;
        }
        if (largeInv)
        {
            largerInvSpots = invLights;
        }
        else
        {
            invSpots = invLights;
        }
    }

    public void addMagazine(GameObject newMag) //this adds magazines to the inventory
    {
        Transform[] invPositions;
        int arrayLength = 0;
        if (largeInv)
        {
            invPositions = largeInvTransforms;
            arrayLength = 9;
        }
        else
        {
            invPositions = invTransforms;
            arrayLength = 4;
        }
        int size = newMag.GetComponent<itemSize>().size;
        if(size == 4)
        {
            size--;
        }
        int remainingInvSpots = heldMagazines.Length - invSpotsTaken;
        Debug.Log(remainingInvSpots + "spots");
        if (remainingInvSpots >= size && (size != 3 || largeInv))
        {
            size = newMag.GetComponent<itemSize>().size;
            for (int i = 0; i < arrayLength; i++)
            {
                if (heldMagazines[i] == null)
                {
                    Debug.Log(i);
                    if (size == 1)
                    {
                        heldMagazines[i] = newMag.transform.gameObject;
                        newMag.transform.position = invPositions[i].position;
                        newMag.transform.localEulerAngles = new Vector3(0, 0, 0);
                        newMag.transform.SetParent(invPositions[i]);
                        if (newMag.CompareTag("magazine"))
                        {
                            newMag.GetComponent<Rigidbody>().useGravity = false;
                        }
                        invSpotsTaken++;
                        newMag.GetComponent<itemSize>().pos[0] = i;
                        if(newMag.tag != "b50")
                        {
                            newMag.GetComponent<Collider>().enabled = false;
                        }
                        if(newMag.CompareTag("grenade"))
                        {
                            grenadeCount++;
                            grenadeCountText.text = grenadeCount.ToString();
                            newMag.GetComponent<Rigidbody>().useGravity = false;
                        }
                        newMag.transform.localEulerAngles = new Vector3(0, 90, 90);
                        changeLayer(newMag, 5);
                        break;
                    }
                    else if (size >= 2)
                    {
                        int remainder1 = i / invY;
                        int remainder2 = (i + 1) / invY;
                        int remainder3 = (i + 2) / invY;
                        //Debug.Log("remainder 1: " + remainder1 + "remainder 2: " + remainder2 + "remainder 3: " + remainder3);
                        bool addObject = false;
                        int pos1 = 0;
                        int pos2 = 0;
                        int pos3 = 0;
                        int pos4 = 0;
                        int sizeThreePos = i + invY + invY;
                        if(i + 2 < heldMagazines.Length && size == 3)
                        {
                            if (size == 3 && heldMagazines[i + 2] == null && invY >= 3)
                            {
                                pos3 = i + 2;
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else if(size == 3)
                        {
                            continue;
                        }
                        bool stopeth = false;
                        bool up = false;
                        bool flip = false;
                        int rotation = 0;
                        int numCheck = i + 1;
                        if (size == 4)
                        {
                           // Debug.Log("1");
                            if(i + 1 + invY >= 0 && i + 1 + invY < heldMagazines.Length)
                            {
                                if(heldMagazines[i + 1 + invY] == null)
                                {
                                    pos4 = i + 1 + invY;
                                    pos2 = i + 1;
                                    stopeth = true;
                                    remainder3 = pos4 / invY;
                                    rotation = 90;
                                    up = true;
                                }
                            }
                            if (i + 1 - invY >= 0 && i + 1 - invY < heldMagazines.Length && !stopeth)
                            {
                                if (heldMagazines[i + 1 - invY] == null)
                                {
                                    pos4 = i + 1 - invY;
                                    pos2 = i - invY;
                                    stopeth = true;
                                    up = true;
                                    remainder3 = pos4 / invY;
                                    numCheck = i - invY;
                                    flip = true;
                                }
                            }
                           /* if (i + invY >= 0 && i + invY < heldMagazines.Length && !stopeth)
                            {
                                if (heldMagazines[i + invY] == null)
                                {
                                    pos4 = i + invY;
                                    stopeth = true;
                                    flip = true;
                                }                             
                            }
                            if (i - invY >= 0 && i - invY < heldMagazines.Length && !stopeth)
                            {
                                if (heldMagazines[i - invY] == null)
                                {
                                    pos4 = i - invY;
                                }
                            }*/
                        }
                      //  Debug.Log(stopeth + "bfdjhbfja");
                      //  Debug.Log(remainder1 + "r1");
                      //  Debug.Log(remainder2 + "r2");
                       // Debug.Log(remainder3 + "r3");
                        if (remainder1 == remainder2 || (remainder1 == remainder3 && size != 3))
                        {
                         //   Debug.Log("if");
                            if (heldMagazines[numCheck] == null)
                            {
                                pos1 = i;
                                if(size != 4)
                                {
                                    pos2 = i + 1;
                                }
                                if (stopeth || size != 4)
                                {
                                    if(size == 3)
                                    {
                                        if(remainder1 == remainder3)
                                        {
                                            addObject = true;
                                        }
                                    }
                                    else
                                    {
                                        addObject = true;
                                    }
                                }

                                if(size != 4)
                                {
                                    newMag.GetComponent<itemSize>().direction[0] = true;
                                    rotation = 90;
                                }
                                else
                                {
                                    if(up)
                                    {
                                        newMag.GetComponent<itemSize>().direction[0] = true;
                                    }
                                    else
                                    {
                                        newMag.GetComponent<itemSize>().direction[1] = true;
                                    }

                                    if(flip)
                                    {
                                        newMag.GetComponent<itemSize>().flipped = true;
                                    }
                                }
                            }
                        }
                      //  Debug.Log(addObject + "addobject");
                        bool passthrough = false;
                        if(i - 1 + invY < heldMagazines.Length || i - 1 - invY >= 0)
                        {
                            passthrough = true;
                        }
                        Debug.Log(addObject + "wrench check");
                        if(passthrough && addObject == false)
                        {
                            if (size == 4)
                            {
                          //      Debug.Log("2");
                                bool stop = false;
                                if (i - 1 + invY >= 0 && i - 1 + invY < heldMagazines.Length && i + invY - 1 == i - 1 + invY)
                                {
                            //        Debug.Log("21");
                                    if (heldMagazines[i - 1 + invY] == null)
                                    {
                                //       Debug.Log("214212121212121");
                                        pos4 = i - 1 + invY;
                                        stop = true;
                                        up = true;
                                        flip = true;
                                        rotation = 180;
                                        pos2 = i + invY;
                                    }
                                }
                            //    Debug.Log(i - 1 - invY + "calculation" + i);
                                if (i - 1 - invY >= 0 && i + 1 - invY < heldMagazines.Length && !stop)
                                {
                             //       Debug.Log("22");
                                    if (heldMagazines[i - 1 - invY] == null)
                                    {
                                        pos4 = i - 1 - invY;
                                        stop = true;
                                        up = true;
                                        flip = false;
                                        rotation = 270;
                                        pos2 = i - 1;
                                    }
                                }
                              /*  if (i + 1 >= 0 && i + 1 < heldMagazines.Length && !stop)
                                {
                                    Debug.Log("23");
                                    if (heldMagazines[i + 1] == null)
                                    {
                                        pos4 = i + 1;
                                        stop = true;
                                        up = false;
                                        flip = true;
                                    }
                                }
                                if (i - 1 >= 0 && i - 1 < heldMagazines.Length && !stop)
                                {
                                    Debug.Log("24");
                                    if (heldMagazines[i - 1] == null)
                                    {
                                        pos4 = i - 1;
                                        stop = true;
                                        up = false;
                                        flip = false;
                                    }
                                }*/
                                if(stop || size != 4)
                                {
                                    addObject = true;
                                }
                           //    Debug.Log(pos4);
                            }
                            if (size == 3 && invY >= 3)
                            {
                           //     Debug.Log("else1");
                                if (sizeThreePos >= heldMagazines.Length)
                                {
                                    continue;
                                }
                                if (heldMagazines[i + invY + invY] == null)
                                {
                                    pos3 = i + invY + invY;
                                }
                                pos2 = i + invY;
                            }
                          //  Debug.Log("else");
                            pos1 = i;
                            if(size != 4)
                            {
                                newMag.GetComponent<itemSize>().direction[1] = true;
                                rotation = 180;
                                addObject = true;
                                pos2 = i + invY;
                            }
                            else
                            {
                                if(up)
                                {
                                    newMag.GetComponent<itemSize>().direction[3] = true;
                                }
                                else
                                {
                                    newMag.GetComponent<itemSize>().direction[2] = true;
                                }

                                if(flip)
                                {
                                    newMag.GetComponent<itemSize>().flipped = true;
                                }
                            }
                        }
                      //  Debug.Log("pos1" + pos1 + "pos2" + pos2 + "pos4" + pos4);
                        if (addObject)
                        {
                            changeLayer(newMag, 5);
                            heldMagazines[pos1] = newMag;
                            heldMagazines[pos2] = newMag;
                            newMag.transform.position = invPositions[pos1].position;
                            newMag.transform.localEulerAngles = new Vector3(0, 0, 0);
                            newMag.transform.SetParent(invPositions[pos1]);
                            invSpotsTaken += 2;
                            newMag.GetComponent<itemSize>().pos[0] = pos1;
                            newMag.GetComponent<itemSize>().pos[1] = pos2;
                            newMag.GetComponent<Collider>().enabled = false;
                            newMag.GetComponent<Rigidbody>().useGravity = false;
                            if (size == 3)
                            {
                                heldMagazines[pos3] = newMag;
                                invSpotsTaken++;
                                newMag.GetComponent<itemSize>().pos[2] = pos3;
                            }
                            if (size == 4)
                            {
                       //         Debug.Log(pos4 + "boiiiizjsdj");
                                heldMagazines[pos4] = newMag;
                                invSpotsTaken++;
                                newMag.GetComponent<itemSize>().pos[2] = pos4;
                            }
                            newMag.transform.localEulerAngles = new Vector3(0, rotation, 90);
                            for(int j = 0; j < newMag.GetComponent<itemSize>().tempDirection.Length; j++)
                            {
                                newMag.GetComponent<itemSize>().tempDirection[j] = newMag.GetComponent<itemSize>().direction[j];
                            }
                            break;
                        }
                        /*else if(i == heldMagazines.Length - 1)
                        {
                            newMag.transform.position = dropItemPos.position;
                        }*/
                    }

                }
            }
        }
    }

    public void selectInvItem(bool unSelect = false) //this is used to specificy which inventory item you want to intereact with
    {
        if (heldMagazines[inventoryPos] != null && !moveInvItem && (invItemSelected || unSelect == false))
        {
            Transform[] invPositions;
            if (largeInv)
            {
                invPositions = largeInvTransforms;
            }
            else
            {
                invPositions = invTransforms;
            }
            if (invItemSelected)
            {
                selectinvPrompts(0);
                if (selectedInvItem.CompareTag("weapon") && unSelect == false)
                {
                    int size = selectedInvItem.GetComponent<itemSize>().size;
                    bool removeItem = false;
                    bool addItem = false;
                    if (!playerScript.haveWeapon)
                    {
                        if (size == 4)
                        {
                            playerScript.weaponPistol = true;
                            playerScript.ChangeWeapon(2);
                        }
                        else if (size == 3)
                        {
                            playerScript.weaponMelee = true;
                            playerScript.ChangeWeapon(1);
                        }
                        playerScript.haveWeapon = true;
                        removeItem = true;
                        playerScript.weaponSize = size;
                    }
                    else if (invSpotsTaken - playerScript.weaponSize >= 0)
                    {
                        addItem = true;
                    }
                    else
                    {
                        GameObject newInvObj = null;
                        if (playerScript.weaponSize == 3)
                        {
                            newInvObj = Instantiate(meleePrefab, dropItemPos.position, Quaternion.identity);
                            playerScript.weaponPistol = true;
                            playerScript.ChangeWeapon(2);
                        }
                        else if (playerScript.weaponSize == 4)
                        {
                            newInvObj = Instantiate(pistolPrefab, dropItemPos.position, Quaternion.identity);
                            playerScript.weaponMelee = true;
                            playerScript.ChangeWeapon(1);
                        }
                        removeItem = true;
                        newInvObj.transform.position = dropItemPos.position;
                        newInvObj.transform.parent = null;
                        newInvObj.GetComponent<Rigidbody>().useGravity = true;
                        newInvObj.GetComponent<Collider>().enabled = true;
                    }
                    Debug.Log(invSpotsTaken + "yeah boiiiiiiiiiiiiii");

                    if (removeItem || addItem)
                    {
                        for (int i = 0; i < heldMagazines.Length; i++)
                        {
                            if (heldMagazines[i] == selectedInvItem)
                            {
                                heldMagazines[i] = null;
                                invSpotsTaken--;
                            }
                        }
                        Destroy(selectedInvItem);
                    }
                    Debug.Log(invSpotsTaken + "yeah boyyyyyyyyyy");
                    if (addItem)
                    {
                        GameObject newInvObj = null;
                        playerScript.weaponMelee = false;
                        playerScript.weaponPistol = false;
                        if(playerScript.weaponSize == 3)
                        {
                            newInvObj = Instantiate(meleePrefab, dropItemPos.position, Quaternion.identity);
                            playerScript.weaponPistol = true;
                            playerScript.ChangeWeapon(2);
                        }
                        else if(playerScript.weaponSize == 4)
                        {
                            newInvObj = Instantiate(pistolPrefab, dropItemPos.position, Quaternion.identity);
                            playerScript.weaponMelee = true;
                            playerScript.ChangeWeapon(1);
                        }
                        addMagazine(newInvObj);
                        playerScript.weaponSize = size;
                    }
                }
                else if(selectedInvItem.CompareTag("healing") && unSelect == false)
                {
                    if(playerScript.health < 100)
                    {
                        playerScript.healDamage(3);
                        for (int i = 0; i < heldMagazines.Length; i++)
                        {
                            if (heldMagazines[i] == selectedInvItem)
                            {
                                heldMagazines[i] = null;
                                invSpotsTaken--;
                            }
                        }
                        selectedInvItem.transform.parent = healPos.transform;
                        healPos.transform.localEulerAngles = new Vector3(60, 0, 0);
                        selectedInvItem.transform.localPosition = new Vector3(0, 0, 0);
                        selectedInvItem.GetComponent<Animator>().SetBool("heal",true);
                    }
                    else
                    {
                        selectedInvItem.transform.position = invPositions[selectedInvItem.GetComponent<itemSize>().pos[0]].position;
                    }
                }
                else
                {
                    selectedInvItem.transform.position = invPositions[selectedInvItem.GetComponent<itemSize>().pos[0]].position;
                }
                magScript = null;
                selectedInvItem = null;
                invItemSelected = false;
            }
            else
            {
                selectinvPrompts(1);
                selectedInvItem = heldMagazines[inventoryPos];
                selectedInvItem.transform.position = selectedItemWorldPos.position;
                if(selectedInvItem.CompareTag("magazine"))
                {
                    magScript = selectedInvItem.GetComponent<magazineScript>();
                }
                invItemSelected = true;
            }
        }
    }

    public void changeBulletsInMag(int changeBullets, bool playAnim = false) //this changes the amount of bullets in the selected magazine
    {
        int newNum = 0;
        bool affectBullets = false;
        bool magScriptExists = false;
        if(magScript != null)
        {
            magScriptExists = true;
        }
        if(magScriptExists && !pickingUpAmmo)
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
        if(affectBullets || pickingUpAmmo)
        {
            if (changeBullets > 0 && looseBulletCount > 0)
            {
                if(playAnim)
                {
                    Animator bulletAnim = selectedInvItem.GetComponentInChildren<Animator>();
                    bulletAnim.SetBool("reload", true);
                }
                looseBulletCount--;
                int currentBulletCount = ammoInInv[currentAmmoInvPos].GetComponent<bulletMultipleInventoryManagement>().currentBulletCount;
                if (removeAmmo)
                {
                    if(ammoInInv[currentAmmoInvPos] != selectedInvItem)
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
                    for(int i = 0; i < heldMagazines.Length; i++)
                    {
                        if(heldMagazines[i] == ammoInInv[currentAmmoInvPos])
                        {
                            heldMagazines[i] = null;
                        }
                    }
                    Destroy(ammoInInv[currentAmmoInvPos]);
                    ammoInInv.RemoveAt(currentAmmoInvPos);
                    invSpotsTaken--;
                    if(looseBulletCount != 0)
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
                if(magScriptExists)
                {
                    if(magScript.bulletsInMag <= 0)
                    {
                        stopTakingAmmo = true;
                    }
                }
                if(invSpotsTaken == heldMagazines.Length)
                {
                    stopTakingAmmo = ammoInInv[currentAmmoInvPos].GetComponent<bulletMultipleInventoryManagement>().isFull;
                }
                if(!stopTakingAmmo)
                {
                    if(playAnim)
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
                    if(magScriptExists)
                    {
                        magScript.bulletsInMag = newNum;
                    }
                    ammoInInv[currentAmmoInvPos].GetComponent<bulletMultipleInventoryManagement>().changeBullets(looseBulletCount, false);
                }
            }
        }
        reserveAmmoText.text = looseBulletCount.ToString();
    }

    public void inventoryChange(int posChange, int rotation) //this changes which inventory slot is highlighted
    {
        //Debug.Log("changein" + posChange);
        GameObject[] invLights;
        Transform[] invPositions;
        if (largeInv)
        {
            invLights = largerInvSpots;
            invPositions = largeInvTransforms;
            invX = 3;
            invY = 3;
        }
        else
        {
            invLights = invSpots;
            invPositions = invTransforms;
        }
        if(itemScript != null)
        {
            if((itemScript.tempPos[1] < invLights.Length) && (itemScript.tempPos[1] >= 0))
            {
                invLights[itemScript.tempPos[1]].SetActive(false);
                invLights[itemScript.tempPos[1]].GetComponent<Image>().material = greenMat;
            }
            if ((itemScript.tempPos[2] < invLights.Length) && (itemScript.tempPos[2] >= 0))
            {
                invLights[itemScript.tempPos[2]].SetActive(false);
                invLights[itemScript.tempPos[2]].GetComponent<Image>().material = greenMat;
            }
        }
        int previousNum = inventoryPos;
        int newNum = inventoryPos;
        if(posChange == 2)
        {
            newNum += invY;
        }
        else if(posChange == -2)
        {
            newNum -= invY;
        }
        else if(posChange == 1)
        {
            newNum += 1;
        }
        else if(posChange == -1)
        {
            newNum -= 1;
        }
        if (!moveInvItem)
        {
            if (newNum < 0)
            {
                inventoryPos = 0;
                newNum = 0;
            }
            else if (newNum > invLights.Length - 1)
            {
                inventoryPos = invLights.Length - 1;
                newNum = invLights.Length - 1;
            }
            else
            {
                inventoryPos = newNum;
            }
            invLights[previousNum].SetActive(false);
            invLights[newNum].SetActive(true);
        }
        else
        {
            int size = itemScript.size;
            itemScript = selectedInvItem.GetComponent<itemSize>();
            if(rotation != 0)
            {
                int currentDir = 0;
                for (int i = 0; i < itemScript.tempDirection.Length; i++)
                {
                    if(itemScript.tempDirection[i] == true)
                    {
                        itemScript.tempDirection[i] = false;
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
                itemScript.tempDirection[currentDir] = true;
            }
            if (newNum > invLights.Length - 1)
            {
                newNum = invLights.Length - 1;
            }
            else if(newNum < 0)
            {
                newNum = 0;
            }
            inventoryPos = newNum;
            selectedInvItem.transform.position = invPositions[newNum].position;
            itemScript.tempPos[0] = newNum;
            bool showSquareFirstCheck = false;
            bool showSquareFirstCheck2 = false;
            bool showSquareSecondCheck = false;
            bool showSquareThirdCheck = false;
            bool showSquareFourthCheck = false;
            int axis = 0;
            bool horizontal = false;
            bool size4Horizontal = false;
            int itemRotation = 0;
            if (size >= 2)
            {
                if (itemScript.tempDirection[0])
                {
                    axis = invY;
                    itemRotation = 90;
                    itemScript.tempPos[1] = newNum + 1;
                    if(size == 3)
                    {
                        itemScript.tempPos[2] = newNum + 2;
                    }
                    if(size == 4)
                    {
                        if(itemScript.flipped)
                        {
                            itemScript.tempPos[2] = newNum + 1 - invY;
                        }
                        else
                        {
                            itemScript.tempPos[2] = newNum + 1 + invY;
                        }
                    }
                }
                else if (itemScript.tempDirection[2])
                {
                    axis = invY;
                    itemRotation = 270;
                    itemScript.tempPos[1] = newNum - 1;
                    if (size == 3)
                    {
                        itemScript.tempPos[2] = newNum - 2;
                    }
                    if (size == 4)
                    {
                        if (itemScript.flipped)
                        {
                            itemScript.tempPos[2] = newNum - 1 + invY;
                        }
                        else
                        {
                            itemScript.tempPos[2] = newNum - 1 - invY;
                        }
                    }
                }
                else if (itemScript.tempDirection[3])
                {
                    axis = invX;
                    horizontal = true;
                    itemRotation = 0;
                    itemScript.tempPos[1] = newNum - invY;
                    if (size == 3)
                    {
                        itemScript.tempPos[2] = newNum - invY - invY;
                    }
                    if (size == 4)
                    {
                        if (itemScript.flipped)
                        {
                            itemScript.tempPos[2] = newNum - invY - 1;
                        }
                        else
                        {
                            itemScript.tempPos[2] = newNum - invY + 1;
                        }
                    }
                    if (itemScript.tempPos[2] / axis == itemScript.tempPos[1] / axis && (itemScript.tempPos[2] != 0 && itemScript.tempPos[1] != 0))
                    {
                        size4Horizontal = true;
                    }
                }
                else if (itemScript.tempDirection[1])
                {
                    axis = invX;
                    horizontal = true;
                    itemRotation = 180;
                    itemScript.tempPos[1] = newNum + invY;
                    if (size == 3)
                    {
                        itemScript.tempPos[2] = newNum + invY + invY;
                    }
                    if (size == 4)
                    {
                        if (itemScript.flipped)
                        {
                            itemScript.tempPos[2] = newNum + invY + 1;
                        }
                        else
                        {
                            itemScript.tempPos[2] = newNum + invY - 1;
                        }
                    }
                    if (itemScript.tempPos[2] / axis == itemScript.tempPos[1] / axis)
                    {
                        size4Horizontal = true;
                    }
                }
                selectedInvItem.transform.localEulerAngles = new Vector3(0, itemRotation, 90);
                bool size2 = false;
                bool size3 = false;
                if(itemScript.tempPos[1] / axis == itemScript.tempPos[0] / axis)
                {
                    size2 = true;
                }
                if(itemScript.tempPos[0] / axis == itemScript.tempPos[2] / axis && size ==3)
                {
                    size3 = true;
                }
                if(itemScript.tempPos[2] >= 0 && itemScript.tempPos[2] < invLights.Length && size == 4)
                {
                    if (heldMagazines[itemScript.tempPos[2]] == null || heldMagazines[itemScript.tempPos[2]] == selectedInvItem)
                    {
                        showSquareFourthCheck = true;
                    }
                    else
                    {
                        showSquareFourthCheck = false;
                    }
                }
                if (size2 || horizontal)
                {
                    showSquareFirstCheck = true;
                    if ((itemScript.tempPos[1] < invLights.Length) && (itemScript.tempPos[1] >= 0))
                    {
                        if (heldMagazines[itemScript.tempPos[1]] != null && heldMagazines[itemScript.tempPos[1]] != selectedInvItem)
                        {
                            showSquareSecondCheck = true;
                        }
                    }
                }
                if((size3 || horizontal) && size == 3)
                {
                    showSquareFirstCheck2 = true;
                    if ((itemScript.tempPos[2] < invLights.Length) && (itemScript.tempPos[2] >= 0))
                    {
                        if (heldMagazines[itemScript.tempPos[2]] != null && heldMagazines[itemScript.tempPos[2]] != selectedInvItem)
                        {
                            showSquareThirdCheck = true;
                        }
                    }
                }
            }
            invLights[previousNum].SetActive(false);
            invLights[newNum].SetActive(true);
            if(heldMagazines[inventoryPos] == null || heldMagazines[itemScript.tempPos[0]] == selectedInvItem)
            {
                invLights[newNum].GetComponent<Image>().material = greenMat;
            }
            else
            {
                invLights[newNum].GetComponent<Image>().material = redMat;
            }
            if (size >= 2)
            {            
                if((itemScript.tempPos[1] < invLights.Length) && (itemScript.tempPos[1] >= 0) && showSquareFirstCheck && !showSquareSecondCheck)
                {
                    invLights[itemScript.tempPos[1]].SetActive(true);
                    invLights[itemScript.tempPos[1]].GetComponent<Image>().material = greenMat;
                }
                else if ((itemScript.tempPos[1] < invLights.Length) && (itemScript.tempPos[1] >= 0) && showSquareSecondCheck)
                {
                    invLights[itemScript.tempPos[1]].SetActive(true);
                    invLights[itemScript.tempPos[1]].GetComponent<Image>().material = redMat;
                }
                if ((itemScript.tempPos[2] < invLights.Length) && (itemScript.tempPos[2] >= 0))
                {
                    if(showSquareThirdCheck)
                    {
                        invLights[itemScript.tempPos[2]].SetActive(true);
                        invLights[itemScript.tempPos[2]].GetComponent<Image>().material = redMat;
                    }
                    else if(showSquareFirstCheck2)
                    {
                        invLights[itemScript.tempPos[2]].SetActive(true);
                        invLights[itemScript.tempPos[2]].GetComponent<Image>().material = greenMat;
                    }                   
                }
                if (showSquareFourthCheck && (showSquareSecondCheck || showSquareFirstCheck))
                {
                    if((horizontal && size4Horizontal) || !horizontal)
                    {
                        invLights[itemScript.tempPos[2]].SetActive(true);
                        invLights[itemScript.tempPos[2]].GetComponent<Image>().material = greenMat;
                    }
                }
                else if (size == 4 && (showSquareSecondCheck || showSquareFirstCheck) && itemScript.tempPos[2] >= 0 && itemScript.tempPos[2] < invLights.Length)
                {
                    if ((horizontal && size4Horizontal) || !horizontal)
                    {
                        invLights[itemScript.tempPos[2]].SetActive(true);
                        invLights[itemScript.tempPos[2]].GetComponent<Image>().material = redMat;
                    }
                }
            }
            if(largeInv)
            {
                largerInvSpots = invLights;
            }
            else
            {
                invSpots = invLights;
            }

            /*if(itemScript.tempPos[1] < invSpots.Length - 1 || itemScript.tempPos[1] >= 0)
            {
                invSpots[itemScript.tempPos[1]].SetActive(false);
                invSpots[itemScript.tempPos[1]].GetComponent<Image>().color = Color.white;
            }*/

        }
    }

    public void swapInventorySlots(bool exitInv = false) //swaps magazine spaces in inventory
    {
        GameObject[] invLights;
        Transform[] invPositions;
        if (largeInv)
        {
            invLights = largerInvSpots;
            invPositions = largeInvTransforms;
            invX = 3;
            invY = 3;
        }
        else
        {
            invLights = invSpots;
            invPositions = invTransforms;
        }
        if(!invItemSelected)
        {
            if (!moveInvItem && heldMagazines[inventoryPos] != null)
            {
                selectinvPrompts(2);
                selectedInvItem = heldMagazines[inventoryPos];
                //invItemSelected = true;
                moveInvItem = true;
                itemScript = selectedInvItem.GetComponent<itemSize>();
                //selectedInvItem.transform.parent = null;
                for (int i = 0; i < itemScript.size; i++)
                {
                    itemScript.tempPos[i] = itemScript.pos[i];
                }

            } //(heldMagazines[inventoryPos] == null || (heldMagazines[itemScript.pos[0]] == selectedInvItem && heldMagazines[itemScript.pos[1]] == selectedInvItem)) && 
            else if (itemScript != null && (heldMagazines[inventoryPos] == null || heldMagazines[inventoryPos] == selectedInvItem))
            {
                selectinvPrompts(0);
                bool moveItem = false;
                int size = 0;
                int arraySize = itemScript.size;
                int objSize = itemScript.size;
                int axis = 0;
                bool horizontal = false;
                if (objSize == 1)
                {
                    if (heldMagazines[itemScript.tempPos[0]] == null)
                    {
                        heldMagazines[itemScript.pos[0]] = null;
                        itemScript.pos[0] = itemScript.tempPos[0];
                        heldMagazines[itemScript.pos[0]] = selectedInvItem;
                    }
                }
                else if (objSize >= 2)
                {
                    if (arraySize == 4)
                    {
                        arraySize--;
                    }
                    for (int i = 0; i < arraySize; i++)
                    {
                        if (itemScript.tempPos[i] < invLights.Length && itemScript.tempPos[i] >= 0)
                        {
                            if (heldMagazines[itemScript.tempPos[i]] == null || heldMagazines[itemScript.tempPos[i]] == selectedInvItem)
                            {
                                size++;
                            }
                        }
                    }
                    Debug.Log("size " + size + "axiiiiii2" + objSize);
                    if (itemScript.tempDirection[0] || itemScript.tempDirection[2])
                    {
                        axis = invY;
                    }
                    else if (itemScript.tempDirection[1] || itemScript.tempDirection[3])
                    {
                        axis = invX;
                        horizontal = true;
                    }
                    Debug.Log(itemScript.tempPos[2] / axis);
                    Debug.Log(itemScript.tempPos[0] / axis);
                    Debug.Log("size " + size + "axiiiiii2" + objSize);
                    if (objSize == 3 && size == objSize && itemScript.tempPos[2] / axis == itemScript.tempPos[0] / axis)
                    {
                        moveItem = true;
                    }
                    else if (objSize == 3 && size == objSize && horizontal)
                    {
                        moveItem = true;
                    }
                    else if (size == objSize && itemScript.tempPos[1] / axis == itemScript.tempPos[0] / axis && objSize == 2)
                    {
                        moveItem = true;
                    }
                    else if (size == objSize && horizontal && objSize == 2)
                    {
                        moveItem = true;
                    }
                    else if (objSize == 4 && size == 3)
                    {
                        if (itemScript.tempPos[1] / axis == itemScript.tempPos[0] / axis && itemScript.tempPos[1] + invY == itemScript.tempPos[2])
                        {
                            moveItem = true;
                        }
                        else if (itemScript.tempPos[1] / axis == itemScript.tempPos[0] / axis && itemScript.tempPos[1] - invY == itemScript.tempPos[2])
                        {
                            moveItem = true;
                        }
                        else if (itemScript.tempPos[1] / axis == itemScript.tempPos[2] / axis && itemScript.tempPos[1] + invY == itemScript.tempPos[0])
                        {
                            moveItem = true;
                        }
                        else if (itemScript.tempPos[1] / axis == itemScript.tempPos[2] / axis && itemScript.tempPos[1] - invY == itemScript.tempPos[0])
                        {
                            moveItem = true;
                        }
                    }
                }
                if (moveItem && !exitInv)
                {
                    heldMagazines[itemScript.pos[0]] = null;
                    heldMagazines[itemScript.pos[1]] = null;
                    if (objSize == 3 || objSize == 4)
                    {
                        heldMagazines[itemScript.pos[2]] = null;
                    }
                    for (int i = 0; i <= size; i++)
                    {
                        itemScript.pos[i] = itemScript.tempPos[i];
                    }
                    heldMagazines[itemScript.pos[0]] = selectedInvItem;
                    heldMagazines[itemScript.pos[1]] = selectedInvItem;
                    if (objSize == 3 || objSize == 4)
                    {
                        heldMagazines[itemScript.pos[2]] = selectedInvItem;
                    }
                    for (int i = 0; i < itemScript.tempDirection.Length; i++)
                    {
                        itemScript.direction[i] = itemScript.tempDirection[i];
                    }
                }
                else
                {
                    int dir = 0;
                    for (int i = 0; i < itemScript.direction.Length; i++)
                    {
                        itemScript.tempPos[i] = itemScript.pos[i];
                        itemScript.tempDirection[i] = false;
                        if (itemScript.direction[i])
                        {
                            Debug.Log(i);
                            dir = i;
                        }
                    }
                    itemScript.tempDirection[dir] = true;
                    Debug.Log(dir);
                    if (dir == 0)
                    {
                        selectedInvItem.transform.localEulerAngles = new Vector3(0, 90, 90);
                        itemScript.direction[0] = true;
                    }
                    else if (dir == 1)
                    {
                        selectedInvItem.transform.localEulerAngles = new Vector3(0, 180, 90);
                        itemScript.direction[1] = true;
                    }
                    else if (dir == 2)
                    {
                        selectedInvItem.transform.localEulerAngles = new Vector3(0, 270, 90);
                        itemScript.direction[2] = true;
                    }
                    else if (dir == 3)
                    {
                        selectedInvItem.transform.localEulerAngles = new Vector3(0, 0, 90);
                        itemScript.direction[3] = true;
                    }
                }
                selectedInvItem.transform.parent = invPositions[itemScript.pos[0]].transform;
                selectedInvItem.transform.position = invPositions[itemScript.pos[0]].transform.position;
                selectedInvItem = null;
                //invItemSelected = true;
                moveInvItem = false;
                itemScript = null;
                for (int i = 0; i < invLights.Length; i++)
                {
                    invLights[i].GetComponent<Image>().material = whiteMat;
                    if (i != inventoryPos)
                    {
                        invLights[i].SetActive(false);
                    }
                }
                if (largeInv)
                {
                    largerInvSpots = invLights;
                }
                else
                {
                    invSpots = invLights;
                }
            }
        }


    }

    public void dropInvItem() //removes item from inventory
    {
        changeLayer(selectedInvItem, 0);
        if(selectedInvItem.CompareTag("magazine"))
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
        else if(selectedInvItem.CompareTag("b50"))
        {
            int bulletCount = selectedInvItem.GetComponent<bulletMultipleInventoryManagement>().currentBulletCount;
            int bulletDiv = bulletCount % 10;
            if(bulletDiv == 0)
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
            if(bulletCount <= 0)
            {
                heldMagazines[inventoryPos] = null;
                selectedInvItem = null;
                invItemSelected = false;
                Destroy(ammoInInv[currentAmmoInvPos]);
                ammoInInv.RemoveAt(currentAmmoInvPos);
                invSpotsTaken--;
            }
        }
        else
        {
            selectedInvItem.transform.position = dropItemPos.position;
            selectedInvItem.transform.parent = null;
            selectedInvItem.GetComponent<Rigidbody>().useGravity = true;
            selectedInvItem.GetComponent<Collider>().enabled = true;
            itemScript = selectedInvItem.GetComponent<itemSize>();
            for(int i = 0; i < itemScript.size; i++)
            {
                heldMagazines[itemScript.pos[i]] = null;
                if(itemScript.size == 4 && i == 2)
                {
                    break;
                }
            }
            if (selectedInvItem.transform.CompareTag("grenade"))
            {
                grenadeCount--;
                grenadeCountText.text = grenadeCount.ToString();
            }
            itemScript = null;
            magScript = null;
            selectedInvItem = null;
            invItemSelected = false;
            invSpotsTaken--;
        }
    }

    public void changeLayer(GameObject obj, int LayerNum)
    {
        obj.layer = 0;
        foreach (Transform child in obj.GetComponentsInChildren<Transform>(true))
        {
            child.gameObject.layer = LayerNum;
        }
    }

    public void bulletPickupCollected(int bullets)
    {
        for(int i = 0; i < bullets; i++)
        {
            pickingUpAmmo = true;
            changeBulletsInMag(-1);
        }
        pickingUpAmmo = false;
    }
}
