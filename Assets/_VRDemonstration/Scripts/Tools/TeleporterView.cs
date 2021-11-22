using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRDemo.Player
{
	[RequireComponent(typeof(LineRenderer))]
    public class TeleporterView : MonoBehaviour
    {
		[SerializeField]
        private TeleportMarker marker;
		public TeleportMarker Marker { get { return marker; } }

		[SerializeField] private Color colorNoHit;
		[SerializeField] private Color colorHit;
		[SerializeField] private Color colorValidHit;

		private LineRenderer lineRenderer;

		public void Init()
		{
			marker.Init();
			lineRenderer = GetComponent<LineRenderer>();
			lineRenderer.useWorldSpace = true;
			enabled = false;
		}

		private void OnEnable()
		{
			if (lineRenderer != null) { lineRenderer.enabled = true; }
		}

		private void OnDisable()
		{
			lineRenderer.enabled = false;
			marker.gameObject.SetActive(false);
		}

		public void SetLineRenderer(TeleportResult teleportResult)
		{
			lineRenderer.positionCount = teleportResult.positions.Length;
			lineRenderer.SetPositions(teleportResult.positions);

			Color resultColor = Color.black;
			switch (teleportResult.validty)
			{
				case TeleportResult.Validity.NO_HIT: resultColor = colorNoHit; break;
				case TeleportResult.Validity.HIT: resultColor = colorHit; break;
				case TeleportResult.Validity.VALID_HIT: resultColor = colorValidHit; break;
			}

			lineRenderer.material.SetColor("_Color", resultColor);
			lineRenderer.startColor = resultColor;
			lineRenderer.endColor = Color.black;

			lineRenderer.enabled = true;
		}
	}
}