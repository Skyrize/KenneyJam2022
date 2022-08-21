using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private SwarmController m_swarm;
    [SerializeField] private GameObject m_gameOverScreen;

    void Start()
    {
        enabled = false;

        if (m_swarm)
        {
            if (m_swarm.Count <= 0)
                GameOver();
            else
                m_swarm.m_onSwarmSizeChanged.AddListener(OnSwarmSizeChanged);
        }
    }

    void OnSwarmSizeChanged(int _size)
    {
        if (_size <= 0)
            GameOver();
    }

    void GameOver()
    {
        GameManager.Instance.LostCount++;

        if (m_gameOverScreen)
        {
            enabled = true;
            m_gameOverScreen.SetActive(true);
        }
        else
        {
            GameManager.Instance.SceneManager.ReloadScene();
        }
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            GameManager.Instance.SceneManager.ReloadScene();
            enabled = false;
        }
    }
}
