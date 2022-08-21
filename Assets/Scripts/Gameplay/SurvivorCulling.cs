using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivorCulling : MonoBehaviour
{
    [SerializeField] private SurvivorController m_survivor;

    private void OnBecameVisible()
    {
#if UNITY_EDITOR
        if (Camera.current && Camera.current.name == "SceneCamera") 
            return;
#endif

        m_survivor.canUpdateBehavior = true;
    }

    private void OnBecameInvisible()
    {
        m_survivor.canUpdateBehavior = false;   
    }
}
