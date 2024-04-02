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
        private static readonly Dictionary<string, bool> CollapsedDrawers = new ();

        private readonly string _name;
        private readonly ScriptableObject _parent;
        private readonly List<ScriptableAction> _actions;
        
        private readonly Dictionary<ScriptableAction, UnityEditor.Editor> _cache = new();
        private readonly List<string> _headers = new ();
        private readonly GUIStyle _header;
        private readonly GUIStyle _h1;
        private readonly GUIStyle _even;
        private readonly GUIStyle _odd;
        private readonly GUIStyle _error;
        
        public ScriptableActionsEditor(string name, ScriptableObject parent, List<ScriptableAction> actions)
        {
            _name = name;
            _parent = parent;
            _actions = actions;
            
            Texture2D headerTexture = new Texture2D(1, 1);
            headerTexture.SetPixel(0,0, new Color(0.12f, 0.12f, 0.12f));
            headerTexture.Apply();
            Texture2D evenTexture = new Texture2D(1, 1);
            evenTexture.SetPixel(0,0, new Color(1f, 1f, 1f, .05f));
            evenTexture.Apply();
            Texture2D oddTexture = new Texture2D(1, 1);
            oddTexture.SetPixel(0,0, new Color(1f, 1f, 1f, 0f));
            oddTexture.Apply();
            Texture2D errorTexture = new Texture2D(1, 1);
            errorTexture.SetPixel(0,0, new Color(0.6f, 0.02f, 0f));
            errorTexture.Apply();
            _header = new GUIStyle
            {
                padding = new RectOffset(5, 5, 5, 5),
                normal = new GUIStyleState
                {
                    background = headerTexture
                }
            };
            _h1 = new GUIStyle
            {
                fontSize = 16,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft,
                normal = new GUIStyleState
                {
                    textColor = new Color(0.36f, 0.36f, 0.36f)
                }
            };
            _even = new GUIStyle
            {
                normal = new GUIStyleState { background = evenTexture }
            };
            _odd = new GUIStyle
            {
                normal = new GUIStyleState { background = oddTexture }
            };
            _error = new GUIStyle
            {
                normal = new GUIStyleState { background = errorTexture }
            };
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
            EditorGUILayout.BeginHorizontal(_header,GUILayout.ExpandWidth(true));
            GUILayout.Label(_name, _h1, GUILayout.ExpandWidth(true));
            if (GUILayout.Button("Add Action", GUILayout.MaxWidth(125))) AddScriptableActionMenu.Open(_parent, _actions);
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
                    
                    if (action.HasErrors()) GUILayout.BeginHorizontal(_error);
                    else GUILayout.BeginHorizontal(entry % 2 == 0 ? _even : _odd);

                    UnityEditor.Editor cachedEditor = _cache.ContainsKey(action)
                        ? _cache[action] : null;
                    UnityEditor.Editor.CreateCachedEditor(action, null, ref cachedEditor);
                
                    GUILayout.BeginVertical();
                    cachedEditor.OnInspectorGUI();
                    GUILayout.EndVertical();

                    if (!_cache.ContainsKey(action)) _cache.Add(action, cachedEditor);
                    else _cache[action] = cachedEditor;

                    if (GUILayout.Button("Invoke", GUILayout.MaxWidth(60)))
                    {
                        action.Invoke();
                        return;
                    }
                
                    if (GUILayout.Button(EditorGUIUtility.IconContent("Animation.FilterBySelection"), GUILayout.MaxWidth(25)))
                    {
                        EditorGUIUtility.PingObject(action);
                        return;
                    }
                
                    if (GUILayout.Button(EditorGUIUtility.IconContent("TreeEditor.Trash"), GUILayout.MaxWidth(25)))
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