using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleComponent : MonoBehaviour
{
    [SerializeField] private float m_height = 20.0f;
    [SerializeField] private float m_survivorSpawnRadius = 5.0f;
    public uint m_nbSurvivorsInside = 0;

    private HealthComponent m_healthComponent;
    private Vector3 m_initialPosition;
    private Timer m_shakeTimer;
    private float m_shakeDuration;
    private float m_shakeAmplitude = 0.0f;
    private float m_shakeAmplitudeMax = 0.5f;
    private List<GameObject> m_spawnedSurvivors;

    private void Awake()
    {
        m_healthComponent = GetComponent<HealthComponent>();
        m_healthComponent.onDamageEvent.AddListener(OnDamage);
        m_healthComponent.onDeathEvent.AddListener(OnDeath);

        m_initialPosition = transform.position;
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
        position.x = m_initialPosition.x + Mathf.Sin(m_shakeTimer.ElapsedTime * 20) * m_shakeAmplitude;
        position.z = m_initialPosition.z + Mathf.Sin(m_shakeTimer.ElapsedTime + 100 * 16) * m_shakeAmplitude;

        if (m_healthComponent.isDead)
        {
            position.y -= 4.0f * Time.deltaTime;

            if (transform.position.y + m_height < 0.0f)
            {
                foreach (GameObject survivor in m_spawnedSurvivors)
                {
                    survivor.GetComponent<Rigidbody>().detectCollisions = true;
                    survivor.GetComponent<SurvivorController>().enabled = true;
                }
                Destroy(gameObject);
            }
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
        enabled = true;
        m_shakeAmplitude = m_shakeAmplitudeMax;
        m_spawnedSurvivors = new List<GameObject>((int)m_nbSurvivorsInside);

        for (uint i = 0; i < m_nbSurvivorsInside; ++i)
        {
            Vector2 spawnPosition2D = Random.insideUnitCircle * Random.Range(0.5f, m_survivorSpawnRadius);
            Vector3 spawnPosition = transform.position + new Vector3(spawnPosition2D.x, 0.0f, spawnPosition2D.y);
            Vector2 spawnDirection2D = Random.insideUnitCircle.normalized;
            Quaternion spawnQuat = Quaternion.LookRotation(new Vector3(spawnDirection2D.x, 0.0f, spawnDirection2D.y));

            GameObject survivor = GameManager.Instance.SpawnManager.SpawnSurvivor(spawnPosition, spawnQuat);
            survivor.GetComponent<Rigidbody>().detectCollisions = false;
            survivor.GetComponent<SurvivorController>().enabled = false;
            m_spawnedSurvivors.Add(survivor);
        }
    }
}
