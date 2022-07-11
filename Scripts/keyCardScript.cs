using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class keyCardScript : MonoBehaviour
{
    public bool redKeyCard;
    public bool yellowKeyCard;
    public bool blueKeyCard;

    private void FixedUpdate()
    {
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z + 5);
    }
}
