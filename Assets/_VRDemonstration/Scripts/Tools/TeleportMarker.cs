using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRDemo.Player
{
	public class TeleportMarker : MonoBehaviour
	{
		[SerializeField]
		private Material material;

		[SerializeField]
		private MeshRenderer ring;
		[SerializeField]
		private MeshRenderer oneArrow;
		[SerializeField]
		private MeshRenderer threeArrows;

		public void Init()
		{
			ring.sharedMaterial = material;
			oneArrow.sharedMaterial = material;
			threeArrows.sharedMaterial = material;
		}


	}
}