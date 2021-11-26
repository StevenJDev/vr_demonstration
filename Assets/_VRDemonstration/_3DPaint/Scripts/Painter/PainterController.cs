using UnityEngine;
using UnityEngine.InputSystem;
using VRDemo.Player;

namespace VRDemo.Paint
{
	public class PainterController : Tool
    {
        public BrushSettings BrushSettings { get; set; }

        public StrokeSettings StrokeSettings { get; set; }

        private PaintingController currentPainting;
        public PaintingController CurrentPainting { get { return currentPainting; } }

        public bool IsPainting { get; private set; }

        private Vector3 lastPointPosition;

		XRInputDevice device;

		public override void Equip(Player.PlayerController player, XRInputDevice device)
		{
			this.device = device;
			player.Input.onTriggerClickStart += StartBrushStroke;
			player.Input.onTriggerClickEnd += EndBrushStroke;
		}

		public override void Unequip(Player.PlayerController player, XRInputDevice device)
		{
			player.Input.onTriggerClickStart -= StartBrushStroke;
			player.Input.onTriggerClickEnd -= EndBrushStroke;
		}

		private void OnValidate()
        {
            if (PaintManager.Instance == null)
			{
                //If instance was equal to null, a new paintmanager will have been created and can now be used
			}
        }

		private void Awake()
		{
			BrushSettings = PaintManager.Instance.BrushSets[0].Brushes[0];
			StrokeSettings = PaintManager.Instance.Strokes[0];
		}

		public void StartBrushStroke(XRInputDevice device)
		{
			Debug.Log("Starting brush stroke");
			if (this.device == device)
			{
				if (currentPainting == null)
				{
					currentPainting = PaintManager.Instance.CreatePainting(transform.position);
				}

				if (currentPainting.CurrentStroke == null)
				{
					currentPainting.AddBrushStroke(BrushSettings);
				}

				IsPainting = true;
			}
		}

        public void EndBrushStroke(XRInputDevice device)
		{
			Debug.Log("Ending brush stroke");
			if (this.device == device)
			{
				IsPainting = false;
				PlacePoint();
				CurrentPainting.CurrentStroke = null;
			}
		}

        private void PlacePoint()
		{
			Vector3 localPos = CurrentPainting.transform.InverseTransformPoint(transform.position);
			CurrentPainting.CurrentStroke.AddPoint(localPos);
			lastPointPosition = localPos;
		}

		private void Update()
		{
			if (IsPainting)
			{
                if (Vector3.Distance(transform.position, lastPointPosition) >= StrokeSettings.PointDistance)
				{
                    PlacePoint();
				}
				else
				{
                    CurrentPainting.CurrentStroke.MoveLastPoint(transform.position);
				}
			}
		}
	}
}