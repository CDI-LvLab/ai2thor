using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ObjectClickEvent : UnityEvent<SimObjPhysics, SimObjPhysics> { }

namespace UnityStandardAssets.Characters.FirstPerson {
    public class HRIAgentController : MonoBehaviour {
        [SerializeField]
        private float HandMoveMagnitude = 0.1f;
        public float ReachableDistance = 5.0f;
        [Header("Object Highlighting")]
        public SimObjPrimaryProperty[] HighlightablePrimaryPropertiesWhenHolding;
        public SimObjSecondaryProperty[] HighlightableSecondaryPropertiesWhenHolding;
        public SimObjPrimaryProperty[] HighlightablePrimaryPropertiesWhenNotHolding;
        public SimObjSecondaryProperty[] HighlightableSecondaryPropertiesWhenNotHolding;

        [Header("Events")]
        public ObjectClickEvent onObjectClick;
        public PhysicsRemoteFPSAgentController Controller = null;
        private bool handMode = false;
        private Camera m_Camera;

        void Start() {
            disableUIElements();

#if !UNITY_EDITOR && UNITY_WEBGL
            // disable WebGLInput.captureAllKeyboardInput so elements in web page can handle keyboard inputs
            WebGLInput.captureAllKeyboardInput = false;
#endif

            var Debug_Canvas = GameObject.Find("DebugCanvasPhysics");
            AgentManager agentManager = GameObject
                .Find("PhysicsSceneManager")
                .GetComponentInChildren<AgentManager>();

            GameObject clone = GameObject.Find("FPSController(Clone)");
            if (!clone) {
                agentManager.agents.Clear();
                agentManager.SetUpPhysicsController();
            }

            Controller = agentManager.PrimaryAgent as PhysicsRemoteFPSAgentController;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Debug_Canvas.GetComponent<Canvas>().enabled = true;

            m_Camera = Camera.main;
#if UNITY_EDITOR
            var result = Controller.SpawnAsset("0a3db3f1bec64abbb2b2c0950e0ba6df", "0a3db3f1bec64abbb2b2c0950e0ba6df");
            if (result.success) {
                Controller.PickupObject(
                    "0a3db3f1bec64abbb2b2c0950e0ba6df",
                    true
                );
            } else {
                Debug.LogError("Object spawning failed: " + result.errorMessage);
            }
#endif
        }

        void Update() {
            SimObjPhysics highlighted = null;
            HighlightManager.Instance.Clear();

            GameObject holding = Controller.WhatAmIHolding();

            // If we're holding an object, only allow interacting with Receptacles.
            highlighted = GetSymObjUnderMouse(
                holding ? HighlightablePrimaryPropertiesWhenHolding : HighlightablePrimaryPropertiesWhenNotHolding,
                holding ? HighlightableSecondaryPropertiesWhenHolding : HighlightableSecondaryPropertiesWhenNotHolding
            );
            if (highlighted) {
                onObjectClick.Invoke(highlighted, holding ? holding.GetComponent<SimObjPhysics>() : null);
            }
#if UNITY_EDITOR
            if (Controller.ReadyForCommand) {
                float WalkMagnitude = 0.25f;
                if (!handMode) {
                    if (Input.GetKeyDown(KeyCode.W)) {
                        executeAction("MoveAhead", "moveMagnitude", WalkMagnitude);
                    }

                    if (Input.GetKeyDown(KeyCode.S)) {
                        executeAction("MoveBack", "moveMagnitude", WalkMagnitude);
                    }

                    if (Input.GetKeyDown(KeyCode.A)) {
                        executeAction("MoveLeft", "moveMagnitude", WalkMagnitude);
                    }

                    if (Input.GetKeyDown(KeyCode.D)) {
                        executeAction("MoveRight", "moveMagnitude", WalkMagnitude);
                    }

                    if (Input.GetKeyDown(KeyCode.UpArrow)) {
                        executeAction("LookUp", "degrees", 30f);
                    }

                    if (Input.GetKeyDown(KeyCode.DownArrow)) {
                        executeAction("LookDown", "degrees", 30f);
                    }

                    if (Input.GetKeyDown(KeyCode.LeftArrow)) {//|| Input.GetKeyDown(KeyCode.J)) {
                        executeAction("RotateLeft", "degrees", 45f);
                    }

                    if (Input.GetKeyDown(KeyCode.RightArrow)) {//|| Input.GetKeyDown(KeyCode.L)) {
                        executeAction("RotateRight", "degrees", 45f);
                    }
                }
            }
#endif
        }

