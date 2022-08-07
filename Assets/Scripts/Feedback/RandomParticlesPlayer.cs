using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomParticlesPlayer : MonoBehaviour
{
    [SerializeField]
    bool m_canMultiplay = false;
    [SerializeField]
    float m_cooldown = 1f;

    ParticleSystem[] m_particles;
    bool m_isPlaying = false;
    WaitForSeconds m_cooldownTimer;

    private void Awake() {
        m_particles = transform.GetComponentsInDirectChildren<ParticleSystem>();
        m_cooldownTimer = new WaitForSeconds(m_cooldown);
    }

    IEnumerator Play(ParticleSystem _particleSystem, Vector3 _position)
    {
        if (!m_canMultiplay && m_isPlaying)
            yield break;
        _particleSystem.transform.position = _position;
        _particleSystem.Play();
        if (!m_canMultiplay)
            m_isPlaying = true;
        yield return m_cooldownTimer;
        m_isPlaying = false;
    }

    public void PlayRandom()
    {
        if (m_particles.Length == 0) {
            Debug.LogError($"Missing Particle systems as child of RandomParticlesPlayer {transform.name}");
            return;
        }
        ParticleSystem particleSystem = m_particles[Random.Range(0, m_particles.Length)];
        StartCoroutine(Play(particleSystem, particleSystem.transform.position));
    }

    public void PlayRandomAt(Vector3 position)
    {
        if (m_particles.Length == 0) {
            Debug.LogError($"Missing Particle systems as child of RandomParticlesPlayer {transform.name}");
            return;
        }
        var particleSystem = m_particles[Random.Range(0, m_particles.Length)];
        StartCoroutine(Play(particleSystem, position));
    }

    public void PlayRandomAt(Transform target)
    {
        PlayRandomAt(target.position);
    }
}
