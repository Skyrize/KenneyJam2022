using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    [SerializeField] private SceneReference m_scene;

    public void Trigger()
    {
        GameManager.Instance.SceneManager.LoadScene(m_scene, null, 0.6f, 0.6f);
    }
}
