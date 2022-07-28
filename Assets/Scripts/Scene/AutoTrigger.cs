using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AutoTrigger : MonoBehaviour
{
    [SerializeField] private UnityEvent m_awakeEvent;
    [SerializeField] private UnityEvent m_startEvent;

    private void Awake()
    {
        m_awakeEvent?.Invoke();
    }

    private void Start()
    {
        m_startEvent?.Invoke();
    }
}
