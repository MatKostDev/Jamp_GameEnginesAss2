using TMPro;
using UnityEngine;

namespace Jampacked.ProjectInca
{
	[RequireComponent(typeof(TMP_Text), typeof(SimplePhysics))]
	public class DamageNumberDisplay : MonoBehaviour
	{
		[Header("Color")]
		[SerializeField]
		Color regularColor = Color.black;

		[SerializeField]
		Color weakSpotColor = Color.red;
		
		[Header("Velocity")]
		[SerializeField]
		float startSpeed = 5f;
		
		[SerializeField]
		Vector3 minStartVelocity = Vector3.zero;

		[SerializeField]
		Vector3 maxStartVelocity = Vector3.zero;

		[Header("Other")]
		[SerializeField]
		float sizeMultiplier = 1f;

		[SerializeField]
		float fadeSpeed = 1f;

		const float START_ALPHA = 2f;
		const float END_ALPHA   = 0f;

		const float MAX_SCALE_VALUE = 0.3f;
		const float MIN_SCALE_VALUE = 0.08f;

		Transform     m_cameraTransform;
		Transform     m_transform;
		SimplePhysics m_physics;
		TMP_Text      m_textDisplay;

		float m_interpolationParam;

		public void Init(Transform a_cameraTransform, Vector3 a_hitPosition, float a_damageDealt, bool a_isWeakSpotHit)
		{
			m_transform   = transform;
			m_textDisplay = GetComponent<TMP_Text>();
			m_physics     = GetComponent<SimplePhysics>();

			m_cameraTransform = a_cameraTransform;

            m_interpolationParam = 0f;

			m_transform.position = a_hitPosition;

			m_textDisplay.text = Mathf.RoundToInt(a_damageDealt).ToString();

			if (a_isWeakSpotHit)
			{
				m_textDisplay.color = weakSpotColor;
			} else
			{
				m_textDisplay.color = regularColor;
			}

			Vector3 startVelocity = new Vector3
			{
				x = Random.Range(minStartVelocity.x, maxStartVelocity.x),
				y = Random.Range(minStartVelocity.y, maxStartVelocity.y),
				z = Random.Range(minStartVelocity.z, maxStartVelocity.z),
			};

			startVelocity = startVelocity.normalized * startSpeed;

			m_physics.Velocity = startVelocity;
		}

		void Update()
		{
			Vector3 currentPosition = m_transform.position;
			Vector3 cameraPosition  = m_cameraTransform.position;
			
			float distanceToViewer = Vector3.Distance(cameraPosition, currentPosition);
			float newScaleValue    = distanceToViewer * sizeMultiplier;

			newScaleValue = Mathf.Clamp(newScaleValue, MIN_SCALE_VALUE, MAX_SCALE_VALUE);

			m_transform.localScale = new Vector3(newScaleValue, newScaleValue, 1f);

			//face the camera, this looks a bit jank because just doing it normally makes it look the opposite direction
			m_transform.LookAt((2f * currentPosition) - cameraPosition);

			float newAlpha = Mathf.Lerp(START_ALPHA, END_ALPHA, m_interpolationParam);

			Color newColor = m_textDisplay.color;

			newColor.a          = newAlpha;
			m_textDisplay.color = newColor;

			if (m_interpolationParam >= 1f)
			{
				DamageNumberPooler.Instance.ResetObject(gameObject);
			}

			m_interpolationParam += fadeSpeed * Time.deltaTime;
		}
	}
}
