//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

using System.IO;
using UnityEditor;
using UnityEngine;

namespace mulova.switcher
{
    public class SwitcherEditorConfig : ScriptableObject
    {
        public const string PATH = "Assets/Editor/switcher_config.asset";

        [Tooltip("Delete other cases except the main case when generating switcher")]
        public bool deleteCases = true;
        [Tooltip("bypass extracting differences from the nested switcher GameObject and it's children")]
        public bool bypassNestedSwitcherTree = false;
        [Tooltip("record nested switcher cases")]
        public bool recordNestedSwitcherData = false;
        [Tooltip("Default Tab Count for CompareTab")]
        public int compareTabCount = 0;
        [Tooltip("Log type for the error")]
        public LogType exceptionLogType = LogType.Error;

        private static SwitcherEditorConfig _instance;
        public static SwitcherEditorConfig instance
        {
            get
            {
                if (_instance == null)
                {
                    Reload();
                }
                return _instance;
            }
        }

        public static void Reload()
        {
            _instance = AssetDatabase.LoadAssetAtPath<SwitcherEditorConfig>(PATH);
            if (_instance == null)
            {
                _instance = CreateInstance<SwitcherEditorConfig>();
                var dir = new DirectoryInfo(Path.GetDirectoryName(PATH));
                if (!dir.Exists)
                {
                    dir.Create();
                }
                AssetDatabase.CreateAsset(_instance, PATH);
            }
        }
    }
}