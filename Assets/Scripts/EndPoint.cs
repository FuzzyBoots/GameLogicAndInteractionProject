using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EndPoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<AI>())
        {
            GetComponent<AudioSource>().Play();
            other.gameObject.SetActive(false);

            GameManager.Instance.AdjustEnemiesEscaped(1);
            GameManager.Instance.AdjustScore(-10);
        }
    }
}
