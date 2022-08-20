using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class SurvivorAction
{
    public enum Type
    {
        FLEE,
        PATROL,
        WANDER,
        CHASE,
        IDLE,
        DETECT,
    }
    public abstract Type m_type
    {
        get;
    }
    
    protected SurvivorController m_controller;

    public void Initialize(SurvivorController controller)
    {
        this.m_controller = controller;
        OnInitialize();
    }

    public virtual void OnInitialize() {}
    //Returns true if done and should go to next action
    public abstract bool Process();
}

[System.Serializable]
public class FleeAction : SurvivorAction
{
    public override Type m_type => Type.FLEE;
    public Transform m_target;
    public override bool Process()
    {
        m_controller.m_desiredVelocity = (m_controller.transform.position - m_target.transform.position).normalized * m_controller.m_runSpeed;
        return false;
    }
}

[System.Serializable]
public class IdleAction : SurvivorAction
{
    public override Type m_type => Type.IDLE;
    public override bool Process()
    {
        return false;
    }
}

[System.Serializable]
public class WanderAction : SurvivorAction
{
    public override Type m_type => Type.FLEE;
    public override bool Process()
    {
        return false;
    }
}

[System.Serializable]
public abstract class Behavior
{
    public enum Type
    {
        COWARD,
        WARRIOR,
        TURRET
    }
    [SerializeField] protected SurvivorAction m_currentAction;

    protected SurvivorController m_controller;

    public void Initialize(SurvivorController controller)
    {
        this.m_controller = controller;
        OnInitialize();
        m_currentAction.Initialize(controller);
    }
    
    public abstract void OnInitialize();

    public abstract void Update();
}

[System.Serializable]
public class CowardBehavior : Behavior
{
    FleeAction m_fleeAction = new FleeAction();
    IdleAction m_idleAction = new IdleAction();
    WanderAction m_wanderAction = new WanderAction();

    public override void OnInitialize()
    {
        int rand = Random.Range(0, 2);
        if (rand == 0)
            m_currentAction = m_idleAction;
        else
            m_currentAction = m_wanderAction;
        m_fleeAction.Initialize(m_controller);
        m_idleAction.Initialize(m_controller);
        m_wanderAction.Initialize(m_controller);
    }

    public override void Update()
    {
        if (!m_fleeAction.m_target)
        {
            Transform detectedTarget = m_controller.DetectClosestTarget();
            if (detectedTarget)
            {
                m_currentAction = m_fleeAction;
                m_fleeAction.m_target = detectedTarget;
            }
        }
        if (m_currentAction.Process())
        {
            switch (m_currentAction.m_type)
            {
                case SurvivorAction.Type.WANDER:
                break;
                case SurvivorAction.Type.IDLE:
                break;
                default:
                break;
            }
        }
    }
}

public class SurvivorController : MonoBehaviour
{

    [SerializeField] private HealthComponent m_healthComponent;
    [SerializeField] private Rigidbody m_rigidBody;
    [SerializeField] private Behavior.Type m_behavior;
    [SerializeField] private float m_detectionRadius = 20f;
    [SerializeField] private LayerMask m_detectionMask;
    [SerializeField] private float m_angularSpeed = 2.0f;
    [SerializeField] private float m_minSpeed = 0.2f;
    [SerializeField] private Animator m_animator;
    public float m_runSpeed = 3;
    public float m_walkSpeed = 1;

    [HideInInspector] public Vector3 m_desiredVelocity = Vector3.zero;
    static Collider[] detectionTargets = new Collider[5];

    // Hit reaction
    private Vector3 m_hitVelocity;

    private void Start()
    {
        m_healthComponent.onDeathEvent.AddListener(OnDeath);
    }

    Behavior m_currentBehavior;
    bool canUpdateBehavior = true;

    public Transform DetectClosestTarget()
    {
        int nbHits = Physics.OverlapSphereNonAlloc(transform.position + transform.forward * (m_detectionRadius - 1f), m_detectionRadius, detectionTargets, m_detectionMask);
        Transform targetDetected = null;

        for (int i = 0; i != nbHits; i++)
        {
            if (i == 0 || Vector3.Distance(targetDetected.position, transform.position) > Vector3.Distance(detectionTargets[i].transform.position, transform.position))
            {
                targetDetected = detectionTargets[i].transform;
            }
        }
        return targetDetected;
    }

    private void Awake() {
        switch (m_behavior)
        {
            case Behavior.Type.COWARD:
                m_currentBehavior = new CowardBehavior();
                m_currentBehavior.Initialize(this);
            break;
            case Behavior.Type.WARRIOR:
            break;
            case Behavior.Type.TURRET:
            break;
        }
    }

    private void OnBecameVisible() {
        canUpdateBehavior = true;
    }

    private void OnBecameInvisible() {
        canUpdateBehavior = false;
    }

    private void Update() {
        if (canUpdateBehavior)
            m_currentBehavior.Update(); 
    }

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

    private void UpdateAnimation()
    {
        float speed = m_rigidBody.velocity.magnitude;
        bool isRunning = m_rigidBody.velocity.sqrMagnitude >= m_minSpeed * m_minSpeed;
        m_animator.SetBool("IsRunning", isRunning);
        m_animator.speed = Mathf.Lerp(0.3f, 1.0f, Mathf.Min(speed, m_runSpeed) / m_runSpeed);
    }

    private void ApplyVelocity()
    {

        if (m_hitVelocity != Vector3.zero)
        {
            m_rigidBody.velocity = m_hitVelocity;
            m_rigidBody.rotation = Quaternion.LookRotation(-m_hitVelocity.normalized);
            m_animator.SetBool("IsRunning", false);
        }
        else if (m_desiredVelocity != Vector3.zero)
        {
            m_rigidBody.velocity = m_desiredVelocity;
            Vector3 direction = Vector3.RotateTowards(transform.forward, m_desiredVelocity, Time.deltaTime * m_angularSpeed, 0.0f);
            direction.y = transform.position.y;
            m_rigidBody.rotation = Quaternion.LookRotation(direction);
            m_rigidBody.angularVelocity = Vector3.zero;
            UpdateAnimation();
        }
        else if (m_rigidBody.velocity != Vector3.zero)
        {
            m_rigidBody.velocity = Vector3.zero;
            UpdateAnimation();
        }
    }

    public void Hit(ZombiController _zombi)
    {
        Vector3 hitDirection = transform.position - _zombi.transform.position;
        hitDirection.y = 0.0f;
        m_hitVelocity = hitDirection * 10.0f;
        m_rigidBody.rotation = Quaternion.LookRotation(-hitDirection);
        m_runSpeed /= 1.5f;
        m_walkSpeed /= 1.5f;
    }

    private void OnDeath()
    {
        GameManager.Instance.AudioComponent.Play("Transform");
        GameManager.Instance.SpawnManager.SpawnPouf(transform.position + Vector3.up * 1.0f);
        ZombiController newZombie = GameManager.Instance.SpawnManager.SpawnZombie(transform.position, transform.rotation).GetComponent<ZombiController>();
        newZombie.Wait(0.6f);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward * (m_detectionRadius - 1f), m_detectionRadius);
    }
}
