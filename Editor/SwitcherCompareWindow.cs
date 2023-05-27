//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

namespace mulova.switcher
{
    using UnityEditor;
    using UnityEngine;

    public class SwitcherCompareWindow : TabbedEditorWindow
    {
        private int compareCount = 2;

        protected override void CreateTabs()
        {
            titleContent.text = "Switcher";
            ShowAllTab(true);
            AddTab(new SwitcherCompareTab(0, this));
            AddTab(new SwitcherCompareTab(1, this));
            syncScroll = true;
        }

        protected override void OnHeaderGUI()
        {
            if (Selection.activeGameObject == null)
            {
                return;
            }
            var s = Selection.activeGameObject.GetComponent<Switcher>();
            var max = s?.switches.Count ?? 2;
            if (max > 2)
            {
                var count = EditorGUILayout.IntSlider(compareCount, 2, max);
                if (compareCount != count)
                {
                    compareCount = count;
                    // Add tabs
                    for (int i=tabCount; i<compareCount; ++i)
                    {
                        AddTab(new SwitcherCompareTab(i, this));
                    }
                    // Remove tabs
                    for (int i=tabCount-1; i >= compareCount; --i)
                    {
                        RemoveTab(GetTab(i));
                    }
                    OnSelectionChange();
                }
            }
        }

        [MenuItem("Tools/Switcher/Switcher Window")]
        public static SwitcherCompareWindow Get()
        {
            // Get existing open window or if none, make a new one:
            SwitcherCompareWindow window = CreateWindow<SwitcherCompareWindow>();
            window.titleContent = new GUIContent("Compare");
            window.Show();
            return window;
        }
    }
}