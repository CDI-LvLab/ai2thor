using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RenderObjathorAssets))]
public class ObjathorAssetRenderer : Editor {

    public override void OnInspectorGUI() {
        // Draw the default inspector (so you still see variables)
        DrawDefaultInspector();

        // Get the target script reference
        RenderObjathorAssets script = (RenderObjathorAssets)target;

#if UNITY_EDITOR

        if (GUILayout.Button("Render Assets")) {
            script.StartRendering();
        }

#endif
    }
}
