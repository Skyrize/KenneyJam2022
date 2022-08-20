using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivorController : MonoBehaviour
{
    [SerializeField] private HealthComponent m_healthComponent;
    [SerializeField] private Rigidbody m_rigidBody;

    private Vector3 m_desiredVelocity = Vector3.zero;

    // Hit reaction
    private Vector3 m_hitVelocity;

    private void FixedUpdate()
    {
        UpdateHit();
        ApplyVelocity();
    }

    private void UpdateHit()
    {
        if (m_hitVelocity == Vector3.zero)
        {
            m_hitVelocity = Vector3.zero;
            return;
        }

        float hitSpeed = m_hitVelocity.magnitude;
        float newHitSpeed = hitSpeed - hitSpeed * 40.0f * Time.fixedDeltaTime; 
        if (newHitSpeed <= 0.0f)
        {
            m_hitVelocity = Vector3.zero;
        }
        else
        {
            m_hitVelocity = m_hitVelocity / hitSpeed * newHitSpeed;
        }
    }

    private void ApplyVelocity()
    {
        m_rigidBody.velocity = m_desiredVelocity + m_hitVelocity;

        if (m_hitVelocity == Vector3.zero)
        {
            m_rigidBody.angularVelocity = Vector3.zero;
        }
        else
        {
            m_rigidBody.rotation = Quaternion.LookRotation(-m_hitVelocity.normalized);
        }
    }

    public void Hit(ZombiController _zombi, float _damagePoints)
    {
        m_healthComponent.ReduceHealth(_damagePoints);

        if (m_healthComponent.isDead)
        {
            GameManager.Instance.AudioComponent.Play("Transform");
            GameManager.Instance.SpawnManager.SpawnPouf(transform.position + Vector3.up * 1.0f);
            GameObject newZombi = GameManager.Instance.SpawnManager.SpawnZombie(transform.position, transform.rotation);
            _zombi.Swarm.AddZombie(newZombi.GetComponent<ZombiController>());
            Destroy(gameObject);
        }
        else
        {
            Vector3 hitDirection = transform.position - _zombi.transform.position;
            hitDirection.y = 0.0f;
            m_hitVelocity = hitDirection * 10.0f;
            m_rigidBody.rotation = Quaternion.LookRotation(-hitDirection);
        }
    }
}
