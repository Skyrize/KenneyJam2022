using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public bool m_clockwise = true;
    public float m_turnsBySecond = 1.0f;
    public float m_initAngle = 0.0f;

    private void Start()
    {
        transform.rotation = Quaternion.Euler(Vector3.back * m_initAngle);
    }

    private void Update()
    {
        transform.Rotate(Vector3.back * (m_clockwise ? 1.0f : -1.0f) * m_turnsBySecond * 360.0f * Time.deltaTime);
    }
}
