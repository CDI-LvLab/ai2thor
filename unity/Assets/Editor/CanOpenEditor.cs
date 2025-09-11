using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CanOpen_Object))]
public class CanOpenEditor : Editor {

    public override void OnInspectorGUI() {
        // Draw the default inspector (so you still see variables)
        DrawDefaultInspector();

        // Get the target script reference
        CanOpen_Object script = (CanOpen_Object)target;

        if (script.openPositions != null && script.openPositions.Length > 0) {
            if (GUILayout.Button("Open")) {
                var movementType = script.GetMovementType();
                if (movementType == CanOpen_Object.MovementType.Slide) {
                    for (int i = 0; i < script.MovingParts.Length; i++) {
                        script.MovingParts[i].transform.position = script.openPositions[i];
                    }
                } else if (movementType == CanOpen_Object.MovementType.Rotate) {
                    for (int i = 0; i < script.MovingParts.Length; i++) {
                        script.MovingParts[i].transform.localEulerAngles = script.openPositions[i];
                    }
                } else if (movementType == CanOpen_Object.MovementType.Slide) {
                    for (int i = 0; i < script.MovingParts.Length; i++) {
                        script.MovingParts[i].transform.localScale = script.openPositions[i];
                    }
                }
                script.isOpen = true;
                script.currentOpenness = 1f;
            }
            if (GUILayout.Button("Set current transform as open")) {
                var movementType = script.GetMovementType();
                if (movementType == CanOpen_Object.MovementType.Slide) {
                    for (int i = 0; i < script.MovingParts.Length; i++) {
                        script.openPositions[i] = script.MovingParts[i].transform.position;
                    }
                } else if (movementType == CanOpen_Object.MovementType.Rotate) {
                    for (int i = 0; i < script.MovingParts.Length; i++) {
                        script.openPositions[i] = script.MovingParts[i].transform.localEulerAngles;
                    }
                } else if (movementType == CanOpen_Object.MovementType.Slide) {
                    for (int i = 0; i < script.MovingParts.Length; i++) {
                        script.openPositions[i] = script.MovingParts[i].transform.localScale;
                    }
                }
            }
        }
        if (script.closedPositions != null && script.closedPositions.Length > 0) {
            if (GUILayout.Button("Close")) {
                var movementType = script.GetMovementType();
                if (movementType == CanOpen_Object.MovementType.Slide) {
                    for (int i = 0; i < script.MovingParts.Length; i++) {
                        script.MovingParts[i].transform.position = script.closedPositions[i];
                    }
                } else if (movementType == CanOpen_Object.MovementType.Rotate) {
                    for (int i = 0; i < script.MovingParts.Length; i++) {
                        script.MovingParts[i].transform.localEulerAngles = script.closedPositions[i];
                    }
                } else if (movementType == CanOpen_Object.MovementType.Slide) {
                    for (int i = 0; i < script.MovingParts.Length; i++) {
                        script.MovingParts[i].transform.localScale = script.closedPositions[i];
                    }
                }
                script.isOpen = false;
                script.currentOpenness = 0f;
            }
            if (GUILayout.Button("Set current transform as closed")) {
                var movementType = script.GetMovementType();
                if (movementType == CanOpen_Object.MovementType.Slide) {
                    for (int i = 0; i < script.MovingParts.Length; i++) {
                        script.closedPositions[i] = script.MovingParts[i].transform.position;
                    }
                } else if (movementType == CanOpen_Object.MovementType.Rotate) {
                    for (int i = 0; i < script.MovingParts.Length; i++) {
                        script.closedPositions[i] = script.MovingParts[i].transform.localEulerAngles;
                    }
                } else if (movementType == CanOpen_Object.MovementType.Slide) {
                    for (int i = 0; i < script.MovingParts.Length; i++) {
                        script.closedPositions[i] = script.MovingParts[i].transform.localScale;
                    }
                }
            }
        }
    }
}
