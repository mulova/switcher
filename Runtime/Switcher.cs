//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

namespace mulova.switcher
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.Events;
    using Assert = UnityEngine.Assertions.Assert;

    public class Switcher : MonoBehaviour
    {
        public List<Case> cases = new List<Case>();
        [SerializeField, HideInInspector] public List<SwitchPreset> preset = new List<SwitchPreset>();
        [HideInInspector, SerializeField, EnumType] private string enumType = "";
        [HideInInspector] public bool caseSensitive = true;

        private HashSet<string> keySet = new HashSet<string>();
        public IReadOnlyCollection<string> keys => keySet;
        public List<string> allKeys => cases.ConvertAll(s => s.name);
        public bool showAction => cases.Exists(c => c.showAction || c.hasAction);
        public bool hasAction => cases.Exists(s => s.hasAction);
        public bool showData { get; set; } = false; // editor only. needs SerializeReferenceExtensions
        public bool showMisc { get; set; } = false;
        public string enumTypeName => enumType;

        private ILogger log => Debug.unityLogger;

        public void ResetKeys()
        {
            keySet.Clear();
        }

        public bool Contains(object o)
        {
            string s = NormalizeKey(o);
            return keySet.Contains(s);
        }

        public bool IsKeys(params object[] keys)
        {
            var count = keys?.Length ?? 0;
            if (count == keySet.Count)
            {
                if (keys != null)
                {
                    foreach (object o in keys)
                    {
                        if (!Contains(o))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            return false;
        }

        public bool IsPreset(string s)
        {
            var p = preset.Find(i => i.presetName == s);
            if (p == null)
            {
                return false;
            }
            return IsKeys(p.keys);
        }

        public void SetPreset(object key)
        {
            var k = NormalizeKey(key);
            foreach (var p in preset)
            {
                if (p.presetName == k)
                {
                    Apply(p.keys);
                    break;
                }
            }
        }

        public void Collect(string caseKey, bool changedOnly)
        {
            var c = cases.Find(s => s.name == caseKey);
            foreach (var d in c.data)
            {
                d.Collect(d.target, changedOnly);
            }
        }

        public void AddKey(params object[] keys)
        {
            foreach (object o in keys)
            {
                keySet.Add(NormalizeKey(o));
            }
        }

        public void RemoveKeys(params object[] keys)
        {
            foreach (object o in keys)
            {
                keySet.Remove(NormalizeKey(o));
            }
        }

        public void ToggleKey(object key)
        {
            string k = NormalizeKey(key);
            if (keySet.Contains(k))
            {
                keySet.Remove(k);
            }
            else
            {
                keySet.Add(k);
            }
        }

        public void SetAction(object key, UnityAction<string> action)
        {
            string k = NormalizeKey(key);
            Case c = cases.Find(e => e.name.Equals(k, StringComparison.OrdinalIgnoreCase));
            if (c.isValid)
            {
                c.action.RemoveAllListeners();
                c.action.AddListener(action);
            }
            else
            {
                Assert.IsTrue(false, $"Key {k} not found");
            }
        }

        public void AddAction(object key, UnityAction<string> action)
        {
            string k = NormalizeKey(key);
            Case s = cases.Find(e => e.name.Equals(k, StringComparison.OrdinalIgnoreCase));
            if (s.isValid)
            {
                s.action.AddListener(action);
            }
            else
            {
                Assert.IsTrue(false, $"Key {k} not found");
            }
        }

        public void Merge(Switcher merged)
        {
            // Replace components
            foreach (var c in merged.cases)
            {
                var clone = c.Clone() as Case;
                for (int i = 0; i < clone.data.Count; ++i)
                {
                    var matching = GetMatchingSibling(merged.transform, clone.data[i].target.transform, transform);
                    clone.data[i].target = GetMatchingComponent(clone.data[i].target, matching);
                }
                cases.Add(clone);
            }
        }

        private Component GetMatchingComponent(Component src, Transform dst)
        {
            var srcComps = src.GetComponents<Component>();
            var index = Array.IndexOf(srcComps, src);
            var dstComps = dst.GetComponents<Component>();
            return dstComps[index];
        }

        private Transform GetMatchingSibling(Transform srcRoot, Transform src, Transform dstRoot)
        {
            var hierarchy = new List<int>();
            while (src != srcRoot)
            {
                hierarchy.Add(src.GetSiblingIndex());
                src = src.parent;
            }
            hierarchy.Reverse();
            var trans = dstRoot;
            foreach (var index in hierarchy)
            {
                trans = trans.GetChild(index);
            }
            return trans;
        }

        private string NormalizeKey(object o)
        {
            var key = o is int ? cases[(int)o].name : o.ToString();
            return caseSensitive ? key : key.ToLower();
        }

        public void Clear()
        {
            cases.Clear();
            keySet.Clear();
        }

        /// <summary>
        /// </summary>
        /// <param name="keysOrIndex">can be key (string) or index (int) </param>
        public void Apply(params object[] keysOrIndex)
        {
            if (IsKeys(keysOrIndex))
            {
                //log.Debug("Already set");
                return;
            }
            ResetKeys();
            AddKey(keysOrIndex);
            Apply();
        }

        public void Apply()
        {
            int match = 0;
            foreach (Case c in cases)
            {
                if (keySet.Contains(NormalizeKey(c.name)))
                {
                    match++;
                    try
                    {
                        foreach (var d in c.data)
                        {
#if UNITY_EDITOR
                            if (!Application.isPlaying)
                            {
                                UnityEditor.Undo.RecordObject(d.target, d.target.name);
                            }
#endif
                            try
                            {
                                d.ApplyTo(d.target);
                            } catch (Exception ex)
                            {
                                log.LogException(ex, this);
                            }
#if UNITY_EDITOR
                            if (!Application.isPlaying)
                            {
                                UnityEditor.EditorUtility.SetDirty(d.target);
                            }
#endif
                        }
                        c.action?.Invoke(c.name);
                    }
                    catch (Exception ex)
                    {
                        log.LogFormat(LogType.Error, this, "{0}({1})\n{2}", c.name, name, ex);
                    }
                }
            }

            //if (log.IsLoggable(LogType.Log))
            //{
            //    log.Debug("Switcher {0}", keySet.Join(","));
            //}
            if (match != keySet.Count)
            {
                log.LogFormat(LogType.Error, this, "Missing key exists among {0}", string.Join('.', keySet));
            }
        }

        public IDisposable NewScope(object startKey, object endKey)
        {
            Apply(startKey);
            return new SwitcherDisposer(this, endKey);
        }

#if UNITY_EDITOR
        [ContextMenu("Spread out")]
        public void SpreadOut()
        {
            SpreadOut(null, true);
        }

        public void SpreadOut(List<RootData> rootData, bool activate)
        {
            Apply(cases[0].name);
            for (int i = 1; i < cases.Count; ++i)
            {
                var name = cases[i].name;
                var clone = Instantiate(this, transform.parent);
                clone.name = name;
                clone.transform.SetSiblingIndex(transform.GetSiblingIndex() + i);
                clone.Apply(name);
                clone.gameObject.SetActive(activate);
                UnityEditor.Undo.RegisterCreatedObjectUndo(clone.gameObject, name);
                if (rootData != null && rootData.Count > i - 1)
                {
                    rootData[i - 1].ApplyTo(clone.transform);
                }
            }
        }

        [ContextMenu("Toggle Misc")]
        private void ToggleMisc()
        {
            showMisc = !showMisc;
        }

        private void OnValidate()
        {
            if (!string.IsNullOrWhiteSpace(enumType))
            {
                var type = TypeEx.GetType(enumType);
                if (type != null)
                {
                    var keys = allKeys;
                    var all = Enum.GetNames(type);
                    if (keys.Count != all.Length)
                    {
                        Debug.LogError($"Switcher keys missing ({enumType})", this);
                    } else
                    {
                        var intersect = keys.Intersect(all);
                        if (keys.Count != intersect.Count())
                        {
                            Debug.LogError($"Switcher keys mismatch ({enumType})", this);
                        } else
                        {
                            SetEnumType(type);
                        }
                    }
                }
            }
        }

        public void SetEnumType(Type type)
        {
            enumType = type.FullName;

            var keys = Enum.GetNames(type);
            if (cases.Count != keys.Length)
            {
                Debug.LogError($"Case count must be {keys.Length} but {cases.Count}", this);
            }
            else
            {
                for (int i=0; i<keys.Length; ++i)
                {
                    cases[i].name = keys[i].ToString();
                }
            }
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }

    public class RootData
    {
        public int siblingIndex;
        public bool rectTransform;
        public Vector2 anchorPosition;
        public Vector3 position;

        public RootData(Transform t)
        {
            rectTransform = t is RectTransform;
            siblingIndex = t.GetSiblingIndex();
            if (rectTransform)
            {
                anchorPosition = ((RectTransform)t).anchoredPosition;
            }
            else
            {
                position = t.localPosition;
            }
        }

        public void ApplyTo(Transform t)
        {
            if (rectTransform)
            {
                ((RectTransform)t).anchoredPosition = anchorPosition;
            }
            else
            {
                t.localPosition = position;
            }
            t.SetSiblingIndex(siblingIndex);
        }
    }
}