using System.Collections.Generic;

using UnityEditor;

using UnityEditorInternal;

using UnityEngine;

namespace Jampacked.ProjectInca.Editor.SceneLayoutManager
{
	internal class SubList
	{
		protected SceneLayout m_currentLayout;

		public SceneLayout CurrentLayout
		{
			get { return m_currentLayout; }
			set
			{
				if (value == null)
				{
					return;
				}

				m_currentLayout = value;
				rlist.list      = m_currentLayout.sceneWrappers;
			}
		}

		protected readonly ReorderableList rlist;

		protected SceneLayoutManager m_manager;

		public SubList(SceneLayoutManager a_manager)
		{
			m_manager = a_manager;

			rlist = new ReorderableList(new List<SceneAssetWrapper>(), typeof(SceneAssetWrapper))
			{
				drawElementCallback   = OnDrawElement,
				onAddCallback         = OnAddElement,
				drawHeaderCallback    = OnDrawHeader,
				elementHeightCallback = OnElementHeight,
			};
		}

		public SubList(SceneLayoutManager a_manager, SceneLayout a_layout)
		{
			m_manager = a_manager;

			m_currentLayout = a_layout;

			rlist = new ReorderableList(m_currentLayout.sceneWrappers, typeof(SceneAssetWrapper))
			{
				drawElementCallback   = OnDrawElement,
				onAddCallback         = OnAddElement,
				drawHeaderCallback    = OnDrawHeader,
				elementHeightCallback = OnElementHeight,
			};
		}

		public void DoLayoutList()
		{
			var rect = GUILayoutUtility.GetRect(0, rlist.GetHeight(), GUILayout.ExpandHeight(true));
			DoList(rect);
		}

		public void DoList(Rect a_rect)
		{
			rlist.DoList(a_rect);

			a_rect.y += rlist.GetHeight() - EditorGUIUtility.singleLineHeight;

			a_rect.width  -= 70;
			a_rect.height =  EditorGUIUtility.singleLineHeight;

			var temp = a_rect;

			temp.width = a_rect.width / 3;
			if (GUI.Button(temp, "Apply"))
			{
				//CurrentLayout.Apply();
				m_manager.ApplyLayout(CurrentLayout);
			}

			temp.x += temp.width;
			if (GUI.Button(temp, "Save"))
			{
				CurrentLayout.Save();
			}

			EditorGUI.BeginDisabledGroup(!CurrentLayout.HasFileHandle);
			temp.x += temp.width;
			if (GUI.Button(temp, "Reload"))
			{
				CurrentLayout.Load();
			}
			EditorGUI.EndDisabledGroup();
		}

		public void OnDrawElement(Rect a_rect, int a_index, bool a_isactive, bool a_isfocused)
		{
			var wrapper = CurrentLayout.sceneWrappers[a_index];

			a_rect.y += 2;

			Rect template = a_rect;
			template.width  = (a_rect.width / 2f) - 22f;
			template.height = EditorGUIUtility.singleLineHeight;

			Rect sceneAssetRect = template;
			sceneAssetRect.y += a_rect.height / 4;

			EditorGUI.BeginChangeCheck();

			var sceneAsset
				= EditorGUI.ObjectField(
					  sceneAssetRect,
					  wrapper.scene,
					  typeof(SceneAsset),
					  true
				  ) as SceneAsset;

			if (EditorGUI.EndChangeCheck())
			{
				bool isValid = true;
				for (int i = 0; isValid && i < CurrentLayout.sceneWrappers.Count; i++)
				{
					if (sceneAsset == CurrentLayout.sceneWrappers[i].scene)
					{
						isValid = false;
					}
				}

				if (isValid)
				{
					wrapper.scene = sceneAsset;
					CurrentLayout.SetDirty();
				}
			}
			
			float cachedLabelWidth = EditorGUIUtility.labelWidth;

			EditorGUIUtility.labelWidth = 45;

			Rect positionRect = template;
			positionRect.x += template.width + 5;

			var pRect = EditorGUI.PrefixLabel(positionRect, new GUIContent("Position"), EditorStyles.miniBoldLabel);
			
			var pos
				= EditorGUI.Vector3Field(
					pRect,
					GUIContent.none,
					wrapper.position
				);

			wrapper.position = pos;

			Rect rotationRect = positionRect;
			rotationRect.y += a_rect.height / 2f;

			var rRect = EditorGUI.PrefixLabel(rotationRect, new GUIContent("Rotation"), EditorStyles.miniBoldLabel);

			var rot
				= EditorGUI.Vector3Field(
					rRect,
					GUIContent.none,
					wrapper.rotation
				);

			wrapper.rotation = rot;

			EditorGUIUtility.labelWidth = cachedLabelWidth;

			Rect transformRect = template;
			transformRect.x     = positionRect.x + template.width + 5;
			transformRect.y     = sceneAssetRect.y;
			transformRect.width = 36;

			Transform transform;
			EditorGUI.BeginChangeCheck();
			{
				transform
					= EditorGUI.ObjectField(
						  transformRect,
						  null,
						  typeof(Transform),
						  true
					  ) as Transform;
			}
			if (EditorGUI.EndChangeCheck())
			{
				if (transform != null)
				{
					wrapper.position = transform.position;
					wrapper.rotation = transform.rotation.eulerAngles;
				} else
				{
					wrapper.position = new Vector3();
					wrapper.rotation = new Vector3();
				}
			}
		}

		public void OnAddElement(ReorderableList a_list)
		{
			a_list.index = a_list.list.Count;

			a_list.list.Add(new SceneAssetWrapper());
		}

		public float OnElementHeight(int a_index)
		{
			return (EditorGUIUtility.singleLineHeight * 2) + 5;
		}

		public void OnDrawHeader(Rect a_rect)
		{
			// If the SceneLayout is dirty, make the label bold
			GUIStyle style = CurrentLayout.IsDirty
				                 ? EditorStyles.boldLabel
				                 : EditorStyles.label;

			if (!CurrentLayout.readOnly)
			{
				CurrentLayout.title = EditorGUI.TextField(a_rect, CurrentLayout.title, style);
			} else
			{
				EditorGUI.LabelField(a_rect, CurrentLayout.title, style);
			}
		}

		public float GetHeight()
		{
			return rlist.GetHeight();
		}
	}
}
