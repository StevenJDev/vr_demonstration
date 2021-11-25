using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRDemo.Test
{
    public class RotationTest : MonoBehaviour
    {
        [SerializeField] Vector2 inputAxis = Vector2.up;
        [SerializeField] private GameObject marker;

		// Update is called once per frame
		void Update()
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit))
			{
                marker.transform.position = hit.point;

                Quaternion rotation = Quaternion.Euler(-90f, 0f, 0f);
                marker.transform.rotation = Quaternion.LookRotation(rotation * hit.normal);
                marker.transform.Rotate(marker.transform.up, Vector2.SignedAngle(Vector2.up, inputAxis));
			}
			else
			{
                marker.transform.position = Vector3.zero;
                marker.transform.rotation = Quaternion.identity;
			}
        }
    }
}