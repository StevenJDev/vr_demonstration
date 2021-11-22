using UnityEngine;
using UnityEngine.InputSystem;

namespace VRDemo.Paint
{
	public class PainterController : MonoBehaviour
    {
        public BrushSettings BrushSettings { get; set; }

        public StrokeSettings StrokeSettings { get; set; }

        private PaintingController currentPainting;
        public PaintingController CurrentPainting { get { return currentPainting; } }

        public bool IsPainting { get; private set; }

        private Vector3 lastPointPosition;

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

		public void StartBrushStroke()
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

        public void EndBrushStroke()
		{
            IsPainting = false;
            PlacePoint();
            CurrentPainting.CurrentStroke = null;
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