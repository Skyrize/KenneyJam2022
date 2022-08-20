using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombiController : MonoBehaviour
{
    [SerializeField] private float m_speed = 2.0f;
    [SerializeField] private float m_angularSpeed = 2.0f;
    [SerializeField] private Rigidbody m_rigidBody;
    [SerializeField] private Animator m_animator;
    [SerializeField] private AnimationCurve m_avoidanceCurve;

    public SwarmController Swarm { get; set; }

    private List<ZombiController> m_nearZombies = new List<ZombiController>(10);
    private Vector3 m_desiredVelocity;

    private void Update()
    {
        if (Swarm)
        {
            // Move
            Vector3 heading = (Swarm.transform.position - transform.position).normalized;
            Vector3 avoidance = ComputeAvoidance();
            float avoidanceRatio = avoidance.magnitude;
            m_desiredVelocity = (heading * (1.0f - avoidanceRatio) + avoidance * avoidanceRatio) * m_speed;
            //m_rigidBody.velocity = m_desiredVelocity;
            transform.position = transform.position + m_desiredVelocity * Time.deltaTime;

            // Rotation
            Vector3 lookDirection = Vector3.RotateTowards(transform.forward, m_desiredVelocity, Time.deltaTime * m_angularSpeed, 0.0f);
            m_rigidBody.rotation = Quaternion.LookRotation(lookDirection);

            UpdateAnimation();
        }
    }

    private void UpdateAnimation()
    {
        float speed = m_desiredVelocity.magnitude;
        bool isRunning = speed > 0.1f;
        m_animator.SetBool("IsRunning", isRunning);
        m_animator.speed = isRunning ? Mathf.Lerp(0.0f, 1.0f, Mathf.Min(speed, m_speed) / m_speed) : 1.0f;
    }

    private void OnTriggerEnter(Collider _other)
    {
        ZombiController zombi = _other.GetComponentInParent<ZombiController>();
        if (zombi)
        {
            m_nearZombies.Add(zombi);
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

    private Vector3 ComputeAvoidance()
    {
        Vector3 avoidance = Vector3.zero;

        foreach (ZombiController zombi in m_nearZombies)
        {
            Vector3 deltaPos = transform.position - zombi.transform.position;
            deltaPos.y = 0.0f;
            avoidance += deltaPos.normalized * m_avoidanceCurve.Evaluate(1 / deltaPos.magnitude);
        }

        if (avoidance.sqrMagnitude > 1.0f)
            avoidance.Normalize();
        
        return avoidance;
    }
}
