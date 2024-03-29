using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace mulova.switcher
{
    public class SwitcherAssetModificationProcessor : AssetModificationProcessor
    {
        static string[] OnWillSaveAssets(string[] paths)
        {
            var filtered = new List<string>();
            foreach (string path in paths)
            {
                var isValid = true;
                var stage = PrefabStageUtility.GetCurrentPrefabStage();
                if (stage != null && stage.assetPath == path)
                {
                    var o = stage.prefabContentsRoot;
                    foreach (var s in o.GetComponentsInChildren<Switcher>())
                    {
                        if (!s.IsValid())
                        {
                            isValid = false;
                            Debug.LogError($"{path} Switcher('{s.name}')has some targets missing");
                            break;
                        }
                    }
                }
                if (isValid)
                {
                    filtered.Add(path);
                }
            }
            return filtered.ToArray();
        }
    }
}