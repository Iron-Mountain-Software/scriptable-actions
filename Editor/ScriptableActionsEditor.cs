using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace IronMountain.ScriptableActions.Editor
{
    public class ScriptableActionsEditor
    {
        public static readonly List<Type> ScriptableActionTypes;
        public static readonly Dictionary<string, bool> CollapsedDrawers;

        private static readonly Texture2D HeaderTexture;
        private static readonly Texture2D EvenTexture;
        private static readonly Texture2D OddTexture;
        private static readonly Texture2D ErrorTexture;
        
        private static readonly GUIStyle Header;
        private static readonly GUIStyle H1;
        private static readonly GUIStyle Even;
        private static readonly GUIStyle Odd;
        private static readonly GUIStyle Error;

        static ScriptableActionsEditor()
        {
            ScriptableActionTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsSubclassOf(typeof(ScriptableAction)))
                .ToList();
            CollapsedDrawers = new Dictionary<string, bool>();
            HeaderTexture = new Texture2D(1, 1);
            HeaderTexture.SetPixel(0,0, new Color(0.12f, 0.12f, 0.12f));
            HeaderTexture.Apply();
            EvenTexture = new Texture2D(1, 1);
            EvenTexture.SetPixel(0,0, new Color(1f, 1f, 1f, .05f));
            EvenTexture.Apply();
            OddTexture = new Texture2D(1, 1);
            OddTexture.SetPixel(0,0, new Color(1f, 1f, 1f, 0f));
            OddTexture.Apply();
            ErrorTexture = new Texture2D(1, 1);
            ErrorTexture.SetPixel(0,0, new Color(0.6f, 0.02f, 0f));
            ErrorTexture.Apply();
            Header = new GUIStyle
            {
                padding = new RectOffset(5, 5, 5, 5),
                normal = new GUIStyleState
                {
                    background = HeaderTexture
                }
            };
            H1 = new GUIStyle
            {
                fontSize = 16,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft,
                normal = new GUIStyleState
                {
                    textColor = new Color(0.36f, 0.36f, 0.36f)
                }
            };
            Even = new GUIStyle
            {
                normal = new GUIStyleState { background = EvenTexture }
            };
            Odd = new GUIStyle
            {
                normal = new GUIStyleState { background = OddTexture }
            };
            Error = new GUIStyle
            {
                normal = new GUIStyleState { background = ErrorTexture }
            };
        }

        private readonly string _name;
        private readonly ScriptableObject _parent;
        private readonly List<ScriptableAction> _actions;
        
        private readonly Dictionary<ScriptableAction, UnityEditor.Editor> _cache = new();
        private readonly List<string> _headers = new ();
        
        public ScriptableActionsEditor(string name, ScriptableObject parent, List<ScriptableAction> actions)
        {
            _name = name;
            _parent = parent;
            _actions = actions;
        }
        
        public void Draw()
        {
            RefreshHeaders();
            DrawMainHeader();
            DrawContent();
        }
        
        private void RefreshHeaders()
        {
            _headers.Clear();
            foreach (ScriptableAction action in _actions)
            {
                if (!action) continue;
                string typeName = action.GetType().Name;
                if (!_headers.Contains(typeName)) _headers.Add(typeName);
            }
        }
        
        private void DrawMainHeader()
        {
            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal(Header,GUILayout.ExpandWidth(true));
            GUILayout.Label(_name, H1, GUILayout.ExpandWidth(true));
            if (GUILayout.Button("Add New", GUILayout.MaxWidth(60))) AddScriptableActionMenu.Open(_parent, _actions);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawSmallHeader(string name, int count)
        {
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();

            if (GUILayout.Button(name + " (" + count + ")", GUILayout.Height(20)))
            {
                if (CollapsedDrawers.ContainsKey(name))
                {
                    CollapsedDrawers[name] = !CollapsedDrawers[name] ;
                }
                else CollapsedDrawers.Add(name, true);
            }

            if (GUILayout.Button(EditorGUIUtility.IconContent("cs Script Icon"), GUILayout.MaxWidth(25), GUILayout.Height(20)))
            {
                foreach (ScriptableAction action in _actions)
                {
                    if (!action || action.GetType().Name != name) continue;
                    Selection.activeObject = MonoScript.FromScriptableObject(action);
                    break;
                }
            }

            if (GUILayout.Button("＋", GUILayout.MaxWidth(25), GUILayout.Height(20)))
            {
                EditorGUI.indentLevel++;
                ScriptableAction scriptableAction = null;
                foreach (ScriptableAction action in _actions)
                {
                    if (!action || action.GetType().Name != name) continue;
                    scriptableAction = ScriptableObject.CreateInstance(action.GetType()) as ScriptableAction;
                    break;
                }
                if (!scriptableAction) return;
                scriptableAction.name = "New " + name;
                _actions?.Add(scriptableAction);
                AssetDatabase.AddObjectToAsset(scriptableAction, _parent);
                EditorUtility.SetDirty(_parent);
                AssetDatabase.SaveAssets();
                return;
            }
            GUILayout.EndHorizontal();
        }

        private void DrawContent()
        {
            int entry = 0;
            foreach (var header in _headers)
            {
                int count = _actions.Sum(test => test && test.GetType().Name == header ? 1 : 0);
                DrawSmallHeader(header, count);
                EditorGUI.indentLevel++;
                foreach (ScriptableAction action in _actions)
                {
                    if (!action) continue;
                    string typeName = action.GetType().Name;
                    if (!string.Equals(typeName, header, StringComparison.Ordinal)) continue;
                    if (CollapsedDrawers.ContainsKey(typeName) && CollapsedDrawers[typeName] == true) continue;
                    entry++;
                    
                    if (action.HasErrors()) GUILayout.BeginHorizontal(Error);
                    else GUILayout.BeginHorizontal(entry % 2 == 0 ? Even : Odd);

                    UnityEditor.Editor cachedEditor = _cache.ContainsKey(action)
                        ? _cache[action] : null;
                    UnityEditor.Editor.CreateCachedEditor(action, null, ref cachedEditor);
                
                    GUILayout.BeginVertical();
                    cachedEditor.OnInspectorGUI();
                    GUILayout.EndVertical();

                    if (!_cache.ContainsKey(action)) _cache.Add(action, cachedEditor);
                    else _cache[action] = cachedEditor;

                    if (GUILayout.Button("Invoke", GUILayout.MaxWidth(60), GUILayout.ExpandHeight(true)))
                    {
                        action.Invoke();
                        return;
                    }
                
                    if (GUILayout.Button(EditorGUIUtility.IconContent("Animation.FilterBySelection"), GUILayout.MaxWidth(25), GUILayout.ExpandHeight(true)))
                    {
                        Selection.activeObject = action;
                        return;
                    }
                
                    if (GUILayout.Button(EditorGUIUtility.IconContent("TreeEditor.Trash"), GUILayout.MaxWidth(25), GUILayout.ExpandHeight(true)))
                    {
                        _actions.Remove(action);
                        AssetDatabase.RemoveObjectFromAsset(action);
                        Object.DestroyImmediate(action);
                        EditorUtility.SetDirty(_parent);
                        AssetDatabase.SaveAssets();
                        return;
                    }

                    GUILayout.EndHorizontal();
                }
                EditorGUI.indentLevel--;
            }
        }
    }
}