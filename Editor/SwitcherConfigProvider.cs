//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace mulova.switcher
{
    class SwitcherConfigProvider : SettingsProvider
    {
        private SerializedObject config;

        public SwitcherConfigProvider(string path, SettingsScope scope = SettingsScope.User)
            : base(path, scope) { }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            // This function is called when the user clicks on the Replay element in the Settings window.
            config = new SerializedObject(SwitcherEditorConfig.instance);
        }

        public override void OnGUI(string searchContext)
        {
            if (GUILayout.Button("Go to config"))
            {
                Selection.activeObject = AssetDatabase.LoadAssetAtPath<SwitcherEditorConfig>(SwitcherEditorConfig.PATH);
            }
        }

        // Register the SettingsProvider
        [SettingsProvider]
        public static SettingsProvider CreateReplaySettingsProvider()
        {
            var provider = new SwitcherConfigProvider("Project/Switcher", SettingsScope.Project);

            // Automatically extract all keywords from the Styles.
            provider.keywords = new[] { "switcher", "ugui", "config" };
            return provider;
        }
    }
}