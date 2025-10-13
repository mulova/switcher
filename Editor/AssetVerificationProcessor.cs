namespace mulova.switcher
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    
    public interface AssetVerificationProcess
    {
        bool VerifyAsset(string path);
        bool VerifyPrefab(string path);
    }
    
    public abstract class AssetVerificationProcessor : AssetModificationProcessor
    {
        private static List<AssetVerificationProcess> _processes;
    
        public static List<AssetVerificationProcess> processes
        {
            get
            {
                if (_processes == null)
                {
                    _processes = new List<AssetVerificationProcess>();
                }
    
                foreach (var t in typeof(AssetVerificationProcess).FindTypes())
                {
                    if (!t.IsAbstract)
                    {
                        _processes.Add((AssetVerificationProcess)Activator.CreateInstance(t));
                        
                    }
                }
                return _processes;
            }
        }
        
        static string[] OnWillSaveAssets(string[] paths)
        {
            var filtered = new List<string>();
            foreach (string path in paths)
            {
                var isValid = true;
                var stage = PrefabStageUtility.GetCurrentPrefabStage();
                var isPrefab = stage != null && stage.assetPath == path; 
                foreach (var p in processes)
                {
                    if ((isPrefab && !p.VerifyPrefab(path)) || (!isPrefab && !p.VerifyAsset(path)))
                    {
                        isValid = false;
                        break;
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