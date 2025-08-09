using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HumanRobotInteractionManager))]
public class MyScriptEditor : Editor
{

    public override void OnInspectorGUI()
    {
        // Draw the default inspector (so you still see variables)
        DrawDefaultInspector();

        // Get the target script reference
        HumanRobotInteractionManager script = (HumanRobotInteractionManager)target;

        if (GUILayout.Button("Enable HRI Mode"))
        {
            script.EnableHRIMode();
        }
    }
}
