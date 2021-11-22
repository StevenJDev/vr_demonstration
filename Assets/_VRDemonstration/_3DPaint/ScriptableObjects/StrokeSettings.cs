using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRDemo.Paint
{
	[CreateAssetMenu(fileName = "Stroke ", menuName = "3DPaint/Brushes/Stroke Settings")]
	public class StrokeSettings : ScriptableObject
	{
		[SerializeField] private string strokeName;
		public string Name { get { return strokeName; } }

		[SerializeField] private float pointDistance;
		public float PointDistance { get { return pointDistance; } }

		[SerializeField] private Color color;
		public Color Color { get { return color; } }
	}
}