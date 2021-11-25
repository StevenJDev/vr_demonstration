using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace VRDemo.Player
{
	public enum XRInputDevice { HEAD, LEFT, RIGHT }

	public delegate void Input1DAxisChanged(XRInputDevice device, float axis);
	public delegate void Input2DAxisChanged(XRInputDevice device, Vector2 axis);
	public delegate void InputButtonChanged(XRInputDevice device);

	/// <summary>
	/// This class might look a little intimidating, but I made it to make processing user input a little easier and cleaner.
	/// You won't really need to look at this class much. Just know that it throws events based on user input.
	/// Add this script to any GameObject, so you can find it with GetComponent<PlayerInputHandler> and subscribe to the input events.
	/// The current setup is based on an Oculus Quest 2 controller. It might support others, but the mapping might be off.
	/// </summary>
	public class PlayerInputHandler : MonoBehaviour
    {
		private const InputDeviceCharacteristics CHARACTERISTICS_LEFT_HAND =
			InputDeviceCharacteristics.HeldInHand |
			InputDeviceCharacteristics.TrackedDevice |
			InputDeviceCharacteristics.Controller |
			InputDeviceCharacteristics.Left;

		private const InputDeviceCharacteristics CHARACTERISTICS_RIGHT_HAND =
			InputDeviceCharacteristics.HeldInHand |
			InputDeviceCharacteristics.TrackedDevice |
			InputDeviceCharacteristics.Controller |
			InputDeviceCharacteristics.Right;

		private const InputDeviceCharacteristics CHARACTERISTICS_HMD =
			InputDeviceCharacteristics.HeadMounted |
			InputDeviceCharacteristics.TrackedDevice;

		private const float MIN_AXIS_VALUE = .01f;

		#region EVENTS
		public event InputButtonChanged onAxisClickStart;
		public event InputButtonChanged onAxisClickHold;
		public event InputButtonChanged onAxisClickEnd;

		public event InputButtonChanged onAxisTouchStart;
		public event InputButtonChanged onAxisTouchHold;
		public event InputButtonChanged onAxisTouchEnd;

		public event Input2DAxisChanged onAxisChanged;

		public event InputButtonChanged onButtonOneClickStart;
		public event InputButtonChanged onButtonOneClickHold;
		public event InputButtonChanged onButtonOneClickEnd;

		public event InputButtonChanged onButtonOneTouchStart;
		public event InputButtonChanged onButtonOneTouchHold;
		public event InputButtonChanged onButtonOneTouchEnd;

		public event InputButtonChanged onButtonTwoClickStart;
		public event InputButtonChanged onButtonTwoClickHold;
		public event InputButtonChanged onButtonTwoClickEnd;

		public event InputButtonChanged onButtonTwoTouchStart;
		public event InputButtonChanged onButtonTwoTouchHold;
		public event InputButtonChanged onButtonTwoTouchEnd;

		public event InputButtonChanged onGripClickStart;
		public event InputButtonChanged onGripClickHold;
		public event InputButtonChanged onGripClickEnd;

		public event InputButtonChanged onGripTouchStart;
		public event InputButtonChanged onGripTouchHold;
		public event InputButtonChanged onGripTouchEnd;

		public event Input1DAxisChanged onGripChanged;

		public event InputButtonChanged onTriggerClickStart;
		public event InputButtonChanged onTriggerClickHold;
		public event InputButtonChanged onTriggerClickEnd;

		public event Input1DAxisChanged onTriggerChanged;
		#endregion

		private class ControllerState
		{
			public bool axisClicked = false;
			public bool axisTouched = false;
			public Vector2 axis = Vector2.zero;

			public bool buttonOneClicked = false;
			public bool buttonOneTouched = false;

			public bool buttonTwoClicked = false;
			public bool buttonTwoTouched = false;

			public bool gripClicked = false;
			public float grip = 0f;

			public bool triggerClicked = false;
			public float trigger = 0f;
		}

		private ControllerState leftControllerState;
		private ControllerState rightControllerState;

		InputDevice hmd;
		InputDevice controllerLeft;
		InputDevice controllerRight;

		private void Awake()
		{
			List<InputDevice> inputDevices = new List<InputDevice>();
			leftControllerState = new ControllerState();
			rightControllerState = new ControllerState();

			InputDevices.GetDevices(inputDevices);
			foreach (InputDevice device in inputDevices)
			{
				SetDevice(device);
			}

			InputDevices.deviceConnected += DeviceConnected;
			InputDevices.deviceDisconnected += DeviceDisconnected;
		}

		private void DeviceDisconnected(InputDevice device)
		{
			Debug.Log($"Device disconnected with name {device.name} and role {device.characteristics.ToString()}");
			SetDevice(device);
		}

		private void DeviceConnected(InputDevice device)
		{
			Debug.Log($"Device connected with name {device.name} and role {device.characteristics.ToString()}");
			SetDevice(device);
		}

		private void SetDevice(InputDevice device)
		{
			string lostOrFound = device == null ? "lost" : "found";
			if (device.characteristics == CHARACTERISTICS_HMD) { hmd = device; Debug.Log($"[InputHandler] {lostOrFound} HMD."); }
			else if (device.characteristics == CHARACTERISTICS_LEFT_HAND) { controllerLeft = device; Debug.Log($"[InputHandler] {lostOrFound} left controller."); }
			else if (device.characteristics == CHARACTERISTICS_RIGHT_HAND) { controllerRight = device; Debug.Log($"[InputHandler] {lostOrFound} right controller."); }
			else
			{
				Debug.Log($"[PlayerInputHandler] device {lostOrFound} with unknown characteristics: {device.characteristics}. Name: {device.name}");
			}
		}

		private void GetControllerInputs(InputDevice controller, XRInputDevice device)
		{
			ControllerState state = device == XRInputDevice.LEFT ? leftControllerState : rightControllerState;
			if (controller.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out bool a))
			{
				if (state.axisClicked == true && a == true) { onAxisClickHold?.Invoke(device); }
				else if (state.axisClicked == false && a == true) { onAxisClickStart?.Invoke(device); state.axisClicked = true; }
				else if (state.axisClicked == true && a == false) { onAxisClickEnd?.Invoke(device); state.axisClicked = false; }
			}

			if (controller.TryGetFeatureValue(CommonUsages.primary2DAxisTouch, out bool at))
			{
				if (state.axisTouched == true && at == true) { onAxisTouchHold?.Invoke(device); }
				else if (state.axisTouched == false && at == true) { onAxisTouchStart?.Invoke(device); state.axisTouched = true; }
				else if (state.axisTouched == true && at == false) { onAxisTouchEnd?.Invoke(device); state.axisTouched = false; }
			}

			if (controller.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 ax))
			{
				if (ax != state.axis && ax.magnitude > MIN_AXIS_VALUE) { onAxisChanged?.Invoke(device, ax); state.axis = ax; }
			}

			if (controller.TryGetFeatureValue(CommonUsages.primaryButton, out bool pb))
			{
				if (state.buttonOneClicked == true && pb == true) { onButtonOneClickHold?.Invoke(device); }
				else if (state.buttonOneClicked == false && pb == true) { onButtonOneClickStart?.Invoke(device); state.buttonOneClicked = true; }
				else if (state.buttonOneClicked == true && pb == false) { onButtonOneClickEnd?.Invoke(device); state.buttonOneClicked = false; }
			}

			if (controller.TryGetFeatureValue(CommonUsages.primaryTouch, out bool pbt))
			{
				if (state.buttonOneTouched == true && pbt == true) { onButtonOneTouchHold?.Invoke(device); }
				else if (state.buttonOneTouched == false && pbt == true) { onButtonOneTouchStart?.Invoke(device); state.buttonOneTouched = true; }
				else if (state.buttonOneTouched == true && pbt == false) { onButtonOneTouchEnd?.Invoke(device); state.buttonOneTouched = false; }
			}

			if (controller.TryGetFeatureValue(CommonUsages.secondaryButton, out bool sb))
			{
				if (state.buttonTwoClicked == true && sb == true) { onButtonTwoClickHold?.Invoke(device); }
				else if (state.buttonTwoClicked == false && sb == true) { onButtonTwoClickStart?.Invoke(device); state.buttonTwoClicked = true; }
				else if (state.buttonTwoClicked == true && sb == false) { onButtonTwoClickEnd?.Invoke(device); state.buttonTwoClicked = false; }
			}

			if (controller.TryGetFeatureValue(CommonUsages.secondaryTouch, out bool sbt))
			{
				if (state.buttonTwoTouched == true && sbt == true) { onButtonTwoTouchHold?.Invoke(device); }
				else if (state.buttonTwoTouched == false && sbt == true) { onButtonTwoTouchStart?.Invoke(device); state.buttonTwoTouched = true; }
				else if (state.buttonTwoTouched == true && sbt == false) { onButtonTwoTouchEnd?.Invoke(device); state.buttonTwoTouched = false; }
			}

			if (controller.TryGetFeatureValue(CommonUsages.gripButton, out bool gb))
			{
				if (state.gripClicked == true && gb == true) { onGripClickHold?.Invoke(device); }
				else if (state.gripClicked == false && gb == true) { onGripClickStart?.Invoke(device); state.gripClicked = true; }
				else if (state.gripClicked == true && gb == false) { onGripClickEnd?.Invoke(device); state.gripClicked = false; }
			}

			if (controller.TryGetFeatureValue(CommonUsages.grip, out float g))
			{
				if (state.grip != g && g > MIN_AXIS_VALUE) { onGripChanged?.Invoke(device, g); state.grip = g; }
			}

			if (controller.TryGetFeatureValue(CommonUsages.triggerButton, out bool tc))
			{
				if (state.triggerClicked == true && tc == true) { onTriggerClickHold?.Invoke(device); }
				else if (state.triggerClicked == false && tc == true) { onTriggerClickStart?.Invoke(device); state.triggerClicked = true; }
				else if (state.triggerClicked == true && tc == false) { onTriggerClickEnd?.Invoke(device); state.triggerClicked = false; }
			}

			if (controller.TryGetFeatureValue(CommonUsages.trigger, out float t))
			{
				if (state.trigger != t && t > MIN_AXIS_VALUE) { onTriggerChanged?.Invoke(device, t); state.trigger = t; }
			}
		}

		private void Update()
		{
			if (controllerLeft != null) { GetControllerInputs(controllerLeft, XRInputDevice.LEFT); }
			if (controllerRight != null) { GetControllerInputs(controllerRight, XRInputDevice.RIGHT); }
		}
	}
}