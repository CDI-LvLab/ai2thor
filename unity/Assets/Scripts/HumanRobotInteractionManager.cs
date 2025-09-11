using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson; // For the BaseFPSAgentController namespace

public class HumanRobotInteractionManager : MonoBehaviour {
#if UNITY_WEBGL
    private bool initialized = false;
    private bool shown = false;

    public void Update() {
        GameObject human = GameObject.Find("FPSController");
        GameObject robot = GameObject.Find("FPSController(Clone)");
        if (robot && !initialized) {
            Debug.Log("Disabling HRIAgentController and JavaScriptInterface on the robot...");
            var controller1 = robot.GetComponent<HRIAgentController>();
            var controller2 = robot.GetComponent<DebugFPSAgentController>();
            var jsInterface = robot.GetComponent<JavaScriptInterface>();
            controller1.enabled = false;
            controller2.enabled = false;
            jsInterface.enabled = false;
            var robotInfo = robot.GetComponent<BaseAgentComponent>();
            robotInfo.AgentName = "robot";

            Debug.Log("Disabling DebugFPSAgentController on the human...");
            var controller3 = robot.GetComponent<DebugFPSAgentController>();
            controller3.enabled = false;
            Camera humanCamera = GameObject.Find("FPSController/FirstPersonCharacter").GetComponent<Camera>();
            var humanInfo = human.GetComponent<BaseAgentComponent>();
            humanInfo.AgentName = "human";

            initialized = true;
        } else if (initialized && !shown) {
            Debug.Log("Showing the robot...");
            ShowRobot();
            shown = true;
        }
    }

    // public void AddRobot() {
    //     AgentManager agentManager = GameObject
    //         .Find("PhysicsSceneManager")
    //         .GetComponentInChildren<AgentManager>();

    //     ServerAction serverAction = new ServerAction();
    //     serverAction.agentCount = 2;
    //     serverAction.makeAgentsVisible = true;

    //     Dictionary<string, object> action = new Dictionary<string, object>();
    //     action["action"] = "Stand";
    //     serverAction.dynamicServerAction = new DynamicServerAction(action);

    //     // agentManager.AddAgents(serverAction);

    //     // Disable the robot's interactive controlls and unset MainCamera tag.
    //     GameObject agent = GameObject.Find("FPSController");
    //     HRIAgentController controller = agent.GetComponent<HRIAgentController>();
    //     controller.enabled = false;
    //     GameObject camera = GameObject.Find("FPSController/FirstPersonCharacter");
    //     camera.tag = "Untagged";

    //     // Disable the human's JS interface
    //     GameObject robotModel = GameObject.Find("FPSController(Clone)");
    //     JavaScriptInterface jsInterface = agent.GetComponent<JavaScriptInterface>();
    //     jsInterface.enabled = false;
    // }

    public void ShowRobot() {
        // Unhide the robot model
        GameObject robotModel = GameObject.Find("FPSController(Clone)/TallVisibilityCapsule");
        foreach (
            MeshRenderer renderer in robotModel.GetComponentsInChildren<MeshRenderer>(true)
        ) {
            renderer.gameObject.SetActive(true);
            renderer.enabled = true;
        }

        // Hide the Human model
        GameObject humanModel = GameObject.Find("FPSController/TallVisibilityCapsule");
        foreach (
            MeshRenderer renderer in humanModel.GetComponentsInChildren<MeshRenderer>(true)
        ) {
            renderer.gameObject.SetActive(false);
            renderer.enabled = false;
        }
    }

    public void EnableHRIMode() {
        var jsInterface = GameObject.Find("FPSController").GetComponent<JavaScriptInterface>();
        var manager = GameObject.Find("PhysicsSceneManager").GetComponent<AgentManager>();
        jsInterface.SetController("HRI");
        manager.onlyEmitOnAction = true;

        string json = @"{
            ""action"": ""Initialize"",
            ""gridSize"": 0.25,
            ""agentCount"": 2,
            ""renderImage"": true,
            ""onlyEmitOnAction"": true,
            ""fieldOfView"": 65
        }";
        jsInterface.Step(json);

        Debug.Log("Agents count: " + manager.agents.Count);
    }
#endif
}
