using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShaker
{
    public enum Intensity { Light, Medium, Heavy };

    public Vector3 Output { get; private set; } = Vector3.zero;

    public void Update()
    {
        if (m_shakes.Count == 0)
            return;

        Output = Vector3.zero;

        m_shakes.RemoveAll
        (
            (ShakeInstance _shake) => 
            {
                if (_shake.Update())
                {
                    Output += _shake.Output;
                    return false;
                }
                return true;
            } 
        );

        foreach (ShakeInstance shake in m_shakes)
        {
            shake.Update();
            Output += shake.Output;
        }
    }

    public void Shake(Intensity _intensity)
    {
        m_shakes.Add(new ShakeInstance(m_presets[_intensity]));
    }

    private List<ShakeInstance> m_shakes = new List<ShakeInstance>();

    private struct ShakeData
    {
        public float duration;
        public float speed;
        public ADSR envelop;

        public ShakeData(float _duration, float _speed, ADSR _envelop)
        {
            duration = _duration;
            speed = _speed;
            envelop = _envelop;
        }
    }

    private static Dictionary<Intensity, ShakeData> m_presets = new Dictionary<Intensity, ShakeData>()
    {
        { Intensity.Light , new ShakeData(0.2f, 2.0f,  new ADSR(0.1f, 0.0f, 0.1f, 0.1f, 0.1f)) },
        { Intensity.Medium, new ShakeData(0.4f, 4.0f,  new ADSR(0.2f, 0.1f, 0.4f, 0.5f, 0.3f)) },
        { Intensity.Heavy , new ShakeData(0.6f, 10.0f, new ADSR(0.4f, 0.2f, 0.6f, 1.0f, 0.8f)) }
    };

    private class ShakeInstance
    {
        public Vector3 Output { get; private set; } = Vector3.zero;

        private readonly ShakeData m_data;
        private readonly float m_seed;

        private float m_elapsedTime = 0;

        public ShakeInstance(ShakeData _data)
        {
            m_data = _data;
            m_seed = Time.time;
        }

        public bool Update()
        {
            m_elapsedTime += Time.deltaTime;

            if (m_elapsedTime < m_data.envelop.attackDuration)
            {
                UpdateAttack();
            }
            else if (m_elapsedTime < m_data.envelop.attackDuration + m_data.envelop.decayDuration)
            {
                UpdateDecay();
            }
            else if (m_elapsedTime < m_data.envelop.attackDuration + m_data.envelop.decayDuration + m_data.duration)
            {
                UpdateSustain();
            }
            else if (m_elapsedTime < m_data.envelop.attackDuration + m_data.envelop.decayDuration + m_data.duration + m_data.envelop.releaseDuration)
            {
                UpdateRelease();
            }
            else
            {
                return false;
            }

            return true;
        }

        private void UpdateAttack()
        {
            float amplitude = Mathf.Lerp(0, m_data.envelop.attackValue, m_elapsedTime / m_data.envelop.attackDuration);
            ComputeOutput(amplitude);
        }

        private void UpdateDecay()
        {
            float elapsed = m_elapsedTime - m_data.envelop.attackDuration;
            float amplitude = Mathf.Lerp(m_data.envelop.attackValue, m_data.envelop.sustainValue, elapsed / m_data.envelop.decayDuration);
            ComputeOutput(amplitude);
        }

        private void UpdateSustain()
        {
            ComputeOutput(m_data.envelop.sustainValue);
        }

        private void UpdateRelease()
        {
            float elapsed = m_elapsedTime - m_data.envelop.attackDuration - m_data.envelop.decayDuration - m_data.duration;
            float amplitude = Mathf.Lerp(m_data.envelop.sustainValue, 0, elapsed / m_data.envelop.releaseDuration);
            ComputeOutput(amplitude);
        }

        private void ComputeOutput(float _amplitude)
        {
            float halfAmplitude = _amplitude * 0.5f;

            Output = new Vector3(
                Mathf.PerlinNoise(m_seed + m_elapsedTime * m_data.speed, 0) * _amplitude - halfAmplitude,
                Mathf.PerlinNoise(0, m_seed + m_elapsedTime * m_data.speed) * _amplitude - halfAmplitude,
                0
            );
        }
    }
    
}
