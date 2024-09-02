using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EndPoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<AI>(out AI aiObj))
        {
            Debug.Log("Entered the end zone", other.gameObject);
            GetComponent<AudioSource>().Play();
            SpawnManager.Instance.DeactivateInstance(aiObj);

            GameManager.Instance.AdjustEnemiesEscaped(1);
            GameManager.Instance.AdjustScore(-10);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log($"Staying {other.gameObject.name}", other.gameObject);
    }
}
