using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthComponent : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField]
    private float m_actualHealth = 100;
    [SerializeField]
    private float m_maxHealth = 100;

    [Header("Events")]
    [HideInInspector] public UnityEvent<float> onHealEvent = new UnityEvent<float>();
    [HideInInspector] public UnityEvent<float> onDamageEvent = new UnityEvent<float>();
    [HideInInspector] public UnityEvent onDeathEvent = new UnityEvent();
    [HideInInspector] public UnityEvent<float> onHealthRatioChanged = new UnityEvent<float>();

    private float actualHealth {
        get {
            return m_actualHealth;
        }
        set {
            m_actualHealth = Mathf.Clamp(value, 0, m_maxHealth);
            onHealthRatioChanged.Invoke(healthRatio);
        }
    }

    public float healthRatio => m_actualHealth / m_maxHealth;
    public float health => m_actualHealth;
    public float maxHealth => m_maxHealth;
    public bool isAlive => actualHealth > 0;
    public bool isDead => !isAlive;
    public bool isFullHealth => actualHealth == m_maxHealth;

    public void SetHealth(float health)
    {
        actualHealth = health;
    }

    public void ReduceHealth(float amount)
    {
        if (isDead)
            return;
        actualHealth -= amount;
        onDamageEvent.Invoke(amount);
        if (isDead)
            onDeathEvent.Invoke();
    }
    
    public void IncreaseHealth(float amount)
    {
        if (isDead)
            return;
        actualHealth += amount;
        onHealEvent.Invoke(amount);
    }

    public void SetMaxHealth(float newMax)
    {
        this.m_maxHealth = newMax;
        this.actualHealth = newMax;
    }

    public void RegenFullHealth()
    {
        actualHealth = m_maxHealth;
    }

}
