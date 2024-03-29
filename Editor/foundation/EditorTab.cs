﻿//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

namespace mulova.switcher
{
	using System;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEditor.SceneManagement;
	using UnityEngine;
    using Object = UnityEngine.Object;

    public abstract class EditorTab {

        private object id;
        private string error = string.Empty;
        private string warning = string.Empty;
        private string info = string.Empty;
        protected TabbedEditorWindow window { get; private set; }
		protected Object targetObject => window.serializedObject?.targetObject;

        public EditorTab(TabbedEditorWindow window)
        {
            this.id = GetType().Name;
            this.window = window;
        }

        public EditorTab(object id, TabbedEditorWindow window) {
            this.id = id;
            this.window = window;
        }

		protected float GetWidth() {
			return position.width;
		}
		protected TabbedEditorWindow GetWindow() {
			return window;
		}

        protected GenericMenu contextMenu
        {
            get
            {
                return window.contextMenu;
            }
        }

        public abstract void OnEnable();
		public abstract void OnHeaderGUI();
		public abstract void OnInspectorGUI();
		public abstract void OnFooterGUI();
        public abstract void OnDisable();
		public virtual void Remove() { }
        public abstract void OnChangePlayMode(PlayModeStateChange stateChange);
		public abstract void OnChangeScene(string sceneName);
		public abstract void OnTabChange(bool sel);
		/// <summary>
		/// Called when the tab is activated
		/// </summary>
		public abstract void OnFocus(bool focus);
		public virtual void OnInspectorUpdate() {}
		public virtual void OnSelectionChange(SerializedObject so) {}
		
		protected virtual void Repaint() {
			window.Repaint();
		}
		
		protected Rect position {
			get {
				return window.position;
			}
		}

		protected string GetSceneName() {
			return window.SceneName;
		}
		
		protected void ClearResults() {
			error = string.Empty;
			warning = string.Empty;
			info = string.Empty;
		}
		
		protected void SetError(Exception ex) {
			if (ex == null) {
				SetError(string.Empty);
			} else {
				SetError(ex.Message);
			}
		}
		
		protected void SetError(string format, params object[] param) {
			if (param != null) {
				this.error = format;
			} else {
				this.error = string.Format(format, param);
			}
		}
		
		protected void SetWarning(string format, params object[] param) {
			if (param != null) {
				this.warning = format;
			} else {
				this.warning = string.Format(format, param);
			}
		}
		
		protected void SetInfo(string format, params object[] param) {
			if (param != null) {
				this.info = format;
			} else {
				this.info = string.Format(format, param);
			}
		}

        public virtual void AddContextMenu() { }

        public void ShowResult() {
			if (!string.IsNullOrEmpty(error)) {
				EditorGUILayout.HelpBox(error, MessageType.Error);
			}
			if (!string.IsNullOrEmpty(warning)) {
				EditorGUILayout.HelpBox(warning, MessageType.Warning);
			}
			if (!string.IsNullOrEmpty(info)) {
				EditorGUILayout.HelpBox(info, MessageType.Info);
			}
		}
        /*
        public bool DrawObjectList<T>(List<T> list) where T: class {
            return DrawObjectList(list, null, null, null, (ReorderableListFlags)0 );
        }

		public bool DrawObjectList<T>(List<T> list, ReorderableListFlags flags) where T: class {
			return DrawObjectList<T>(list, null, null, null, flags);
		}

        public bool DrawObjectList<T>(List<T> list, ObjListFilter<T> filter, ReorderableListFlags flags) where T: class {
			AndPredicate<T> predicate = filter.GetPredicate(list);
			return DrawObjectList<T>(list, predicate.Accept, null, null, flags);
		}

		/// <summary>
		/// Draws the object list.
		/// </summary>
		/// <returns><c>true</c>, if object list was drawn, <c>false</c> otherwise.</returns>
		/// <param name="list">List.</param>
		/// <param name="toString">default ToString is used if null</param>
		/// <param name="predicates">Predicates.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
        public bool DrawObjectList<T>(List<T> list, Predicate<T> filter, ConvToString toString, ConvToObject toObject, ReorderableListFlags flags) where T: class {
            ItemDrawer<T> itemDrawer = new ItemDrawer<T>();
            itemDrawer.toStr = toString; 
            itemDrawer.toObj = toObject;
            ListDrawer<T> drawer = new ListDrawer<T>(list, itemDrawer);
            drawer.Filter(filter);
            return drawer.Draw(flags);
		}
		*/

		/// <summary>
		/// Apply scene changes to prefab
		/// </summary>
		/// <param name="col">Col.</param>
		protected void SaveChange(IEnumerable<GameObject> col) {
			bool changed = false;
			HashSet<GameObject> saved = new HashSet<GameObject>();
			foreach (GameObject o in col) {
                if (PrefabUtility.IsPartOfAnyPrefab(o))
                {
                    GameObject objRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(o);
                    if (objRoot != null && !saved.Contains(objRoot) && PrefabUtility.IsAnyPrefabInstanceRoot(objRoot)) {
                        saved.Add(objRoot);
                        var path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(objRoot);
                        PrefabUtility.SaveAsPrefabAsset(objRoot, path, out var result);
                        if (result)
                        {
                            changed = true;
                        }
                    }
                }
			}
			if (changed) {
				EditorSceneManager.SaveOpenScenes();
				AssetDatabase.SaveAssets();
			}
		}

        public override string ToString()
        {
            return id.ToString();
        }
    }
}


