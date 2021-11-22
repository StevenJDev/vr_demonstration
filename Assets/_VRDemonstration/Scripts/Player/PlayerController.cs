using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRDemo.Player
{
    public class PlayerController : MonoBehaviour
    {
		protected PlayerInputHandler inputHandler;
		public PlayerInputHandler Input{ get { return inputHandler; } }

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
			if (rightHandTool != null) { rightHandTool.Equip(this, XRInputDevice.RIGHT); }
			if (leftHandTool != null) { leftHandTool.Equip(this, XRInputDevice.LEFT); }
		}

		public virtual void Teleport(Vector3 location, Quaternion rotation)
		{
			
		}
	}
}