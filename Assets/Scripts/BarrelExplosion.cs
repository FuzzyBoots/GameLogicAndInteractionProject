using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelExplosion : MonoBehaviour, IShootable
{
    [SerializeField] private float _explosionRadius = 5f;
    [SerializeField] GameObject _explosionObject;

    public void HandleShot()
    {
        // Do a SphereOverlap to find targets
        Collider[] colliders = Physics.OverlapSphere(transform.position, _explosionRadius);

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.CompareTag("Enemy"))
            {
                AI target = collider.gameObject.GetComponent<AI>();
                if (target != null)
                {
                    target.HandleShot();
                }
            }
        }

        Instantiate(_explosionObject, transform.position, Quaternion.identity);
        Destroy(gameObject);

        // AddExplosionForce?
    }
}
