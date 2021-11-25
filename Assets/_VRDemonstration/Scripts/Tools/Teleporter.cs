using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace VRDemo.Player
{
	public struct TeleportResult
	{
		public enum Validity { NO_HIT, HIT, VALID_HIT }
		public Validity validty;
		public Vector3[] positions;
		public float[] sqrDistances;
		public float totalSqrDistance;

		public Quaternion Rotation { get; set; }
		public Vector3 Location
		{
			get
			{
				if (positions.Length > 0)
				{
					return positions[positions.Length - 1];
				}
				else
				{
					return Vector3.zero;
				}
			}
		}
	}

	public class Teleporter : Tool
	{
		public const int MAX_POINTS = 100;

		public enum RotationMode { NONE, XZ, XYZ }
		[Header("Teleport Motion")]
		[SerializeField]
		private RotationMode rotationMode;

		[Header("Teleport Cast")]
		[SerializeField] private LayerMask validTeleportMask;
		[SerializeField] private LayerMask collisionMask;
		[Space(4f)]
		[SerializeField, Range(-1f, 1f),
			Tooltip("Maximum amount of difference between rig up and the up vector of the target surface. -1 means full difference, 1 means none")]
		private float maxUpDeviation = .5f;
		[SerializeField] private float maxDistance = 10f;
		[SerializeField] private Vector3 gravity;
		[SerializeField] private float velocity;

		private XRInputDevice device;
		private PlayerController player;
		private TeleporterView view;
		private Vector2 inputAxis;
		private float inputAxisChangeLastTime = 0f;

		private bool isTeleporting = false;
		

		private void Awake()
		{
			view = GetComponent<TeleporterView>();
			view.Init();
			inputAxis = new Vector2(0f, 1f);
		}

		private void Update()
		{
			if (isTeleporting)
			{
				view.SetLineRenderer(GetArcPoints());
			}
		}

		public override void Equip(PlayerController player, XRInputDevice device)
		{
			Debug.Log($"Equipped Teleporter on {device}");
			this.device = device;
			this.player = player;

			player.Input.onTriggerClickStart += OnTriggerClickStart;
			player.Input.onTriggerClickEnd += OnTriggerClickEnd;
			player.Input.onAxisChanged += onAxisChanged;

			this.enabled = true;
		}

		public override void Unequip(PlayerController player, XRInputDevice device)
		{
			if (isTeleporting)
			{
				AbortTeleport();
			}

			player.Input.onTriggerClickStart -= OnTriggerClickStart;
			player.Input.onTriggerClickEnd -= OnTriggerClickEnd;
			player.Input.onAxisChanged -= onAxisChanged;
			player = null;

			this.enabled = false;
			view.enabled = false;
		}

		private void OnTriggerClickStart(XRInputDevice device)
		{
			if (this.device == device && !isTeleporting)
			{
				StartTeleport();
			}
		}

		private void OnTriggerClickEnd(XRInputDevice device)
		{
			if (this.device == device && isTeleporting)
			{
				if (inputAxis.magnitude < .5f) { AbortTeleport(); }
				else { EndTeleport(); }
			}	
		}

		private void onAxisChanged(XRInputDevice device, Vector2 axis)
		{
			if (this.device == device)
			{
				if (axis.magnitude > .5f)
				{
					inputAxisChangeLastTime = Time.time;
					inputAxis = axis.normalized;
				}
				else
				{
					if (inputAxisChangeLastTime > Time.time + .2f)
					{
						inputAxis = axis.normalized;
					}
				}
			}
		}

		private void StartTeleport()
		{
			Debug.Log("[Teleporter] StartTeleport()");
			isTeleporting = true;
			view.enabled = true;
		}

		private void AbortTeleport()
		{
			Debug.Log("[Teleporter] AbortTeleport()");
			isTeleporting = false;
			view.enabled = false;
		}

		private void EndTeleport()
		{
			view.enabled = false;
			isTeleporting = false;
			TryTeleport(GetArcPoints());
		}

		private void TryTeleport(TeleportResult result)
		{
			Debug.Log("[Teleporter] TryTeleport()");
			if (result.validty == TeleportResult.Validity.VALID_HIT)
			{
				player.Teleport(result.Location, result.Rotation);
			}
		}

		private TeleportResult GetArcPoints()
		{
			if (view != null) { view.Marker.enabled = false; }

			List<Vector3> arcPoints = new List<Vector3>();
			List<float> arcPointsSqrDistances = new List<float>();

			arcPoints.Add(transform.position);
			arcPointsSqrDistances.Add(0f);

			TeleportResult result = new TeleportResult();

			float currentSqrDistance = 0f;
			while (currentSqrDistance < maxDistance && arcPoints.Count < MAX_POINTS)
			{
				Vector3 newPoint = arcPoints[arcPoints.Count - 1];

				newPoint += (transform.forward * velocity);
				newPoint += gravity * (arcPoints.Count);

				RaycastHit hit;
				if (Physics.Linecast(arcPoints[arcPoints.Count - 1], newPoint, out hit, validTeleportMask | collisionMask, QueryTriggerInteraction.Ignore))
				{
					newPoint = hit.point;
					arcPoints.Add(newPoint);

					currentSqrDistance += (newPoint - arcPoints[arcPoints.Count - 2]).sqrMagnitude;
					arcPointsSqrDistances.Add(currentSqrDistance);

					result.validty = TeleportResult.Validity.HIT;

					if (IsInLayer(validTeleportMask, hit.collider.gameObject.layer)
						&& (Vector3.Dot(player != null ? player.transform.up : Vector3.up, hit.normal) > maxUpDeviation))
					{
						result.validty = TeleportResult.Validity.VALID_HIT;
						//result.Rotation = Quaternion.Euler(new Vector3())
						if (view != null)
						{
							view.Marker.enabled = true;
							view.Marker.transform.position = hit.point;
							view.Marker.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
						}
					}
					break;
				}
				else
				{
					arcPoints.Add(newPoint);
					currentSqrDistance += (newPoint - arcPoints[arcPoints.Count - 2]).sqrMagnitude;
					arcPointsSqrDistances.Add(currentSqrDistance);
				}
			}

			result.positions = arcPoints.ToArray();
			result.sqrDistances = arcPointsSqrDistances.ToArray();
			result.totalSqrDistance = currentSqrDistance;
			return result;
		}

		private bool IsInLayer(LayerMask mask, int layer)
		{
			return ((mask & (1 << layer)) != 0);
		}

		private void OnDrawGizmosSelected()
		{
			TeleportResult teleport = GetArcPoints();

			switch (teleport.validty)
			{
				case TeleportResult.Validity.NO_HIT: Gizmos.color = Color.red; break;
				case TeleportResult.Validity.HIT: Gizmos.color = Color.yellow; break;
				case TeleportResult.Validity.VALID_HIT: Gizmos.color = Color.green; break;
			}

			int count = teleport.positions.Length;
			for (int i = 0; i < count; i++)
			{
				float size = Mathf.InverseLerp(teleport.totalSqrDistance, 0f, teleport.sqrDistances[i]) * .1f;
				Gizmos.DrawWireSphere(teleport.positions[i], size + .025f);
			}
		}
	}
}