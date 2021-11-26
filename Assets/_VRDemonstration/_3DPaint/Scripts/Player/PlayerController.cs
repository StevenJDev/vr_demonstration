using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRDemo.Player;

namespace VRDemo.Paint
{
    public class PlayerController : VRDemo.Player.PlayerController
    {
        [SerializeField]
        private PainterController rightPainter;

		private void OnEnable()
		{
			inputHandler.onTriggerClickStart += OnTriggerClickStart;
			inputHandler.onTriggerClickEnd += OnTriggerClickEnd; ;
		}

		private void OnTriggerClickStart(XRInputDevice device)
		{
			if (device == XRInputDevice.RIGHT)
			{
				rightPainter.StartBrushStroke(device);
			}
		}

		private void OnTriggerClickEnd(XRInputDevice device)
		{
			if (device == XRInputDevice.RIGHT)
			{
				rightPainter.EndBrushStroke(device);
			}
		}
	}
}