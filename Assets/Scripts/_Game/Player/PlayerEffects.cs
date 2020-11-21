using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Jampacked.ProjectInca.Events;

namespace Jampacked.ProjectInca
{
	public class PlayerEffects : MonoBehaviour
	{
		[Header("On Hit Effect")]
		[SerializeField] 
		[Range(0, 1)] 
		float onHitVignetteIntensity = 0.5f;
		
		[SerializeField] 
		Color onHitVignetteColor = Color.red;
		
		[SerializeField] 
		float onHitVignetteFadeSpeed = 1f;

		Volume   m_postProcessVolume;
		Vignette m_vignette;

		float m_vignetteInitialIntensity;
		Color m_vignetteInitialColor;
		float m_vignetteFadeParam = 1f;

		//variables for the current effect that's taking place
		float m_effectVignetteIntensity;
		Color m_effectVignetteColor;
		float m_effectVignetteFadeSpeed;

		void Start()
		{
			m_postProcessVolume = FindObjectOfType<Volume>();
			if (!m_postProcessVolume)
			{
				throw new System.NullReferenceException("POST PROCESSING VOLUME FOR PLAYER DOES NOT EXIST!");
			}

			m_postProcessVolume.profile.TryGet(out m_vignette);

			m_vignetteInitialIntensity = m_vignette.intensity.value;
			m_vignetteInitialColor     = m_vignette.color.value;
		}

		void Update()
		{
			if (m_vignetteFadeParam < 1f)
			{
				m_vignetteFadeParam += m_effectVignetteFadeSpeed * Time.deltaTime;

				m_vignette.intensity.value = Mathf.Lerp(m_effectVignetteIntensity, m_vignetteInitialIntensity, m_vignetteFadeParam);
				m_vignette.color.value     = Color.Lerp(m_effectVignetteColor,     m_vignetteInitialColor,     m_vignetteFadeParam);
			}
		}

		public void OnDamaged()
		{
			m_effectVignetteIntensity = onHitVignetteIntensity;
			m_effectVignetteColor     = onHitVignetteColor;
			m_effectVignetteFadeSpeed = onHitVignetteFadeSpeed;

			m_vignetteFadeParam = 0f;
		}
	}
}
