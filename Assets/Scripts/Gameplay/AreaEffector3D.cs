using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaEffector3D : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private Vector3 m_force = Vector3.up;
    [SerializeField]
    private ForceMode m_mode = ForceMode.Acceleration;

    private void OnTriggerStay(Collider _other) {
        if (_other.GetComponent<Rigidbody>()) {
            _other.GetComponent<Rigidbody>().AddForce(m_force, m_mode);
        }
    }
}
