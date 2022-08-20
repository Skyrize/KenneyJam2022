using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmController : MonoBehaviour
{
    [SerializeField] private float m_speed = 3.0f;
    [SerializeField] private uint m_initialSize = 10;
    [SerializeField] private List<GameObject> m_zombiePrefabs;

    private GameObject m_zombiPool;
    private List<GameObject> m_zombies = new List<GameObject>();

    void Awake()
    {
        // Instantiate pool
        m_zombiPool = new GameObject();
        
        // Instantiate zombies
        float spawnRadius = 10.0f;
        m_zombies.Capacity = (int)m_initialSize;
        for (uint i = 0; i < m_initialSize; ++i)
        {
            int prefabIndex = Random.Range(0, m_zombiePrefabs.Count);
            GameObject zombi = Instantiate(m_zombiePrefabs[prefabIndex], m_zombiPool.transform);
            Vector2 spawnPosition2D = Random.insideUnitCircle * spawnRadius;
            zombi.transform.position = new Vector3(spawnPosition2D.x, 0.0f, spawnPosition2D.y);
            zombi.GetComponent<ZombiController>().Swarm = this;
            m_zombies.Add(zombi);
        }
    }

    void Update()
    {
        Vector3 displacement = new Vector3(
            Input.GetAxis("Horizontal"),
            0.0f,
            Input.GetAxis("Vertical"));

        transform.position = transform.position + displacement * Time.deltaTime * m_speed;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 1.0f);
    }
}
