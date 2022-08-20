using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerOnAnyButtonPressed : MonoBehaviour
{
    public UnityEvent m_onPressed;

    void Update()
    {
        if (Input.anyKeyDown)
            m_onPressed?.Invoke();
    }
}
