using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemSize : MonoBehaviour
{
    public int size;
    public int[] pos = new int[] { 0,0,0,0,0 };
    public int[] tempPos = new int[] { 0,0,0,0,0 };
    public bool[] direction = new bool[] { false, false, false, false }; //up/down/left/right
    public bool[] tempDirection = new bool[] { false, false, false, false }; //up/down/left/right
    public bool flipped;
}
