//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

namespace mulova.switcher
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using Object = UnityEngine.Object;

    public abstract class TabbedEditorWindow : EditorWindow {
		
		private TabData selectedTab;
		private List<TabData> tabs = new List<TabData>();
		private bool closable;

        public Color activeTabBgColor = Color.green;
        public Color activeTabContentColor = Color.white;
        public Color inactiveTabBgColor = Color.black;
        public Color inactiveTabContentColor = Color.green;
        public bool syncScroll;
        public virtual Object targetObject => Selection.activeObject;
        private SerializedObject _serializedObject;
        public SerializedObject serializedObject
        {
            get
            {
                if (_serializedObject?.targetObject == null)
                {
                    _serializedObject = null;
                }
                if (targetObject != null && (_serializedObject == null || _serializedObject.targetObject != targetObject))
                {
                    _serializedObject = new SerializedObject(targetObject);
                }
                return _serializedObject;
            }
        }

        protected EditorTab activeTab
        {
            get
            {
                return selectedTab?.tab;
            }
            set
            {
                foreach (var t in tabs)
                {
                    if (t.tab == value)
                    {
                        selectedTab = t;
                        break;
                    }
                }
            }
        }

        private bool isContextMenu
        {
            get
            {
                Event currentEvent = Event.current;
                return currentEvent.type == EventType.ContextClick;
            }
        }

        public int tabCount
        {
            get
            {
                return tabs.Count;
            }
        }

        protected virtual void OnEnable() {
			CreateTabs();
            if (showAllTab)
            {
                tabs.ForEach(t => t.tab.OnTabChange(true));
            } else
            {
			    string tabName = EditorPrefs.GetString(GetWindowId());
			    if (!string.IsNullOrEmpty(tabName)) {
				    foreach (TabData t in tabs) {
					    if (tabName == t.tab.ToString()) {
						    selectedTab = t;
						    break;
					    }
				    }

			    } else {
				    selectedTab = tabs[0];
			    }
			    selectedTab.tab.OnTabChange(true);
            }
            OnSelectionChange();
            #if UNITY_2017_1_OR_NEWER
            EditorApplication.playModeStateChanged += ChangePlayMode;
            #else
            EditorApplication.playmodeStateChanged += ChangePlaymode;
            #endif
			autoRepaintOnSceneChange = true;
		}

        protected virtual void OnDisable()
		{
			OnLostFocus();
			foreach (TabData t in tabs) {
				t.tab.OnDisable();
			}
		}

		protected void OnDestroy() {
			#if UNITY_2017_1_OR_NEWER
			EditorApplication.playModeStateChanged += ChangePlayMode;
			#else
			EditorApplication.playmodeStateChanged += ChangePlaymode;
			#endif
		}

		public string GetWindowId()
		{
			return string.Concat(GetType().FullName, "_", titleContent.text);
		}

		private bool showAllTab;
		public void ShowAllTab(bool show) {
			this.showAllTab = show;
			foreach (TabData t in tabs)
			{
				t.tab.OnTabChange(true);
			}
		}

		protected abstract void CreateTabs();

		protected virtual void OnSelectionChange() {
			Repaint();
            var so = targetObject != null? new SerializedObject(targetObject): null;
            if (showAllTab)
            {
                tabs.ForEach(t => t.tab.OnSelectionChange(so));
            } else
            {
			    selectedTab.tab.OnSelectionChange(so);
            }
        }

        private GenericMenu _contextMenu;
        public GenericMenu contextMenu
        {
            get
            {
                if (_contextMenu == null)
                {
                    _contextMenu = new GenericMenu();
                }
                return _contextMenu;
            }
        }

        protected virtual void OnHeaderGUI() { }

        protected void OnGUI() {
			try {
                _contextMenu = null;
                OnHeaderGUI();
				if (showAllTab) {
					EditorGUILayout.BeginHorizontal();
					for (int i=0; i<tabs.Count; ++i) {
						EditorGUILayout.BeginVertical();
						TabData t = tabs[i];
                        var tabName = t.tab.ToString();
                        if (!string.IsNullOrWhiteSpace(tabName))
                        {
                            GUILayout.Label(tabName, EditorStyles.boldLabel);
                        }
						t.tab.OnHeaderGUI();
						t.tab.ShowResult();
                        var pos = EditorGUILayout.BeginScrollView(t.scrollPos);
                        if (pos != t.scrollPos)
                        {
                            if (syncScroll)
                            {
                                foreach (var tab in tabs)
                                {
                                    tab.scrollPos = pos;
                                }
                            } else
                            {
                                t.scrollPos = pos;
                            }
                        }
						t.tab.OnInspectorGUI();
						EditorGUILayout.EndScrollView();
						t.tab.OnFooterGUI();
						EditorGUILayout.EndVertical();
                        if (i != tabs.Count-1)
                        {
                            EditorGUILayout.LabelField("", GUI.skin.verticalSlider, GUILayout.Width(10), GUILayout.Height(position.height));
                        }
                        if (isContextMenu)
                        {
                            selectedTab.tab.AddContextMenu();
                        }
                    }
                    EditorGUILayout.EndHorizontal();
				} else {
                    if (tabs.Count > 1)
                    {
                        Color bgColor = GUI.backgroundColor;
                        Color contentColor = GUI.contentColor;
                        EditorGUILayout.BeginHorizontal();
                        for (int i=0; i<tabs.Count; ++i) {
                            TabData t = tabs[i];
                            bool sel = t == selectedTab;
                            if (sel) {
                                GUI.backgroundColor = activeTabBgColor;
                                GUI.contentColor = activeTabContentColor;
                                GUILayout.Button(t.tab.ToString(), EditorStyles.toolbarButton);
                                if (closable && GUILayout.Button("x", EditorStyles.toolbarButton, GUILayout.Width(15))) {
                                    selectedTab.tab.OnDisable();
                                    selectedTab.tab.Remove();
                                    tabs.Remove(selectedTab);
                                    i = Math.Max(0, i-1);
                                    if (tabs.Count > 0) {
                                        selectedTab = tabs[i];
                                    } else {
                                        selectedTab = null;
                                    }
                                }
                            } else {
                                GUI.backgroundColor = inactiveTabBgColor;
                                GUI.contentColor = inactiveTabContentColor;
                                if (GUILayout.Button(t.tab.ToString(), EditorStyles.toolbarButton)) {
                                    selectedTab.tab.OnTabChange(false);
                                    selectedTab = t;
                                    selectedTab.tab.OnTabChange(true);
                                    sel = true;
                                    EditorPrefs.SetString(GetWindowId(), t.tab.ToString());
                                }
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                        GUI.backgroundColor = bgColor;
                        GUI.contentColor = contentColor;
                    }
					if (selectedTab != null) {
						selectedTab.tab.OnHeaderGUI();
						selectedTab.tab.ShowResult();
						selectedTab.scrollPos = EditorGUILayout.BeginScrollView(selectedTab.scrollPos);
						selectedTab.tab.OnInspectorGUI();
						EditorGUILayout.EndScrollView();
						selectedTab.tab.OnFooterGUI();
                        if (isContextMenu)
                        {
                            selectedTab.tab.AddContextMenu();
                        }
					}
				}

                if (isContextMenu)
                {
                    AddContextMenu();
                    _contextMenu.ShowAsContext();
                }
			} catch (Exception ex) {
                Debug.LogException(ex);
//				EditorUtility.DisplayDialog("Error", "Show error details in Editor log", "OK");
			}
		}

        protected virtual void AddContextMenu() { }

		protected void OnFocus() {
			sceneName = SceneName;
			foreach (TabData tab in tabs) {
				tab.tab.OnFocus(true);
			}
		}
		
		protected void OnLostFocus() {
			foreach (TabData tab in tabs) {
				tab.tab.OnFocus(false);
			}
		}
		
		protected void OnInspectorUpdate() {
			string newName = SceneName;
			if (sceneName != newName) {
				sceneName = newName;
				foreach (TabData tab in tabs) {
					tab.tab.OnChangeScene(sceneName);
				}
				Repaint();
			}
			foreach (TabData tab in tabs) {
				tab.tab.OnInspectorUpdate();
			}
		}

		private string sceneName = string.Empty;
		public string SceneName {
			get {
				if (Application.isPlaying) {
					return SceneManager.GetActiveScene().name;
				}
				return Path.GetFileNameWithoutExtension(SceneManager.GetActiveScene().path);
			}
		}
		
        protected void ChangePlayMode(PlayModeStateChange change) {
            foreach (TabData t in tabs) {
                t.tab.OnChangePlayMode(change);
            }
        }

		public void AddTab(params EditorTab[] tab) {
            var so = targetObject != null ? new SerializedObject(targetObject) : null;
			foreach (EditorTab t in tab) {
				tabs.Add(new TabData(t));
                try {
                    t.OnEnable();
                    if (showAllTab)
                    {
                        t.OnTabChange(true);
                    }
                    t.OnSelectionChange(so);
                } catch (Exception ex)
                {
                    Debug.LogError(ex.ToString());
                }
			}
			if (selectedTab == null) {
				selectedTab = tabs[0];
			}
		}

        public void RemoveTab(EditorTab tab)
        {
            int index = tabs.FindIndex(t => t.tab == tab);
            tabs.RemoveAt(index);
            if (selectedTab != null && selectedTab.tab == tab)
            {
                tab.OnTabChange(false);
                if (index > 0)
                {
                    selectedTab = tabs[index - 1];
                } else if (tabs.Count > 0)
                {
                    selectedTab = tabs[0];
                }
                selectedTab.tab.OnTabChange(true);
            }
            tab.OnDisable();
        }

        public EditorTab GetSelected() {
			return selectedTab.tab;
		}
		
		public void SetClosable(bool closable) {
			this.closable = closable;
		}

        public EditorTab GetTab(int i)
        {
            return i < tabs.Count? tabs[i].tab: null;
        }
		
		private class TabData {
			public EditorTab tab;
			public Vector2 scrollPos;
			
			public TabData(EditorTab t) {
				this.tab = t;
			}
		}
	}
}
