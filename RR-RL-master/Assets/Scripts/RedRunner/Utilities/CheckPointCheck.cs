using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedRunner.Utilities
{

	public class CheckPointCheck : MonoBehaviour
	{

		public delegate void CheckPointedHandler ();

		public event CheckPointedHandler OnCheckPointed;

		public const string CHECKPOINT_TAG = "Checkpoint";
		public const string CHECKPOINT_LAYER_NAME = "CheckpointLayer";

		[SerializeField]
		private Collider2D m_Collider2D;

		[SerializeField]
		private float m_RayDistance = 0.5f;

		public bool IsCheckPointed { get { return m_IsCheckPointed; } }

		private bool m_IsCheckPointed = false;

		void Awake ()
		{
			m_IsCheckPointed = false;
		}

		void Update ()
		{
			//questi 3 li uso con ugual nome del file 'GroundCheck', tanto sono variabili locali a questa classe e a questo metodo
			Vector2 left = new Vector2 (m_Collider2D.bounds.max.x, m_Collider2D.bounds.center.y);
			Vector2 center = new Vector2 (m_Collider2D.bounds.center.x, m_Collider2D.bounds.center.y);
			Vector2 right = new Vector2 (m_Collider2D.bounds.min.x, m_Collider2D.bounds.center.y);
		
			RaycastHit2D hit1 = Physics2D.Raycast (left, new Vector2 (0f, -1f), m_RayDistance, LayerMask.GetMask (CHECKPOINT_LAYER_NAME));
			Debug.DrawRay (left, new Vector2 (0f, -m_RayDistance));
			bool checkPointed1 = hit1 != null && hit1.collider != null && hit1.collider.CompareTag (CHECKPOINT_TAG);
		
			RaycastHit2D hit2 = Physics2D.Raycast (center, new Vector2 (0f, -1f), m_RayDistance, LayerMask.GetMask (CHECKPOINT_LAYER_NAME));
			Debug.DrawRay (center, new Vector2 (0f, -m_RayDistance));
			bool checkPointed2 = hit2 != null && hit2.collider != null && hit2.collider.CompareTag (CHECKPOINT_TAG);
		
			RaycastHit2D hit3 = Physics2D.Raycast (right, new Vector2 (0f, -1f), m_RayDistance, LayerMask.GetMask (CHECKPOINT_LAYER_NAME));
			Debug.DrawRay (right, new Vector2 (0f, -m_RayDistance));
			bool checkPointed3 = hit3 != null && hit3.collider != null && hit3.collider.CompareTag (CHECKPOINT_TAG);

			bool checkPointed = checkPointed1 || checkPointed2 || checkPointed3;
		
			if (checkPointed && !m_IsCheckPointed) {
				if (OnCheckPointed != null) {
					OnCheckPointed ();
				}
			}

			m_IsCheckPointed = checkPointed;
		}

	}

}
