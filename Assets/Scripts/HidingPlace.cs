using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingPlace : MonoBehaviour
{
    [SerializeField] bool inUse;

    public bool InUse { get => inUse; set => inUse = value; }
    // Start is called before the first frame update
}
