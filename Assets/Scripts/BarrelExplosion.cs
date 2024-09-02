using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelExplosion : MonoBehaviour, IShootable
{
    [SerializeField] private float _explosionRadius = 5f;
    [SerializeField] GameObject _explosionObject;

    public void HandleShot()
    {
        if (gameObject.TryGetComponent<Collider>(out Collider barrel_collider))
        {
            barrel_collider.enabled = false;
        }

        // Do a SphereOverlap to find targets
        Collider[] colliders = Physics.OverlapSphere(transform.position, _explosionRadius);

        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent<IShootable>(out IShootable target))
            {
                target.HandleShot();
            }
        }

        GameObject explosion = Instantiate(_explosionObject, transform.position, Quaternion.identity);
        Destroy(explosion, 2f);

        // Add explosion force?

        Destroy(gameObject);
    }
}
