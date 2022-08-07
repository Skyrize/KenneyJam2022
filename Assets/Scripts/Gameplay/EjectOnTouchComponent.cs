using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EjectOnTouchComponent : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float m_ejectionForce = 3;
    [SerializeField] private float m_ejectionTimer = .3f;
    [SerializeField]
    LayerMask m_hitBoxLayerMask;
    [SerializeField] bool m_isOnCooldown = true;

    public IEnumerator Eject(Rigidbody _targetRb, Vector3 _direction)
    {
        Vector3 ejection = _direction * m_ejectionForce;
        _targetRb.AddForce(ejection, ForceMode.Impulse);
        m_isOnCooldown = true;
        yield return new WaitForSeconds(m_ejectionTimer);
        m_isOnCooldown = false;
    }

    private void OnCollisionEnter(Collision _other) {
        if (m_isOnCooldown)
            return;
        ContactPoint contactPoint = _other.GetContact(0);
        if (m_hitBoxLayerMask.ContainsLayer(contactPoint.thisCollider.gameObject.layer))
        {
            Rigidbody rb = _other.gameObject.GetComponent<Rigidbody>();
            if (rb)
                StartCoroutine(Eject(rb, (rb.transform.position - contactPoint.point).normalized));
        }
    }
}
