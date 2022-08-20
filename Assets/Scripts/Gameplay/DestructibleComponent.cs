using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleComponent : MonoBehaviour
{
    [SerializeField] private float m_height = 20.0f;

    private HealthComponent m_healthComponent;
    private float m_initialPositionX;
    private Timer m_shakeTimer;
    private float m_shakeDuration;
    private float m_shakeAmplitude = 0.0f;
    private float m_shakeAmplitudeMax = 0.5f;

    private void Awake()
    {
        m_healthComponent = GetComponent<HealthComponent>();
        m_healthComponent.onDamageEvent.AddListener(OnDamage);
        m_healthComponent.onDeathEvent.AddListener(OnDeath);

        m_initialPositionX = transform.position.x;
    }

    private void Update()
    {
        bool mustShake = m_healthComponent.isDead || m_shakeTimer.ElapsedTime <= m_shakeDuration;
        if (!mustShake)
        {
            m_shakeTimer.Stop();
            m_shakeAmplitude = 0.0f;
            enabled = false;
            return;
        }

        Vector3 position = transform.position;
        position.x = m_initialPositionX + Mathf.Sin(m_shakeTimer.ElapsedTime * 20) * m_shakeAmplitude;

        if (m_healthComponent.isDead)
        {
            position.y -= 4.0f * Time.deltaTime;

            if (m_height < 0.0f)
                Destroy(gameObject);
        }

        transform.position = position;
    }

    private void OnDamage(float _damagePoints)
    {
        enabled = true;
        if (!m_shakeTimer.IsStarted)
            m_shakeTimer.Restart();
        m_shakeDuration = m_shakeTimer.ElapsedTime + 0.1f;
        m_shakeAmplitude = Mathf.Min(m_shakeAmplitude + 0.01f, m_shakeAmplitudeMax);
    }

    private void OnDeath()
    {
        m_shakeAmplitude = m_shakeAmplitudeMax;
        enabled = true;
    }
}
