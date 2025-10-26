//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

using mulova.switcher.foundation;

namespace mulova.switcher
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    public class SwitcherCompareWindow : TabbedEditorWindow, IHasCustomMenu
    {
        private int compareCount = 2;
        internal HashSet<int> hideData { get; private set; } = new();
        public override UnityEngine.Object targetObject => Selection.activeGameObject != null ? Selection.activeGameObject.GetComponent<Switcher>() : null;

        protected override void CreateTabs()
        {
            titleContent.text = "Switcher";
            ShowAllTab(true);
            OnSelectionChange();
            if (SwitcherEditorConfig.instance.compareTabCount != 0)
            {
                compareCount = Mathf.Min(compareCount, SwitcherEditorConfig.instance.compareTabCount);
            }
            syncScroll = true;
        }

        private Switcher selected;
        protected override void OnSelectionChange()
        {
            base.OnSelectionChange();
            if (serializedObject != null)
            {
                selected = serializedObject.targetObject as Switcher;
                compareCount = selected.cases.Count;
            }
        }

        protected override void OnHeaderGUI()
        {
            if (selected != null)
            {
                compareCount = EditorGUILayout.IntSlider(compareCount, 2, selected.cases.Count);
                if (compareCount != tabCount)
                {
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
                }
            }
        }

        [MenuItem("Tools/Switcher/Switcher Window")]
        public static SwitcherCompareWindow Get()
        {
            // Get existing open window or if none, make a new one:
            SwitcherCompareWindow window = GetWindow<SwitcherCompareWindow>();
            window.Show();
            return window;
        }

        internal bool isAdd { get; private set; }
        public void AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Toggle Add"), isAdd, ToggleAdd);
            menu.AddItem(new GUIContent("Add Empty Data"), false, AddComponentData);
        }

        private void ToggleAdd()
        {
            isAdd = !isAdd;
        }

        private void AddComponentData()
        {
            if (selected == null)
            {
                return;
            }
            selected.cases.ForEach(c => c.data.Add(null));
        }
    }
}