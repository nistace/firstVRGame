using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRController))]
public class HandControllerInput : MonoBehaviour {
	private XRController controller        { get; set; }
	private InputDevice  controllerDevice  { get; set; }
	public  bool         isRightController => controller && controller.controllerNode == XRNode.RightHand;

	private bool triggerPressed       { get; set; }
	private bool primaryButtonPressed { get; set; }

	public UnityEvent onTriggerPressed       { get; } = new UnityEvent();
	public UnityEvent onPrimaryButtonPressed { get; } = new UnityEvent();

	private void Start() {
		controller = GetComponent<XRController>();
		controllerDevice = InputDevices.GetDeviceAtXRNode(controller.controllerNode);
	}

	private void Update() {
		UpdateTrigger();
		if (!controllerDevice.TryGetFeatureValue(CommonUsages.primaryButton, out var value)) return;
		if (primaryButtonPressed && !value) primaryButtonPressed = false;
		else if (!primaryButtonPressed && value) {
			primaryButtonPressed = true;
			onPrimaryButtonPressed.Invoke();
		}
	}

	private void UpdateTrigger() {
		if (!controllerDevice.TryGetFeatureValue(CommonUsages.trigger, out var value)) return;
		if (triggerPressed && value < .3f) triggerPressed = false;
		else if (!triggerPressed && value > .7f) {
			triggerPressed = true;
			onTriggerPressed.Invoke();
		}
	}
}