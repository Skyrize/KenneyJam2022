using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class SceneReference : ISerializationCallbackReceiver
{
#if UNITY_EDITOR
    // For editor
    [SerializeField] private SceneAsset m_sceneAsset = null;
#endif

    // For runtime
    [SerializeField, HideInInspector] private string m_scenePath = string.Empty;
    
    public string ScenePath
    {
        get
        {
#if UNITY_EDITOR
            return GetScenePathFromAsset();
#else
            return m_scenePath;
#endif
        }
        set
        {
            m_scenePath = value;

#if UNITY_EDITOR
            m_sceneAsset = GetSceneAssetFromPath();
#endif
        }
    }

    public static implicit operator string(SceneReference _sceneReference)
    {
        return _sceneReference.ScenePath;
    }

    public void OnBeforeSerialize()
    {
#if UNITY_EDITOR

        // Asset is invalid but have Path to try and recover from
        if (m_sceneAsset == null && !string.IsNullOrEmpty(m_scenePath))
        {
            m_sceneAsset = GetSceneAssetFromPath();
            if (m_sceneAsset == null)
                m_scenePath = string.Empty;

            UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
        }

        // Asset takes precendence and overwrites Path
        else
        {
            m_scenePath = GetScenePathFromAsset();
        }
#endif
    }

    public void OnAfterDeserialize()
    {
#if UNITY_EDITOR
        // We sadly cannot touch assetdatabase during serialization, so defer by a bit.
        EditorApplication.update += HandleAfterDeserialize;
#endif
    }

#if UNITY_EDITOR

    private SceneAsset GetSceneAssetFromPath()
    {
        if (string.IsNullOrEmpty(m_scenePath))
            return null;

        return AssetDatabase.LoadAssetAtPath<SceneAsset>(m_scenePath);
    }

    private string GetScenePathFromAsset()
    {
        if (m_sceneAsset == null)
            return string.Empty;

        return AssetDatabase.GetAssetPath(m_sceneAsset);
    }

    private void HandleAfterDeserialize()
    {
        EditorApplication.update -= HandleAfterDeserialize;

        // Asset is valid, don't do anything - Path will always be set based on it when it matters
        if (m_sceneAsset != null)
            return;

        // Asset is invalid but have path to try and recover from
        if (!string.IsNullOrEmpty(m_scenePath))
        {
            m_sceneAsset = GetSceneAssetFromPath();
            
            // No asset found, path was invalid. Make sure we don't carry over the old invalid path
            if (m_sceneAsset == null)
                m_scenePath = string.Empty;

            if (!Application.isPlaying)
                UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
        }
    }
#endif
}
