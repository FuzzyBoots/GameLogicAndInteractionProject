using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceBarrier : MonoBehaviour, IShootable
{
    [SerializeField] private float _fadeTime;
    [SerializeField] private float _reactivationDelay;

    public void HandleShot()
    {
        DeactivateBarrier();
    }

    private void DeactivateBarrier()
    {
        foreach (Renderer renderer in GetComponents<Renderer>())
        {
            renderer.enabled = false;
        }

        gameObject.layer = LayerMask.NameToLayer("Default");

        StartCoroutine(DelayedReactivation());
    }

    private void ActivateBarrier()
    {
        foreach (Renderer renderer in GetComponents<Renderer>())
        {
            renderer.enabled = true;
        }

        gameObject.layer = LayerMask.NameToLayer("PossibleCover");
    }

    IEnumerator DelayedReactivation ()
    {
        yield return new WaitForSeconds(_reactivationDelay);

        ActivateBarrier();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
