using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenEffectManager
{
    float m_remainingFreeze = 0;

    public void Freeze(float _duration)
    {
        m_remainingFreeze = Mathf.Max(m_remainingFreeze, _duration);
        Time.timeScale = 0.05f;
    }

    public void Update()
    {
        if (m_remainingFreeze > 0)
            UpdateFreeze();

        Debug.Log(Time.timeScale);
    }

    private void UpdateFreeze()
    {
        m_remainingFreeze -= Time.unscaledDeltaTime;
        if (m_remainingFreeze <= 0)
        {
            m_remainingFreeze = 0;
            Time.timeScale = 1;
        }
    }
}
