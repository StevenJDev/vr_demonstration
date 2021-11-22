using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRDemo.Paint
{
	public class PaintManager : MonoBehaviour
	{
		private static PaintManager instance;
		public static PaintManager Instance
		{
			get
			{
				if (instance == null)
				{
					instance = FindObjectOfType<PaintManager>();
					if (instance == null)
					{
						instance = new GameObject("PaintManager", typeof(PaintManager)).GetComponent<PaintManager>();
					}
				}
				return instance;
			}
		}

		[Header("Internal")]
		[SerializeField] private BrushStrokeController brushStrokePrefab;
		public BrushStrokeController BrushStrokePrefab { get { return brushStrokePrefab; } }

		[SerializeField] private BrushSetSettings[] brushSets;
		public BrushSetSettings[] BrushSets { get { return brushSets; } }

		[SerializeField] private StrokeSettings[] strokes;
		public StrokeSettings[] Strokes { get { return strokes; } }

		public List<PaintingController> Paintings { get; private set; }

		private void OnValidate()
		{
			instance = this;
		}

		private void Awake()
		{
			Paintings = new List<PaintingController>();
		}

		public PaintingController CreatePainting(Vector3 position)
		{
			PaintingController painting = new GameObject("Painting_New", typeof(PaintingController)).GetComponent<PaintingController>();
			Paintings.Add(painting);
			return painting;
		}

		public BrushStrokeController CreateBrushStroke(Vector3 position)
		{
			BrushStrokeController stroke = Instantiate(brushStrokePrefab);
			return stroke;
		}
	}
}