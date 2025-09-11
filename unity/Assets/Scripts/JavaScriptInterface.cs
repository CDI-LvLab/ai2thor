﻿using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class JavaScriptInterface : MonoBehaviour {
    // IL2CPP throws exceptions about SendMetadata and Init not existing
    // so the body is only used for WebGL
#if UNITY_WEBGL
    private AgentManager agentManager;

    // private DebugInputField inputField; // inputField.setControlMode no longer used in SetController

    [DllImport("__Internal")]
    private static extern void Init();

    [DllImport("__Internal")]
    private static extern void SendMetadata(string str);

    [DllImport("__Internal")]
    private static extern void SendImage(string key, byte[] pointer, int length);

    /*
        metadata: serialized metadata, commonly an instance of MultiAgentMetadata
     */
    public void SendActionMetadata(string metadata) {
#if !UNITY_EDITOR && UNITY_WEBGL
        SendMetadata(metadata);
#endif
    }

    /*
        image: a series of bytes.
    */
    public void SendActionImage(string key, byte[] image) {
#if !UNITY_EDITOR && UNITY_WEBGL
        SendImage(key, image, image.Length);
#endif
    }

    void Start() {
        this.agentManager = GameObject
            .Find("PhysicsSceneManager")
            .GetComponentInChildren<AgentManager>();
        this.agentManager.SetUpPhysicsController();

        // inputField = GameObject.Find("DebugCanvasPhysics").GetComponentInChildren<DebugInputField>();// FindObjectOfType<DebugInputField>();
        // GameObject.Find("DebugCanvas").GetComponentInChildren<AgentManager>();
#if !UNITY_EDITOR && UNITY_WEBGL
        Init();
#endif

        Debug.Log("Calling store data");
    }

    public void GetRenderPath() {
        SendMetadata("" + GetComponentInChildren<Camera>().actualRenderingPath);
    }

    public void SetController(string controlModeEnumString) {
        ControlMode controlMode = (ControlMode)
            Enum.Parse(typeof(ControlMode), controlModeEnumString, true);
        // inputField.setControlMode(controlMode);

        Type componentType;
        var success = PlayerControllers.controlModeToComponent.TryGetValue(
            controlMode,
            out componentType
        );
        var Agent = CurrentActiveController().gameObject;
        if (success) {
            var previousComponent = Agent.GetComponent(componentType) as MonoBehaviour;
            if (previousComponent == null) {
                previousComponent = Agent.AddComponent(componentType) as MonoBehaviour;
            }
            previousComponent.enabled = true;
        }
    }

    public void Step(string jsonAction) {
        this.agentManager.ProcessControlCommand(new DynamicServerAction(jsonAction));
    }

    private BaseFPSAgentController CurrentActiveController() {
        return this.agentManager.PrimaryAgent;
    }

#endif
}
