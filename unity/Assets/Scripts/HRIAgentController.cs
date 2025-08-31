using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityStandardAssets.Characters.FirstPerson {
    public class HRIAgentController : MonoBehaviour {
        [SerializeField]
        private float HandMoveMagnitude = 0.1f;
        public float ReachableDistance = 5.0f;
        public PhysicsRemoteFPSAgentController PhysicsController = null;
        private bool handMode = false;
        private Camera m_Camera;

        [DllImport("__Internal")]
        private static extern void SetCursorStyle(string style);

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

            PhysicsController = agentManager.PrimaryAgent as PhysicsRemoteFPSAgentController;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Debug_Canvas.GetComponent<Canvas>().enabled = true;

            m_Camera = Camera.main;
        }

        void Update() {
            SimObjPhysics highlighted = null;
            HighlightManager.Instance.Clear();

            GameObject holding = PhysicsController.WhatAmIHolding();

            // If we're holding an object, only allow interacting with Receptacles.
            highlighted = GetSymObjUnderMouse(
                holding ? new SimObjPrimaryProperty[] { } : new SimObjPrimaryProperty[] { SimObjPrimaryProperty.CanPickup },
                new SimObjSecondaryProperty[] { SimObjSecondaryProperty.Receptacle }
            );
            if (highlighted) {
                HighlightManager.Instance.Highlight(highlighted.gameObject);
#if !UNITY_EDITOR && UNITY_WEBGL
                SetCursorStyle("pointer");
#endif
                if (Input.GetMouseButtonDown(0)) {
                    if (!holding && highlighted.PrimaryProperty == SimObjPrimaryProperty.CanPickup) {
                        // We can only pick up an object if not holding another one.
                        PickupObject(highlighted);
                    } else if (highlighted.SecondaryProperties.Contains(SimObjSecondaryProperty.Receptacle)) {
                        // Otherwise, we're looking for somewhere to put the object.
                        if (highlighted.SecondaryProperties.Contains(SimObjSecondaryProperty.CanOpen)) {
                            // If the receptacle can be opened, toggle it to ensure that it is open before putting the object in.
                            if (holding && highlighted.GetComponent<CanOpen_Object>().isOpen) {
                                // If already open, then directly put in.
                                PutdownObject(highlighted);
                            } else {
                                // Otherwise toggle open/close receptacle
                                ToggleReceptacle(highlighted);
                            }
                        } else if (holding) {
                            // Otherwise, the highlighted thing is a receptacle that is always open, like a pan or a bowl.
                            // In a holding state, we treat pickup-able receptacles as receptacles first.
                            PutdownObject(highlighted);
                        }
                    }
                }
            } else {
#if !UNITY_EDITOR && UNITY_WEBGL
                SetCursorStyle("default");
#endif
            }

            if (PhysicsController.ReadyForCommand) {
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
                        executeAction("RotateLeft", "degrees", 30f);
                    }

                    if (Input.GetKeyDown(KeyCode.RightArrow)) {//|| Input.GetKeyDown(KeyCode.L)) {
                        executeAction("RotateRight", "degrees", 30f);
                    }
                }
            }
        }

        private void ToggleReceptacle(SimObjPhysics receptacle) {
            Debug.Log("Toggling Receptacle");
            Dictionary<string, object> action = new Dictionary<string, object>();
            action["action"] = receptacle.GetComponent<CanOpen_Object>().isOpen
                ? "CloseObject"
                : "OpenObject";
            action["objectId"] = receptacle.objectID;
            action["forceAction"] = true;
            this.PhysicsController.ProcessControlCommand(action);
        }

        private void PickupObject(SimObjPhysics simObject) {
            Debug.Log("Picking up object.");
            Dictionary<string, object> action = new Dictionary<string, object>();
            action["action"] = "PickupObject";
            action["objectId"] = simObject.objectID;
            action["forceAction"] = true;
            this.PhysicsController.ProcessControlCommand(action);
        }

        private void PutdownObject(SimObjPhysics simObject) {
            Debug.Log("Putting down object.");
            Dictionary<string, object> action = new Dictionary<string, object>();
            action["action"] = "PutObject";
            action["objectId"] = simObject.objectID;
            action["forceAction"] = true;
            this.PhysicsController.ProcessControlCommand(action);
        }

        private void executeAction(string actionName, string argName, float value) {
            Dictionary<string, object> action = new Dictionary<string, object>();
            action["action"] = actionName;
            action["renderImage"] = true;
            action["onlyEmitOnAction"] = true;
            action[argName] = value;
            PhysicsController.ProcessControlCommand(action);
        }

        private void executeAction(string actionName) {
            Dictionary<string, object> action = new Dictionary<string, object>();
            action["action"] = actionName;
            action["renderImage"] = true;
            action["onlyEmitOnAction"] = true;
            PhysicsController.ProcessControlCommand(action);
        }

        private SimObjPhysics GetSymObjUnderMouse(
            SimObjPrimaryProperty[] primaryProperties,
            SimObjSecondaryProperty[] secondaryProperties
        ) {
            // Raycast from mouse position
            RaycastHit hit = new RaycastHit();
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            int layerMask = LayerMask.GetMask("SimObjVisible", "Procedural1", "Procedural2", "Procedural3", "Procedural0");
            bool hitSomething = Physics.Raycast(ray, out hit, ReachableDistance, layerMask);

            SimObjPhysics resultObject = null;
            // Check for tag "SimObjPhysics"
            if (!hitSomething || hit.transform.tag != "SimObjPhysics") {
                return null;
            }

            // Get SimObjPhysics component
            SimObjPhysics simObjPhysics = hit.transform.GetComponent<SimObjPhysics>();
            if (!simObjPhysics) {
                return null;
            }

            foreach (var primaryProperty in primaryProperties) {
                if (simObjPhysics.PrimaryProperty == primaryProperty) {
                    resultObject = simObjPhysics;
                }
            }

            foreach (var secondaryProperty in secondaryProperties) {
                if (simObjPhysics.SecondaryProperties.Contains(secondaryProperty)) {
                    resultObject = simObjPhysics;
                }
            }

            return resultObject;
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
