using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

[CustomEditor(typeof(Generator), true)]
public class GeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        Generator houseGen = (Generator)target;
        if(GUILayout.Button("Generate"))
        {
            houseGen.Generate();
            EditorSceneManager.MarkSceneDirty(UnitySceneManager.GetActiveScene());
        }
        if(GUILayout.Button("Clean"))
        {
            houseGen.Clean();
            EditorSceneManager.MarkSceneDirty(UnitySceneManager.GetActiveScene());
        }
    }
}