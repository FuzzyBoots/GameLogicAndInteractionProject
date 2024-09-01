using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EndPoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entered the end zone");
        if (other.gameObject.TryGetComponent<AI>(out AI aiObj))
        {
            GetComponent<AudioSource>().Play();
            SpawnManager.Instance.DeactivateInstance(aiObj);

            GameManager.Instance.AdjustEnemiesEscaped(1);
            GameManager.Instance.AdjustScore(-10);
        }
    }
}
