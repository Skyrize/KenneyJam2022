using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombiController : MonoBehaviour
{
    [SerializeField] private float m_baseSpeed = 2.0f;
    public float Speed => Swarm ? Swarm.Speed : m_baseSpeed;
    [SerializeField] private float m_minSpeed = 0.2f;
    [SerializeField] private float m_angularSpeed = 2.0f;
    [SerializeField] private Rigidbody m_rigidBody;
    [SerializeField] private Animator m_animator;
    [SerializeField] private HealthComponent m_healthComponent;
    [SerializeField] private SphereCollider m_avoidanceCollider;
    [SerializeField] private AnimationCurve m_avoidanceCurve;
    [SerializeField] private float m_hitDamagePoints = 1.0f;
    [SerializeField] private float m_hitCooldownDuration = 1.0f;
    [SerializeField] private AnimationCurve m_avoidanceRadiusSwarmSizeCurve;

    public SwarmController Swarm { get; set; }
    public List<ZombiController> NearZombies { get => m_nearZombies; set => m_nearZombies = value; }

    private List<ZombiController> m_nearZombies = new List<ZombiController>(10);
    private Vector3 m_desiredVelocity;
    private Timer m_waitTimer = new Timer();
    private float m_waitDuration = 0.0f;
    private Timer m_hitCooldownTimer = new Timer();

    public float AvoidanceRadiusMax => Swarm ? m_avoidanceRadiusSwarmSizeCurve.Evaluate(Swarm.Count) : 10; 

    public void Wait(float _duration)
    {
        m_waitDuration = _duration;
        m_waitTimer.Restart();
    }

    private void Start()
    {
        m_healthComponent.onDeathEvent.AddListener(OnDeath);
    }

    private void Update()
    {
        if (Swarm)
        {
            if (m_waitTimer.IsStarted)
            {
                if (m_waitTimer.ElapsedTime >= m_waitDuration)
                    m_waitTimer.Stop();

                m_desiredVelocity = Vector3.zero;
            }
            else
            {
                Vector3 heading = ComputeHeading();
                Vector3 avoidance = ComputeAvoidance();
                if (avoidance.sqrMagnitude <= 0.1f * 0.1f)
                    avoidance = Vector3.zero;
                UpdateVelocity(heading, avoidance);
            }

            ApplyVelocity();
        }
    }

    private void LateUpdate()
    {
        UpdateAnimation();
    }

    private Vector3 ComputeHeading()
    {
        Vector3 heading = Swarm.transform.position - transform.position;
        heading.y = 0.0f;
        return Vector3.ClampMagnitude(heading, 1.0f);
    }

    private Vector3 ComputeAvoidance()
    {
        Vector3 avoidance = Vector3.zero;

        foreach (ZombiController zombi in m_nearZombies)
        {
            Vector3 deltaPos = transform.position - zombi.transform.position;
            deltaPos.y = 0.0f;
            float deltaNorm = deltaPos.magnitude;
            float avoidanceFactor = m_avoidanceCurve.Evaluate(1 - (deltaNorm / zombi.AvoidanceRadiusMax));
            if (avoidanceFactor > Mathf.Epsilon)
                avoidance += deltaPos / deltaNorm * avoidanceFactor;
        }

        // Crappy failsafe
        if (float.IsNaN(avoidance.x))
            avoidance = Vector3.zero;

        return avoidance;
    }

    private void UpdateVelocity(Vector3 _heading, Vector3 _avoidance)
    {
        Vector3 currentVelocity = m_rigidBody.velocity;
        float headingRatio = _heading.magnitude / Speed;
        //m_desiredVelocity = Vector3.ClampMagnitude(currentVelocity.normalized * 0.0f + _heading * headingRatio + _avoidance.normalized * (1.0f - headingRatio), 1.0f) * m_speed;
        m_desiredVelocity = Vector3.ClampMagnitude(_heading + _avoidance, 1.0f) * Speed;
    }

    private void ApplyVelocity()
    {
        if (m_desiredVelocity.sqrMagnitude >= m_minSpeed * m_minSpeed)
        {
            //transform.position = transform.position + m_desiredVelocity * Time.deltaTime;
            //transform.position = Vector3.MoveTowards(transform.position, transform.position + m_desiredVelocity * Time.deltaTime, m_speed);
            m_rigidBody.velocity = m_desiredVelocity;

            Vector3 lookDirection = Vector3.RotateTowards(transform.forward, m_desiredVelocity.normalized, Time.deltaTime * m_angularSpeed, 0.0f);
            m_rigidBody.rotation = Quaternion.LookRotation(lookDirection);
        }
        else
        {
            m_rigidBody.velocity = Vector3.zero;
            m_rigidBody.angularVelocity = Vector3.zero;
        }
    }

    private void UpdateAnimation()
    {
        float speed = m_rigidBody.velocity.magnitude;
        bool isRunning = m_rigidBody.velocity.sqrMagnitude >= m_minSpeed * m_minSpeed;
        m_animator.SetBool("IsRunning", isRunning);
        m_animator.speed = isRunning ? Mathf.Lerp(0.3f, 1.0f, Mathf.Min(speed, Speed) / Speed) : 1.0f;
    }

    private void OnTriggerEnter(Collider _other)
    {
        ZombiController zombie = _other.GetComponentInParent<ZombiController>();
        if (zombie)
        {
            if (zombie.Swarm == null)
                Swarm.AddZombie(zombie);

            m_nearZombies.Add(zombie);
        }
    }

    private void OnTriggerExit(Collider _other)
    {
        ZombiController zombi = _other.GetComponentInParent<ZombiController>();
        if (zombi)
        {
            m_nearZombies.Remove(zombi);
        }
    }

    private void OnCollisionStay(Collision _collision)
    {
        // Hit survivor
        if (!m_hitCooldownTimer.IsStarted || m_hitCooldownTimer.ElapsedTime > m_hitCooldownDuration)
        {
            {
                SurvivorController survivor = _collision.gameObject.GetComponent<SurvivorController>();
                if (survivor)
                {
                    survivor.Hit(this);
                }
            }
            {
                HealthComponent healthComponent = _collision.gameObject.GetComponent<HealthComponent>();
                if (healthComponent)
                {
                    healthComponent.ReduceHealth(m_hitDamagePoints);
                    
                    Vector3 hitPosition = _collision.contacts[0].point;
                    hitPosition.y = 2.0f;
                    GameManager.Instance.SpawnManager.SpawnHit(hitPosition);
                    GameManager.Instance.AudioComponent.Play("Punch");

                    m_hitCooldownTimer.Restart();
                }
            }
        }
    }

    public void OnDeath()
    {
        Swarm.RemoveZombie(this);
        GameManager.Instance.SpawnManager.SpawnPouf(transform.position);
        Destroy(gameObject);
    }
}
