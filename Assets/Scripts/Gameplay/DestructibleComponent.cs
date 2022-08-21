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
    private List<ParticleSystem> m_spawnedParticles;
    private bool m_destroyed = false;

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

        Vector3 shakedPosition = transform.position;
        shakedPosition.x = m_initialPosition.x + Mathf.Sin(m_shakeTimer.ElapsedTime * 20) * m_shakeAmplitude;
        shakedPosition.z = m_initialPosition.z + Mathf.Sin(m_shakeTimer.ElapsedTime + 100 * 16) * m_shakeAmplitude;

        if (m_healthComponent.isDead)
        {
            shakedPosition.y -= 4.0f * Time.deltaTime;

            if (transform.position.y + m_height < 0.0f)
            {
                foreach (GameObject survivor in m_spawnedSurvivors)
                {
                    survivor.GetComponent<Rigidbody>().detectCollisions = true;
                    survivor.GetComponent<SurvivorController>().enabled = true;
                }

                gameObject.GetComponent<Rigidbody>().detectCollisions = false;

                StopParticle();

                Vector3 position = transform.position;
                position.y = 0.4f;
                transform.position = position;

                Vector3 scale = transform.localScale;
                scale.y = -1;
                transform.localScale = scale;

                enabled = false;
                m_destroyed = true;
                return;
            }
        }
        
        transform.position = shakedPosition;
    }

    private void OnDamage(float _damagePoints)
    {
        if (m_destroyed)
            return;

        enabled = true;
        if (!m_shakeTimer.IsStarted)
        {
            m_shakeTimer.Restart();
            StartParticle();
        }
        m_shakeDuration = m_shakeTimer.ElapsedTime + 0.1f;
        m_shakeAmplitude = Mathf.Min(m_shakeAmplitude + 0.01f, m_shakeAmplitudeMax);
    }

    private void OnDeath()
    {
        if (m_destroyed)
            return;

        if (!m_shakeTimer.IsStarted)
        {
            StartParticle();
        }

        enabled = true;
        m_shakeAmplitude = m_shakeAmplitudeMax;

        Vector3 bodyCenter = GetComponent<Rigidbody>().centerOfMass;
        bodyCenter.y = 0.0f;
        m_spawnedSurvivors = new List<GameObject>((int)m_nbSurvivorsInside);
        for (uint i = 0; i < m_nbSurvivorsInside; ++i)
        {
            Vector2 spawnPosition2D = Random.insideUnitCircle * Random.Range(0.5f, m_survivorSpawnRadius);
            Vector3 spawnPosition = transform.position + bodyCenter + new Vector3(spawnPosition2D.x, 0.0f, spawnPosition2D.y);
            Vector2 spawnDirection2D = Random.insideUnitCircle.normalized;
            Quaternion spawnQuat = Quaternion.LookRotation(new Vector3(spawnDirection2D.x, 0.0f, spawnDirection2D.y));

            GameObject survivor = GameManager.Instance.SpawnManager.SpawnSurvivor(spawnPosition, spawnQuat);
            survivor.GetComponent<Rigidbody>().detectCollisions = false;
            survivor.GetComponent<SurvivorController>().enabled = false;
            survivor.transform.position = spawnPosition;
            m_spawnedSurvivors.Add(survivor);
        }
    }

    private void StartParticle()
    {
        if (m_spawnedParticles == null)
        {
            m_spawnedParticles = new List<ParticleSystem>();
            GameManager.Instance.SpawnManager.SpawnDestruction(gameObject, ref m_spawnedParticles);
        }

        foreach (ParticleSystem particleSystem in m_spawnedParticles)
            particleSystem.Play();
    }

    private void StopParticle()
    {
        foreach (ParticleSystem particleSystem in m_spawnedParticles)
            particleSystem.Stop();
    }
}
