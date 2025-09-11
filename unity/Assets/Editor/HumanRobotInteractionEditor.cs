using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HumanRobotInteractionManager))]
public class HumanRobotInteractionEditor : Editor {

    public override void OnInspectorGUI() {
        // Draw the default inspector (so you still see variables)
        DrawDefaultInspector();

        // Get the target script reference
        HumanRobotInteractionManager script = (HumanRobotInteractionManager)target;

#if UNITY_WEBGL

        if (GUILayout.Button("Enable HRI Mode")) {
            script.EnableHRIMode();
        }

        if (GUILayout.Button("Robot Rotate Left")) {
            Debug.Log("Running Robot Rotate Left to test why robot status is not changed to ActionComplete...");
            var jsInterface = GameObject.Find("FPSController").GetComponent<JavaScriptInterface>();
            string json = @"{
                ""action"": ""RotateLeft"",
                ""degrees"": 30,
                ""agentId"": 1,
                ""renderImage"": true,
                ""onlyEmitOnAction"": true
            }";
            jsInterface.Step(json);
        }

#endif
    }
}
