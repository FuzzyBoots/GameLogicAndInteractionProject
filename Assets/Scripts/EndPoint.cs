using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<AI>())
        {
            other.gameObject.SetActive(false);
            // Decrement score?
        }
    }
}
