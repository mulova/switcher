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

        private HashSet<int> applySet = new();
        private Dictionary<string, int> indexDic = new();
        public List<string> allKeys => cases.ConvertAll(s => s.name);
        public bool showAction => cases.Exists(c => c.showAction || c.hasAction);
        public bool hasAction => cases.Exists(s => s.hasAction);
        public bool showData { get; set; } = false; // editor only. needs SerializeReferenceExtensions
        public bool showMisc { get; set; } = false;
        public string enumTypeName => enumType;

        public Case this[int i] => cases[i];

        private ILogger log => Debug.unityLogger;

        public void Refresh()
        {
            indexDic.Clear();
            for (int i = 0; i < cases.Count; ++i)
            {
                indexDic[cases[i].name] = i;
            }
        }

        public Case GetCase(object key) => cases[NormalizeKey(key)];

        public bool Contains(object o) => Contains(NormalizeKey(o));

        public bool Contains(int i) => i >= 0 && i < cases.Count && applySet.Contains(i);

        public bool IsKey(object key) => applySet.Count == 1 && applySet.Contains(NormalizeKey(key));
        public bool IsKeys(params object[] keys) => IsKeys((IReadOnlyList<object>)keys);
        public bool IsKeys(IReadOnlyList<object> keys)
        {
            var count = keys?.Count ?? 0;
            if (count == applySet.Count)
            {
                if (keys != null)
                {
                    foreach (var k in keys)
                    {
                        if (!Contains(k))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            return false;
        }

        public bool IsIndex(int i) => applySet.Count == 1 && applySet.Contains(i);
        public bool IsIndex(params int[] index) => IsIndex((IReadOnlyList<int>)index);
        public bool IsIndex(IReadOnlyList<int> index)
        {
            var count = index?.Count ?? 0;
            if (count == applySet.Count)
            {
                if (index != null)
                {
                    foreach (var i in index)
                    {
                        if (!Contains(i))
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
            var k = key.ToString();
            foreach (var p in preset)
            {
                if (p.presetName == k)
                {
                    Apply(p.keys);
                    break;
                }
            }
        }

        public void Collect(object key, bool changedOnly) => Collect(NormalizeKey(key), changedOnly);

        public void Collect(int i, bool changedOnly)
        {
            foreach (var d in cases[i].data)
            {
                d.Collect(d.target, changedOnly);
            }
        }

        public void AddIndex(int i) => applySet.Add(i);
        public void AddIndex(params int[] index) => applySet.AddAll(index);
        public void AddIndex(IEnumerable<int> index) => applySet.AddAll(index);
        public void AddKey(object key) => applySet.Add(NormalizeKey(key));
        public void AddKeys(params object[] keys) => AddKeys((IEnumerable<object>)keys);
        public void AddKeys(IEnumerable<object> keys)
        {
            foreach (object o in keys)
            {
                applySet.Add(NormalizeKey(o));
            }
        }

        public void RemoveIndex(int i) => applySet.Remove(i);
        public void RemoveIndex(IEnumerable<int> index) => applySet.RemoveAll(index);
        public void RemoveIndex(params int[] index) => applySet.RemoveAll(index);
        public void RemoveKey(object key) => applySet.Remove(NormalizeKey(key));
        public void RemoveKeys(params object[] keys) => AddKeys((IEnumerable<object>)keys);
        public void RemoveKeys(IEnumerable<object> keys)
        {
            foreach (object o in keys)
            {
                applySet.Remove(NormalizeKey(o));
            }
        }

        public void ToggleKey(object key) => ToggleIndex(NormalizeKey(key));

        public void ToggleIndex(int i)
        {
            if (applySet.Contains(i))
            {
                applySet.Remove(i);
            }
            else
            {
                applySet.Add(i);
            }
        }

        public void ClearAction(object key) => ClearAction(NormalizeKey(key));

        public void ClearAction(int i)
        {
            cases[i].action.RemoveAllListeners();
        }

        public void AddAction(object key, UnityAction<string> action) => AddAction(NormalizeKey(key), action);

        public void AddAction(int i, UnityAction<string> action)
        {
            Case s = cases[i];
            if (s.isValid)
            {
                s.action.AddListener(action);
            }
            else
            {
                Assert.IsTrue(false, $"Key {i} not found");
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

        private int NormalizeKey(object o)
        {
            if (indexDic.Count == 0)
            {
                Refresh();
            }
            var key = caseSensitive ? o.ToString() : o.ToString().ToLower();
            if (indexDic.TryGetValue(key, out var i))
            {
                return i;
            }
            else
            {
                log.LogFormat(LogType.Error, this, "Missing key {0}", o);
                return -1;
            }
        }

        public void Clear()
        {
            cases.Clear();
            applySet.Clear();
        }

        public void Apply(params object[] index) => Apply((IReadOnlyList<object>)index);
        public void Apply(IReadOnlyList<object> keys)
        {
            if (IsKeys(keys))
            {
                //log.Debug("Already set");
                return;
            }
            applySet.Clear();
            AddKeys(keys);
            Apply();
        }

        public void Apply(object key)
        {
            if (IsKey(key))
            {
                //log.Debug("Already set");
                return;
            }
            applySet.Clear();
            AddKey(key);
            Apply();
        }

        public void Apply(int index)
        {
            if (IsIndex(index))
            {
                //log.Debug("Already set");
                return;
            }
            applySet.Clear();
            AddIndex(index);
            Apply();
        }

        public void Apply(params int[] index) => Apply((IReadOnlyList<int>)index);
        public void Apply(IReadOnlyList<int> index)
        {
            if (IsIndex(index))
            {
                //log.Debug("Already set");
                return;
            }
            applySet.Clear();
            AddIndex(index);
            Apply();
        }

        public void Apply()
        {
            int match = 0;
            foreach (Case c in cases)
            {
                if (applySet.Contains(NormalizeKey(c.name)))
                {
                    match++;
                    foreach (var d in c.data)
                    {
                        if (d.target == null)
                        {
                            log.LogFormat(LogType.Warning, this, "[{0}] Case '{1}' target is missing", name, c.name);
                            continue;
                        }
                        try
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
                            }
                            catch (Exception ex)
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
                        catch (Exception ex)
                        {
                            log.LogFormat(LogType.Error, this, "[{0}] Case {1}\n{2}", name, c.name, ex);
                        }
                    }
                    c.action?.Invoke(c.name);
                }
            }

            //if (log.IsLoggable(LogType.Log))
            //{
            //    log.Debug("Switcher {0}", keySet.Join(","));
            //}
            if (match != applySet.Count)
            {
                log.LogFormat(LogType.Error, this, "Missing key exists among {0}", string.Join('.', applySet));
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