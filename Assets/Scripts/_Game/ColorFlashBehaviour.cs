using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jampacked.ProjectInca
{
	public class ColorFlashBehaviour : MonoBehaviour
	{
		[System.Serializable]
		public struct RendererIndexData
		{
			public Renderer renderer;
			public int      materialIndex;

			public RendererIndexData(Renderer a_renderer, int a_materialIndex)
			{
				renderer      = a_renderer;
				materialIndex = a_materialIndex;
			}
		}
		
		[SerializeField]
		Material materialToAffect = null;
		
		[Tooltip("The gradient representing the color of the flash on hit")]
		[SerializeField]
		[GradientUsage(true)]
		Gradient flashColorGradient = null;

		[SerializeField]
		float flashDuration = 0.2f;

		List<RendererIndexData> m_renderersData = new List<RendererIndexData>();
		MaterialPropertyBlock   m_materialPropertyBlock;

		float m_flashTimerElapsed;

		void Start()
		{
			if (!materialToAffect)
			{
				Debug.LogWarning("WARNING: YOU HAVE NOT SET A COLOR FLASH MATERIAL FOR: " + gameObject.name);
			}
			
			foreach (Renderer bodyRenderer in GetComponentsInChildren<Renderer>(true))
			{
				for (int i = 0; i < bodyRenderer.sharedMaterials.Length; i++)
				{
					if (bodyRenderer.sharedMaterials[i] == materialToAffect)
					{
						bodyRenderer.sharedMaterials[i].EnableKeyword("_EMISSION");
						m_renderersData.Add(new RendererIndexData(bodyRenderer, i));
					}
				}
			}

			m_materialPropertyBlock = new MaterialPropertyBlock();
		}

		public void StartColorFlash()
		{
			m_flashTimerElapsed = 0f;
			
			StartCoroutine(ColorFlashRoutine());
		}

		IEnumerator ColorFlashRoutine()
		{
			while (m_flashTimerElapsed < flashDuration)
			{
				m_flashTimerElapsed += Time.deltaTime;

				SetBodyColorFromGradient(m_flashTimerElapsed / flashDuration);

				yield return null;
			}

			SetBodyColorFromGradient(1f);
		}

		void SetBodyColorFromGradient(float a_evaluateTime)
		{
			Color currentColor = flashColorGradient.Evaluate(a_evaluateTime);

			m_materialPropertyBlock.SetColor("_EmissionColor", currentColor);
			foreach (var rendererData in m_renderersData)
			{
				rendererData.renderer.SetPropertyBlock(m_materialPropertyBlock, rendererData.materialIndex);
			}
		}
	}
}
