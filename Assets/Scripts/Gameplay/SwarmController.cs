using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SwarmController : MonoBehaviour
{
    [SerializeField] private uint m_initialSize = 10;
    [SerializeField] private AnimationCurve m_speedCurve;
    [SerializeField] private float m_boostScale = 1.5f;
    [SerializeField] private float m_boostDuration = 3f;
    [SerializeField] private float m_boostRecoveryScale = 0.75f;
    [SerializeField] private UIFillBar m_boostBar;

    private GameObject m_zombiePool;
    private List<ZombiController> m_zombies = new List<ZombiController>();
    [HideInInspector] public UnityEvent<int> m_onSwarmSizeChanged = new UnityEvent<int>();

    float m_speed = 1;
    float m_boostSpeed = 1;
    bool m_isBoosting = false;
    float m_boostTimer = 0;
    public float Speed => m_isBoosting ? m_boostSpeed : m_speed;
    private void Awake()
    {
        // Instantiate pool
        m_zombiePool = new GameObject();
        m_zombiePool.name = "Zombie Pool";
        
        // Instantiate zombies
        float spawnRadius = 10.0f;
        m_zombies.Capacity = (int)m_initialSize;
        for (uint i = 0; i < m_initialSize; ++i)
        {
            Vector2 spawnPosition2D = Random.insideUnitCircle * Random.Range(0.5f, spawnRadius);
            Vector3 spawnPosition = transform.position + new Vector3(spawnPosition2D.x, 0.0f, spawnPosition2D.y);
            GameObject zombie = GameManager.Instance.SpawnManager.SpawnZombie(spawnPosition, Quaternion.identity);
            AddZombie(zombie.GetComponent<ZombiController>());
        }
        m_boostTimer = m_boostDuration;
        UpdateBoost();
    }

    private void Start() {
        m_onSwarmSizeChanged.Invoke(m_zombies.Count);
    }

    private void UpdateDisplacement()
    {
        Vector3 displacement = new Vector3(
            Input.GetAxis("Horizontal"),
            0.0f,
            Input.GetAxis("Vertical"));

        transform.position = transform.position + displacement * Time.deltaTime * Speed;
    }

    private void UpdateBoost()
    {
        m_isBoosting = false;
        if (Input.GetButton("Fire1") || Input.GetButton("Fire2") || Input.GetButton("Fire3") || Input.GetButton("Jump"))
        {
            if (m_boostTimer == 0)
                return;
            m_boostTimer = Mathf.Clamp(m_boostTimer - Time.deltaTime, 0, m_boostDuration);
            m_isBoosting = true;
        }
        else
        {
            if (m_boostTimer == m_boostDuration)
                return;
            m_boostTimer = Mathf.Clamp(m_boostTimer + Time.deltaTime * m_boostRecoveryScale, 0, m_boostDuration);
        }

        if (m_boostBar)
            m_boostBar.SetFill(m_boostTimer / m_boostDuration);
    }

    private void Update()
    {
        UpdateDisplacement();
        UpdateBoost();

        if (Input.GetKeyDown(KeyCode.P))
        {
            for (int i = 0; i != 5; i++)
            {
                Vector2 spawnPosition2D = Random.insideUnitCircle * 10f;
                Vector3 spawnPosition = transform.position + new Vector3(spawnPosition2D.x, 0.0f, spawnPosition2D.y);
                GameObject zombie = GameManager.Instance.SpawnManager.SpawnZombie(spawnPosition, Quaternion.identity);
                AddZombie(zombie.GetComponent<ZombiController>());
            }
        }
    }

    public void UpdateSpeed()
    {
        m_speed = m_speedCurve.Evaluate(m_zombies.Count);
        m_boostSpeed = m_speed * m_boostScale;
    }

    public void AddZombie(ZombiController _zombie)
    {
        _zombie.Swarm = this;
        _zombie.transform.parent = m_zombiePool.transform;
        m_zombies.Add(_zombie);
        UpdateSpeed();
        m_onSwarmSizeChanged.Invoke(m_zombies.Count);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 1.0f);
    }
}
