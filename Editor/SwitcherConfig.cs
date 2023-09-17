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
    public class SwitcherConfig : ScriptableObject
    {
        public const string PATH = "Assets/Editor/switcher_config.asset";

        [Tooltip("Delete other cases except the main case when generating switcher")]
        public bool deleteCases = true;
        [Tooltip("Don't extract diff from the RectTransfrom driven from other script")]
        public bool ignoreDrivenRectTransform = true;

        private static SwitcherConfig _instance;
        public static SwitcherConfig instance
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
            _instance = AssetDatabase.LoadAssetAtPath<SwitcherConfig>(PATH);
            if (_instance == null)
            {
                _instance = CreateInstance<SwitcherConfig>();
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