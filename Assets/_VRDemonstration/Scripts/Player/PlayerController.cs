using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace VRDemo.Player
{
    public class PlayerController : MonoBehaviour
    {
		public enum MovementMode { SMOOTH, TELEPORT }
		[SerializeField] private MovementMode movementStyle;
		[SerializeField] private float movementSpeed = 5f;
		[SerializeField] private float easeAmount = .1f;

		private Vector2 inputAxis = Vector2.zero;
		private Vector3 moveDampVelocity = Vector3.zero;

		public enum RotationMode { SMOOTH, SNAP }
		[SerializeField] private RotationMode rotationStyle;
		[SerializeField] private float snapAngle = 15f;

		protected PlayerInputHandler inputHandler;
		public PlayerInputHandler Input{ get { return inputHandler; } }

		private XRRig xrRig;
		public XRRig XRRig { get { return xrRig; } }

		[SerializeField] private Tool teleporter;

		[SerializeField]
		private Tool rightHandTool;
		public Tool RightHandTool
		{
			get { return rightHandTool; }
			set
			{
				if (rightHandTool != null) { rightHandTool.Unequip(this, XRInputDevice.RIGHT); }
				rightHandTool = value;
				rightHandTool.Equip(this, XRInputDevice.RIGHT);
			}
		}
		
		[SerializeField]
		private Tool leftHandTool;
		public Tool LeftHandTool
		{
			get { return leftHandTool; }
			set
			{
				if (leftHandTool != null) { leftHandTool.Unequip(this, XRInputDevice.LEFT); }
				leftHandTool = value;
				leftHandTool.Equip(this, XRInputDevice.LEFT);
			}
		}

		protected virtual void Awake()
		{
			inputHandler = gameObject.AddComponent<PlayerInputHandler>();
			xrRig = GetComponent<XRRig>();

			if (rightHandTool != null) { rightHandTool.Equip(this, XRInputDevice.RIGHT); }
			if (leftHandTool != null) { leftHandTool.Equip(this, XRInputDevice.LEFT); }
		}

		private void Update()
		{
			if (movementStyle == MovementMode.SMOOTH) { HandleMovement(inputAxis, easeAmount); }
			HandleTurning(inputAxis);
		}

		private void OnAxisChanged(XRInputDevice device, Vector2 axis)
		{
			if (device == XRInputDevice.LEFT)
			{
				inputAxis = axis;
			}
		}

		private void OnEnable()
		{
			Input.onAxisChanged += OnAxisChanged;
		}

		private void OnDisable()
		{
			Input.onAxisChanged -= OnAxisChanged;
		}

		private void HandleMovement(Vector2 axis, float ease)
		{
			Vector3 target = transform.position + (xrRig.cameraGameObject.transform.forward * axis.y * movementSpeed * Time.deltaTime);
			target = new Vector3(target.x, 0f, target.z);
			transform.position = Vector3.SmoothDamp(transform.position, target, ref moveDampVelocity, easeAmount);
		}

		private void HandleTurning(Vector2 axis)
		{

		}

		public virtual void Teleport(Vector3 location, Quaternion rotation)
		{
			XRRig.MoveCameraToWorldLocation(location + new Vector3(0f, XRRig.cameraYOffset, 0f));
		}
	}
}