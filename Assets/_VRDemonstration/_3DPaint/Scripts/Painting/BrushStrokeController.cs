using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRDemo.Paint
{
	[RequireComponent(typeof(LineRenderer))]
	public class BrushStrokeController : MonoBehaviour
	{
		private BrushStrokeData data;
		public BrushStrokeData Data { get { return data; } }

		private LineRenderer lineRenderer;

		public void Init(BrushSettings settings)
		{
			this.data = new BrushStrokeData(transform.localPosition, settings);
			lineRenderer = GetComponent<LineRenderer>();
			lineRenderer.useWorldSpace = false;

			lineRenderer.sharedMaterial = settings.Material;
			lineRenderer.startColor = settings.StartColor;
			lineRenderer.endColor = settings.EndColor;
			lineRenderer.startWidth = settings.StartWidth;
			lineRenderer.endWidth = settings.EndWidth;
			UpdateLine();
		}

		public void AddPoint(Vector3 positionInPaintingSpace)
		{
			PaintPointData point = new PaintPointData(transform.InverseTransformPoint(positionInPaintingSpace));
			Data.points.Add(point);
			UpdateLine();
		}

		public void MoveLastPoint(Vector3 positionInPaintingSpace)
		{
			if (Data.points.Count > 0)
			{
				Data.points[Data.points.Count - 1].localPosition = transform.InverseTransformPoint(positionInPaintingSpace);
				UpdateLine();
			}
		}

		private void UpdateLine()
		{
			transform.localPosition = data.localPosition;

			lineRenderer.positionCount = data.points.Count;
			for (int i = 0; i < data.points.Count; i++) { lineRenderer.SetPosition(i, data.points[i].localPosition); }
		}
	}
}