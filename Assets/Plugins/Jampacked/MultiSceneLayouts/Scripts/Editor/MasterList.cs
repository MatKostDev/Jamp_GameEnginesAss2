using System.Collections.Generic;

using UnityEditor;

using UnityEditorInternal;

using UnityEngine;

namespace Jampacked.ProjectInca.Editor.SceneLayoutManager
{
	internal class MasterList
	{
		private readonly ReorderableList m_masterList;

		public readonly List<SubList> masterIList = new List<SubList>();

		private readonly SceneLayoutManager m_manager;

		public MasterList(SceneLayoutManager a_manager)
		{
			m_manager = a_manager;

			m_masterList = new ReorderableList(masterIList, typeof(SubList))
			{
				drawElementCallback   = OnDrawElement,
				onAddCallback         = OnAddElement,
				onRemoveCallback      = OnRemoveElement,
				drawHeaderCallback    = OnDrawHeader,
				elementHeightCallback = OnElementHeight,
			};
		}

		public void Draw()
		{
			m_masterList.DoLayoutList();
		}

		public void OnDrawElement(Rect a_rect, int a_index, bool a_isactive, bool a_isfocused)
		{
			if (m_masterList.list[a_index] is SubList subList)
			{
				a_rect.y += 5;

				subList.DoList(a_rect);

				if ((m_manager.SaveAll || m_manager.Autosave) && subList.CurrentLayout.IsDirty)
				{
					subList.CurrentLayout.Save();
				}
			}
		}

		public void OnAddElement(ReorderableList a_list)
		{
			a_list.index = a_list.count;

			var sceneLayout = new SceneLayout(m_manager)
			{
				title = $"Scene Layout {a_list.count + 1}",
			};

			masterIList.Add(new SubList(m_manager, sceneLayout));
		}

		public void OnRemoveElement(ReorderableList a_list)
		{
			var index = a_list.index;

			masterIList.RemoveAt(index);
		}

		public void OnDrawHeader(Rect a_rect)
		{
			EditorGUI.LabelField(a_rect, "Layouts");
		}

		public float OnElementHeight(int a_index)
		{
			var subList = masterIList[a_index];

			float height = subList.GetHeight() + 10;

			return height;
		}
	}
}
