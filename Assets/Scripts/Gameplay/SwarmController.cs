using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SwarmController : MonoBehaviour
{
    [SerializeField] private float m_speed = 3.0f;
    [SerializeField] private uint m_initialSize = 10;

    private GameObject m_zombiePool;
    private List<ZombiController> m_zombies = new List<ZombiController>();
    [HideInInspector] public UnityEvent<int> m_onSwarmSizeChanged = new UnityEvent<int>();

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
            Vector2 spawnPosition2D = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPosition = transform.position + new Vector3(spawnPosition2D.x, 0.0f, spawnPosition2D.y);
            GameObject zombie = GameManager.Instance.SpawnManager.SpawnZombie(spawnPosition, Quaternion.identity);
            AddZombie(zombie.GetComponent<ZombiController>());
        }
    }

    private void Start() {
        m_onSwarmSizeChanged.Invoke(m_zombies.Count);
    }

    private void Update()
    {
        Vector3 displacement = new Vector3(
            Input.GetAxis("Horizontal"),
            0.0f,
            Input.GetAxis("Vertical"));

        transform.position = transform.position + displacement * Time.deltaTime * m_speed;
    }

    public void AddZombie(ZombiController _zombie)
    {
        _zombie.Swarm = this;
        _zombie.transform.parent = m_zombiePool.transform;
        m_zombies.Add(_zombie);
        m_onSwarmSizeChanged.Invoke(m_zombies.Count);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 1.0f);
    }
}
