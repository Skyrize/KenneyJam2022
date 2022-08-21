using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] float m_roundPerSec = 2;
    [SerializeField] float m_damagePerRound = 1;
    [SerializeField] public Transform m_spawnPoint;
    [SerializeField] ParticleSystem m_muzzleFX;
    [SerializeField] LayerMask m_layerMask;
    public float CooldownDuration => 1.0f / m_roundPerSec;

    public float cooldownTimer = 0;

    private void Update() {
        if (cooldownTimer != 0)
        {
            cooldownTimer -= Time.deltaTime;
        }

        if (cooldownTimer <= 0)
        {
            cooldownTimer = 0;
        }
    }

    public void Shoot(Vector3 direction)
    {
        if (cooldownTimer != 0)
            return;
        RaycastHit hit;
        m_muzzleFX.Emit(5);
        if (Physics.Raycast(m_spawnPoint.position, direction, out hit, 100, m_layerMask))
        {
            GameManager.Instance.SpawnManager.SpawnHit(hit.point);
            if (hit.collider.transform.parent.GetComponent<ZombiController>())
            {
                hit.collider.transform.parent.GetComponent<HealthComponent>().ReduceHealth(m_damagePerRound);
            }
        }
        cooldownTimer = CooldownDuration;
    }
}
