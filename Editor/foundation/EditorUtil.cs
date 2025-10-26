//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

namespace mulova.switcher.foundation
{
	using System;
	using UnityEditor;
	using System.IO;
	using System.Text;
	using UnityEngine;
	using System.Collections.Generic;
	using Object = UnityEngine.Object;
	using UnityEditor.SceneManagement;
	using UnityEngine.SceneManagement;

	public static class EditorUtil
	{
        public static SceneView sceneView {
            get {
                if (SceneView.currentDrawingSceneView != null)
                {
                    return SceneView.currentDrawingSceneView;
                }
                if (SceneView.lastActiveSceneView != null)
                {
                    return SceneView.lastActiveSceneView;
                }
                if (SceneView.sceneViews.Count > 0)
                {
                    return (SceneView)SceneView.sceneViews[0];
                }
                return null;
            }
        }

		public static void OpenExplorer(string path)
		{
			if (Application.platform == RuntimePlatform.OSXEditor) {
				EditorUtility.RevealInFinder(path);
			} else {
				System.Diagnostics.Process.Start("explorer.exe", "/select," + path.Replace(@"/", @"\"));
			}
		}

		public static string OpenFilePanel(string title, string ext)
		{
			string saved = EditorPrefs.GetString(title, "Assets/");
			string file = EditorUtility.OpenFilePanel(title, saved, ext);
			if (!string.IsNullOrEmpty(file)) {
				EditorPrefs.SetString(title, file);
			}
			return file;
		}

		public static string Encode(Stream stream, Encoding enc)
		{
			StreamReader reader = new StreamReader(stream, enc);
			string str = reader.ReadToEnd();
			reader.Close();
			return str;
		}

		public static bool DisplayProgressBar(string title, string info, float progress)
		{
			if (SystemInfo.graphicsDeviceID != 0) {
				return EditorUtility.DisplayCancelableProgressBar(title, info, progress);
			}
//            log.Debug("{0} ({1:P2})", info, progress);
			return false;
		}

        public static void SaveScene()
        {
            var currentScene = SceneManager.GetActiveScene().path;
            if (!string.IsNullOrEmpty(currentScene) && currentScene != "Untitled")
            {
                EditorSceneManager.SaveOpenScenes();
            }
        }

		public static List<GameObject> GetSceneRoots()
		{
			var list = new List<GameObject>();
			for (int i = 0; i < SceneManager.sceneCount; ++i) {
				list.AddRange(SceneManager.GetSceneAt(i).GetRootGameObjects());
			}
			return list;
		}

		public static void SetDirty(Object o)
		{
			if (Application.isPlaying || o == null) {
				return;
			}
			GameObject go = null;
			if (o is GameObject) {
				go = o as GameObject;
			} else if (o is Component) {
				go = (o as Component).gameObject;
			}
			if (go != null && go.scene.IsValid()) {
				EditorSceneManager.MarkSceneDirty(go.scene);
			} else {
				EditorUtility.SetDirty(o);
			}
		}

		public static void DisplayProgressBar<T>(string title, IList<T> list, Action<T> action)
		{
			try {
				for (int i = 0; i < list.Count; ++i) {
					if (EditorUtility.DisplayCancelableProgressBar(title, list[i].ToString(), i / (float)list.Count)) {
						return;
					}
					action(list[i]);
				}
			} finally {
				EditorUtility.ClearProgressBar();
			}
		}
	}
}


