using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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
        }
        if(GUILayout.Button("Clean"))
        {
            houseGen.Clean();
        }
    }
}