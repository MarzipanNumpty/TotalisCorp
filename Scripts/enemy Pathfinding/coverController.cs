using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coverController : MonoBehaviour
{
    #region Singleton

    public static coverController instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion

    public bool combatStartedwh1;
    public bool combatStartedwh2;
    public bool combatStartedwh3;
    public bool combatStartedl1;
    public bool combatStartedl2;
    public bool combatStarted;
    public bool combatStartedGC;
    public List<GameObject> coverObjswh1;
    public List<GameObject> standingPoswh1;
    public List<GameObject> coverObjswh2;
    public List<GameObject> standingPoswh2;
    public List<GameObject> coverObjswh3;
    public List<GameObject> standingPoswh3;
    public List<GameObject> coverObjsl1;
    public List<GameObject> standingPosl1;
    public List<GameObject> coverObjsl2;
    public List<GameObject> standingPosl2;
    public List<GameObject> coverObjs;
    public List<GameObject> standingPos;

    void Start()
    {
        GameObject[] cov = (GameObject.FindGameObjectsWithTag("cover"));
        foreach (GameObject a in cov)
        {
            coverObjs.Add(a);
        }

        GameObject[] cov2 = (GameObject.FindGameObjectsWithTag("standingPos"));
        foreach (GameObject b in cov2)
        {
            standingPos.Add(b);
        }
    }
}
