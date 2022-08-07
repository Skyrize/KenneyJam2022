using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DamageOnTouchComponent : MonoBehaviour
{
    [Header("Damage on touch")]
    [SerializeField] float m_damageOnTouch = 13;
    [SerializeField] float m_cooldown = 0.75f;
    [SerializeField] LayerMask m_hitBoxLayerMask;
    private bool m_isOnCooldown = false;

    [Header("Damage over time")]
    [SerializeField] private float m_damagePerTick = 1;
    [SerializeField] private float m_tickPerSecond = 5;

    [HideInInspector] public UnityEvent<GameObject, float> onDamage = new UnityEvent<GameObject, float>();

    Dictionary<HealthComponent, Coroutine> m_targetsOfOverTimeDamages = new Dictionary<HealthComponent, Coroutine>();

#if !UNITY_EDITOR
    WaitForSeconds m_tickTimer;
    WaitForSeconds m_cooldownTimer;

    private void Awake()
    {
        m_tickTimer = new WaitForSeconds(1.0f / m_tickPerSecond);
        m_cooldownTimer = new WaitForSeconds(m_cooldown);
    }
#endif

    void PushDamageOverTime(HealthComponent _target)
    {
        if (_target && !m_targetsOfOverTimeDamages.ContainsKey(_target))
        {
            m_targetsOfOverTimeDamages.Add(_target, StartCoroutine(DamageOverTime(_target)));
        }
    }

    void PopDamageOverTime(HealthComponent _target)
    {
        Coroutine coroutine;

        if (_target && m_targetsOfOverTimeDamages.Remove(_target, out coroutine))
        {
            StopCoroutine(coroutine);
        }
    }

    private void OnTriggerEnter(Collider other) {
        PushDamageOverTime(other.GetComponent<HealthComponent>());
    }

    private void OnTriggerExit(Collider other) {
        PopDamageOverTime(other.GetComponent<HealthComponent>());
    }

    private void OnCollisionEnter(Collision other)
    {
        ContactPoint contactPoint = other.GetContact(0);
        HealthComponent otherHealth = other.gameObject.GetComponent<HealthComponent>();
        
        if (otherHealth && m_hitBoxLayerMask.ContainsLayer(contactPoint.thisCollider.gameObject.layer))
        {
            if (!m_isOnCooldown)
                StartCoroutine(DamageOnTouch(otherHealth));
            PushDamageOverTime(otherHealth);
        }
    }

    private void OnCollisionExit(Collision other)
    {
        ContactPoint contactPoint = other.GetContact(0);
        if (m_hitBoxLayerMask.ContainsLayer(contactPoint.thisCollider.gameObject.layer))
            PopDamageOverTime(other.gameObject.GetComponent<HealthComponent>());
    }

    IEnumerator DamageOverTime(HealthComponent _target)
    {
        while (true)
        {
            Damage(_target, m_damagePerTick);
#if UNITY_EDITOR
            yield return new WaitForSeconds(1.0f / m_tickPerSecond);
#else
            yield return m_tickTimer;
#endif
        }
    }

    IEnumerator DamageOnTouch(HealthComponent _target)
    {
        Damage(_target, m_damageOnTouch);
        m_isOnCooldown = true;
#if UNITY_EDITOR
        yield return new WaitForSeconds(m_cooldown);
#else
        yield return m_cooldownTimer;
#endif
        m_isOnCooldown = false;
    }

    void Damage(HealthComponent _target, float _damageAmount)
    {
        _target.ReduceHealth(_damageAmount);
        onDamage.Invoke(_target.gameObject, _damageAmount);
    }
}
