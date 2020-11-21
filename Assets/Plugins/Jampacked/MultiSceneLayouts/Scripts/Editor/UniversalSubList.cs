using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEditor;

using UnityEditorInternal;

using UnityEngine;

namespace Jampacked.ProjectInca.Editor.SceneLayoutManager
{
	[Serializable]
	internal class SceneLayoutConfig
	{
		public bool treatUniversalsAsActive = false;

		public List<SceneAssetWrapper> universalScenes;

		[NonSerialized]
		public bool cachedTreatUniversalAsActive;
	}

	internal class UniversalSubList : SubList
	{
		private SceneLayoutConfig m_config = new SceneLayoutConfig();

		public bool TreatUniversalAsActive
		{
			get { return m_config.treatUniversalsAsActive; }
		}

		public bool IsDirty
		{
			get
			{
				return m_config.cachedTreatUniversalAsActive != m_config.treatUniversalsAsActive
				       || m_currentLayout.IsDirty;
			}
		}

		public UniversalSubList(SceneLayoutManager a_manager, SceneLayout a_layout)
			: base(a_manager, a_layout)
		{
			m_config.universalScenes = a_layout.sceneWrappers;
		}

		public new void DoLayoutList()
		{
			var rect = GUILayoutUtility.GetRect(0, rlist.GetHeight(), GUILayout.ExpandHeight(true));
			DoList(rect);
		}

		public new void DoList(Rect a_rect)
		{
			rlist.DoList(a_rect);

			a_rect.y += rlist.GetHeight() - EditorGUIUtility.singleLineHeight;

			a_rect.width  -= 70;
			a_rect.height =  EditorGUIUtility.singleLineHeight;

			var temp = a_rect;

			temp.width = a_rect.width / 3;

			var toggleRect = temp;
			toggleRect.x      += 10;
			toggleRect.y      += 1;
			toggleRect.height -= 5;

			m_config.treatUniversalsAsActive
				= GUI.Toggle(toggleRect, m_config.treatUniversalsAsActive, "Make Active");

			temp.x += temp.width;
			if (GUI.Button(temp, "Save"))
			{
				Save();
			}

			temp.x += temp.width;
			if (GUI.Button(temp, "Reload"))
			{
				Load();
			}
		}

		public void Save()
		{
			var jsonString = EditorJsonUtility.ToJson(m_config);

			const string dir      = "Assets/Editor/SceneLayouts/Config/";
			string       fileName = $"{m_currentLayout.title}.asset";

			Directory.CreateDirectory(dir);

			var asset = new TextAsset(jsonString);
			AssetDatabase.CreateAsset(asset, dir + fileName);

			m_currentLayout.CacheSaveData();
			CacheSavedData();
		}

		public void Load()
		{
			const string dir      = "Assets/Editor/SceneLayouts/Config/";
			string       fileName = $"{m_currentLayout.title}.asset";

			var asset = AssetDatabase.LoadAssetAtPath<TextAsset>(dir + fileName);

			if (asset == null)
			{
				Save();
				return;
			}

			var config = new SceneLayoutConfig();
			EditorJsonUtility.FromJsonOverwrite(asset.text, config);

			m_config = config;

			m_currentLayout.sceneWrappers = m_config.universalScenes;

			rlist.list = m_currentLayout.sceneWrappers;

			m_currentLayout.CacheSaveData();
			CacheSavedData();
		}

		public void CacheSavedData()
		{
			m_config.cachedTreatUniversalAsActive = m_config.treatUniversalsAsActive;
		}
	}
}
