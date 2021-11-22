using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRDemo;

namespace VRDemo.Paint
{
	[System.Serializable]
	public class PaintPointData
	{
		public Vector3 localPosition { get; set; }

		public PaintPointData(Vector3 localPosition)
		{
			this.localPosition = localPosition;
		}
	}

	[System.Serializable]
	public class BrushStrokeData
	{
		public Vector3 localPosition { get; private set; }
		public List<PaintPointData> points;
		public Color startColor;
		public Color endColor;
		public float startWidth;
		public float endWidth;

		public BrushStrokeData(Vector3 localPosition, BrushSettings settings)
		{
			points = new List<PaintPointData>();

			this.localPosition = localPosition;
			this.startColor = settings.StartColor;
			this.endColor = settings.EndColor;
			this.startWidth = settings.StartWidth;
			this.endWidth = settings.EndWidth;
		}
	}

    public class PaintingController : MonoBehaviour
    {
		public List<BrushStrokeController> BrushStrokes { get; private set; }

		public BrushStrokeController CurrentStroke { get; set; }

		private Vector3 calculatedCenter;
		public Vector3 CalculatedCenter { get { return calculatedCenter; } }

		private void Awake()
		{
			BrushStrokes = new List<BrushStrokeController>();
		}

		public void AddBrushStroke(BrushSettings settings)
		{
			CurrentStroke = PaintManager.Instance.CreateBrushStroke(transform.position);
			CurrentStroke.transform.parent = transform;
			CurrentStroke.Init(settings);
			BrushStrokes.Add(CurrentStroke);
		}
    }
}