using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEditor;
using UnityEditor.SceneManagement;

using UnityEditorInternal;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Jampacked.ProjectInca.Editor.SceneLayoutManager
{
	[Serializable]
	public class SceneAssetWrapper
	{
		public Vector3 position = new Vector3();
		public Vector3 rotation = new Vector3();

		[NonSerialized]
		public Vector3 cachedPosition = new Vector3();

		[NonSerialized]
		public Vector3 cachedRotation = new Vector3();

		public SceneAsset scene = new UnityEngine.Object() as SceneAsset;
	}

	// TODO [Myles]: Convert to Scriptable Object
	public class SceneLayout
	{
		[NonSerialized]
		public string title;

		[NonSerialized]
		public bool readOnly = false;

		public List<SceneAssetWrapper> sceneWrappers = new List<SceneAssetWrapper>();

		private TextAsset m_fileHandle;

		private readonly SceneLayoutManager m_sceneLayoutManager;

		private int m_cachedLength;

		private string m_cachedTitle = string.Empty;

		public bool HasFileHandle
		{
			get { return m_fileHandle != null; }
		}

		private bool m_dirty;

		public bool IsDirty
		{
			get
			{
				return m_dirty
				       || m_cachedLength != sceneWrappers.Count
				       || m_cachedTitle  != title
				       || sceneWrappers.Any(a_wrapper => a_wrapper.position != a_wrapper.cachedPosition);
			}
		}

		public void SetDirty()
		{
			m_dirty = true;
		}

		public bool IsSameAsset(TextAsset a_asset)
		{
			string currPath = AssetDatabase.GetAssetPath(m_fileHandle);
			string newPath  = AssetDatabase.GetAssetPath(a_asset);

			return currPath == newPath;
		}

		public SceneLayout()
		{
		}

		public SceneLayout(SceneLayoutManager a_window)
		{
			m_sceneLayoutManager = a_window;
		}

		public SceneLayout(SceneLayoutManager a_window, TextAsset a_file)
		{
			m_sceneLayoutManager = a_window;

			if (a_file != null)
			{
				m_fileHandle = a_file;

				Load();
			}
		}

		public void Apply(bool a_doSetActive, bool a_isSceneLoaded)
		{
			if (sceneWrappers.Count <= 0)
			{
				return;
			}

			for (var i = 0; i < sceneWrappers.Count; i++)
			{
				var sceneAsset = sceneWrappers[i];

				var path = AssetDatabase.GetAssetPath(sceneAsset.scene);

				var mode = !a_isSceneLoaded ? OpenSceneMode.Single : OpenSceneMode.Additive;

				var scene = EditorSceneManager.OpenScene(path, mode);

				if (a_doSetActive && !a_isSceneLoaded)
				{
					SceneManager.SetActiveScene(scene);
				}

				a_isSceneLoaded = true;

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
			}
		}

		public void Save()
		{
			var jsonString = EditorJsonUtility.ToJson(this);

			if (!m_sceneLayoutManager.IsLayoutConflicting(this))
			{
				if (m_fileHandle == null)
				{
					const string dir      = "Assets/Editor/SceneLayouts/";
					string       fileName = $"{title}.asset";

					Directory.CreateDirectory(dir);

					m_fileHandle = new TextAsset(jsonString);
					AssetDatabase.CreateAsset(m_fileHandle, dir + fileName);
				} else
				{
					var path = AssetDatabase.GetAssetPath(m_fileHandle);
					m_fileHandle = new TextAsset(jsonString);

					AssetDatabase.CreateAsset(m_fileHandle, path);

					// TODO [Myles]: Optimize
					UpdateObjectHandle();
				}
			} else
			{
				Debug.LogError($"Invalid SceneLayout title \"{title}\"!");
			}

			CacheSaveData();
		}

		public void Load()
		{
			if (m_fileHandle != null)
			{
				string filename = AssetDatabase.GetAssetPath(m_fileHandle);
				filename = Path.GetFileNameWithoutExtension(filename);

				title = filename;

				var layout = new SceneLayout();
				EditorJsonUtility.FromJsonOverwrite(m_fileHandle.text, layout);

				sceneWrappers = layout.sceneWrappers;

				CacheSaveData();
			}
		}

		public void UpdateObjectHandle()
		{
			var newPath = $"{title}.asset";

			if (m_fileHandle != null)
			{
				var path = AssetDatabase.GetAssetPath(m_fileHandle);
				var e    = AssetDatabase.RenameAsset(path, newPath);
				if (e.Length > 0)
				{
					Debug.LogError(e);
				}
			}
		}

		public void CacheSaveData()
		{
			m_dirty        = false;
			m_cachedLength = sceneWrappers.Count;
			m_cachedTitle  = title;

			foreach (var wrapper in sceneWrappers)
			{
				wrapper.cachedPosition = wrapper.position;
				wrapper.cachedRotation = wrapper.rotation;
			}
		}
	}
}
