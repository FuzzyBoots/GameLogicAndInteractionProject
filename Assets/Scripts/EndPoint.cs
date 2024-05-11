using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Contact with {other.name}");
        if (other.gameObject.GetComponent<AI>())
        {
            other.gameObject.SetActive(false);
            // Decrement score?
        }
    }
}