        private void executeAction(string actionName, string argName, float value) {
            Dictionary<string, object> action = new Dictionary<string, object>();
            action["action"] = actionName;
            action["renderImage"] = true;
            action["onlyEmitOnAction"] = true;
            action[argName] = value;
            Controller.ProcessControlCommand(action);
        }

        private void executeAction(string actionName) {
            Dictionary<string, object> action = new Dictionary<string, object>();
            action["action"] = actionName;
            action["renderImage"] = true;
            action["onlyEmitOnAction"] = true;
            Controller.ProcessControlCommand(action);
        }

        private SimObjPhysics GetSymObjUnderMouse(
            SimObjPrimaryProperty[] primaryProperties,
            SimObjSecondaryProperty[] secondaryProperties
        ) {
            // Raycast from mouse position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            int layerMask = LayerMask.GetMask("SimObjVisible", "Procedural1", "Procedural2", "Procedural3", "Procedural0", "PlaceableSurface");
            RaycastHit[] hits = Physics.RaycastAll(ray, ReachableDistance, layerMask, QueryTriggerInteraction.Collide);
            Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            foreach (var hit in hits) {
                // Check for tag "SimObjPhysics"
                if (hit.transform.tag != "SimObjPhysics") {
                    continue;
                }

                // Get SimObjPhysics component
                SimObjPhysics simObjPhysics = hit.transform.GetComponent<SimObjPhysics>();
                // Debug.LogWarning(simObjPhysics ? simObjPhysics.assetID : null);
                if (!simObjPhysics) {
                    continue;
                }

                foreach (var primaryProperty in primaryProperties) {
                    if (simObjPhysics.PrimaryProperty == primaryProperty) {
                        return simObjPhysics;
                    }
                }

                foreach (var secondaryProperty in secondaryProperties) {
                    if (simObjPhysics.SecondaryProperties.Contains(secondaryProperty)) {
                        return simObjPhysics;
                    }
                }
            }

            return null;
        }

        private void disableUIElements() {
            var inputModeText = GameObject.Find("DebugCanvasPhysics/InputModeText");
            var throwForceBar = GameObject.Find("DebugCanvasPhysics/ThrowForceBar");
            var crosshair = GameObject.Find("DebugCanvasPhysics/Crosshair");
            var targetText = GameObject.Find("DebugCanvasPhysics/TargetText");
            if (inputModeText) {
                inputModeText.SetActive(false);
            }
            if (throwForceBar) {
                throwForceBar.SetActive(false);
            }
            if (crosshair) {
                crosshair.transform.localScale = Vector3.zero;
            }
            if (targetText) {
                targetText.transform.localScale = Vector3.zero;
            }
        }

        private void enableUIElements() {
            var inputModeText = GameObject.Find("DebugCanvasPhysics/InputModeText");
            var throwForceBar = GameObject.Find("DebugCanvasPhysics/ThrowForceBar");
            var crosshair = GameObject.Find("DebugCanvasPhysics/Crosshair");
            var targetText = GameObject.Find("DebugCanvasPhysics/TargetText");
            if (inputModeText) {
                inputModeText.SetActive(true);
            }
            if (throwForceBar) {
                throwForceBar.SetActive(true);
            }
            if (crosshair) {
                crosshair.transform.localScale = Vector3.one;
            }
            if (targetText) {
                targetText.transform.localScale = Vector3.one;
            }
        }
    }
}
