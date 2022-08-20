using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public SwarmController swarm;
    Cinemachine.CinemachineVirtualCamera cam;
    public float baseCamDist = 20;
    public float camDistSwarmScale = 10f;

    private void Awake() {
        cam = GetComponent<Cinemachine.CinemachineVirtualCamera>();
        swarm.m_onSwarmSizeChanged.AddListener(ReplaceCamera);
    }

    public void ReplaceCamera(int zombieAmount)
    {
        Cinemachine.CinemachineComponentBase componentBase = cam.GetCinemachineComponent(Cinemachine.CinemachineCore.Stage.Body);
        if (componentBase is Cinemachine.CinemachineFramingTransposer)
        {
            (componentBase as Cinemachine.CinemachineFramingTransposer).m_CameraDistance = baseCamDist + (float)zombieAmount / camDistSwarmScale; // your value
        }
    }
}
