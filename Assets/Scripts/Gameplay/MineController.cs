using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineController : MonoBehaviour
{
    [SerializeField] private float m_explosionRadius = 3.0f;
    [SerializeField] private ParticleSystem m_particles;

    private void OnTriggerEnter(Collider _other)
    {
        if (_other.GetComponentInParent<ZombiController>() != null)
        {
            Explode();
        }
    }

    static private Collider[] s_overlapColliders = new Collider[100];

    private void Explode()
    {
        m_particles.Play();
        m_particles.transform.parent = null;

        GetComponent<Rigidbody>().detectCollisions = false;

        int nbColliders = Physics.OverlapSphereNonAlloc(transform.position, m_explosionRadius, s_overlapColliders);
        for (int i = 0; i < nbColliders; ++i)
        {
            Collider collider = s_overlapColliders[i];
            ZombiController zombi = collider.GetComponentInParent<ZombiController>();
            if (zombi)
            {
                zombi.GetComponent<HealthComponent>().ReduceHealth(100.0f);
            }
        }

        Destroy(gameObject);
    }
}
