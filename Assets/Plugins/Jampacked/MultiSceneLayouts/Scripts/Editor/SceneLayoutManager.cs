using System;
using System.Collections.Generic;
using System.IO;

using UnityEditor;
using UnityEditor.SceneManagement;

using UnityEngine;

namespace Jampacked.ProjectInca.Editor.SceneLayoutManager
{
	public class SceneLayoutManager : EditorWindow
	{
		private const string SCENCE_LAYOUTS_DIR = "Assets/Editor/SceneLayouts/";

		//private readonly SceneLayoutConfig m_config = new SceneLayoutConfig();

		private MasterList m_masterList;

		//private SubList m_universalSubList;
		private UniversalSubList m_universalSubList;

		private Vector2 m_mainScrollPosition;

		// TODO [Myles]: Make this save to a config file along with order of layouts
		private bool m_autosave = false;

		public bool Autosave
		{
			get { return m_autosave; }
		}

		private bool m_saveAll = false;

		public bool SaveAll
		{
			get { return m_saveAll; }
		}

		[MenuItem("Tools/Jampacked/Scene Layout Manager", priority = 1001)]
		private static void Init()
		{
			var window = GetWindow<SceneLayoutManager>();
			window.Show();
			window.titleContent = new GUIContent("Scene Layout Manager");
		}

		private void OnEnable()
		{
			if (m_masterList == null)
			{
				m_masterList = new MasterList(this);
			}

			if (m_universalSubList == null)
			{
				m_universalSubList = new UniversalSubList(
					this,
					new SceneLayout(this)
					{
						readOnly = true,
						title    = "UniversalLayouts",
					}
				);

				m_universalSubList.Load();
			}

			TryLoadLayoutsFromFile();
		}

		private void OnGUI()
		{
			GUILayout.BeginHorizontal();
			{
				if (GUILayout.Button("Refresh"))
				{
					TryLoadLayoutsFromFile();
				}

				m_saveAll = GUILayout.Button("Save All");

				m_autosave = GUILayout.Toggle(m_autosave, "Auto-Save");

				if (GUILayout.Button("Prune Files")
				    && EditorUtility.DisplayDialog(
					    "Prune Unreferenced Layout Files?",
					    "Are you sure you would like to prune all unreferenced Scene Layout files? This action cannot be undone.",
					    "Prune",
					    "Cancel"
				    ))
				{
					PruneUnreferencedLayoutFiles();
				}
			}
			GUILayout.EndHorizontal();

			GUILayout.Space(EditorGUIUtility.singleLineHeight);

			m_mainScrollPosition =
				GUILayout.BeginScrollView(m_mainScrollPosition);
			{
				m_universalSubList.DoLayoutList();

				if ((m_autosave || m_saveAll) && m_universalSubList.IsDirty)
				{
					m_universalSubList.Save();
				}

				GUILayout.Space(EditorGUIUtility.singleLineHeight);

				m_masterList.Draw();

				m_saveAll = false;
			}
			GUILayout.EndScrollView();
		}

		public bool IsLayoutConflicting(in SceneLayout a_layout)
		{
			var list = m_masterList.masterIList;

			for (int i = 0; i < list.Count; i++)
			{
				var element = list[i].CurrentLayout;
				if (element          != a_layout
				    && element.title == a_layout.title)
				{
					return true;
				}
			}

			return false;
		}

		private void TryLoadLayoutsFromFile()
		{
			if (!Directory.Exists(SCENCE_LAYOUTS_DIR))
			{
				return;
			}

			m_universalSubList.Load();

			var fileNames = Directory.GetFiles(SCENCE_LAYOUTS_DIR);

			var list = m_masterList.masterIList;

			foreach (var fileName in fileNames)
			{
				var asset = AssetDatabase.LoadMainAssetAtPath(fileName) as TextAsset;
				if (asset != null)
				{
					int index = list.FindIndex(a_subList => a_subList.CurrentLayout.IsSameAsset(asset));

					var layout = new SceneLayout(this, asset);

					if (index == -1)
					{
						list.Add(new SubList(this, layout));
					} else
					{
						list[index] = new SubList(this, layout);
					}
				}
			}
		}

		private void PruneUnreferencedLayoutFiles()
		{
			if (!Directory.Exists(SCENCE_LAYOUTS_DIR))
			{
				return;
			}

			var fileNames = Directory.GetFiles(SCENCE_LAYOUTS_DIR);

			var list = m_masterList.masterIList;

			foreach (var fileName in fileNames)
			{
				var asset = AssetDatabase.LoadMainAssetAtPath(fileName) as TextAsset;
				if (asset != null)
				{
					string pathWithoutExtension = Path.GetFileNameWithoutExtension(fileName);

					int index = list.FindIndex(a_subList => a_subList.CurrentLayout.title == pathWithoutExtension);

					if (index == -1)
					{
						AssetDatabase.DeleteAsset(fileName);
					}
				}
			}
		}

		public void ApplyLayout(SceneLayout a_layout)
		{
			var sceneWrappers = m_universalSubList.CurrentLayout.sceneWrappers;

			bool doSetActive = m_universalSubList.TreatUniversalAsActive;

			var loadedScenes = 0;
			for (var i = 0; i < sceneWrappers.Count; i++)
			{
				var sceneAsset = sceneWrappers[i];

				var path = AssetDatabase.GetAssetPath(sceneAsset.scene);

				var mode = loadedScenes == 0 ? OpenSceneMode.Single : OpenSceneMode.Additive;

				var scene = EditorSceneManager.OpenScene(path, mode);

				int numRoomRoots = 0;

				foreach (var go in scene.GetRootGameObjects())
				{
					if (go.CompareTag("SceneRoot"))
					{
						go.transform.position = sceneAsset.position;
						go.transform.rotation = Quaternion.Euler(sceneAsset.rotation);
						numRoomRoots++;
					}

					if (numRoomRoots > 1)
					{
						Debug.LogWarning(
							$"Scene \"{scene.name}\" has more than one object tagged \"SceneRoot\", "
							+ "scenes should only have 1!"
						);
					}
				}

				loadedScenes++;
			}

			a_layout.Apply(!doSetActive, loadedScenes > 0);
		}
	}
}
