using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace IronMountain.ScriptableActions.Editor
{
    public static class AddScriptableActionMenu
    {
        public static readonly List<Type> ScriptableActionTypes;

        static AddScriptableActionMenu()
        {
            ScriptableActionTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsSubclassOf(typeof(ScriptableAction)) && !type.IsAbstract)
                .ToList();
        }
        
        public static void Open(Object parent, List<ScriptableAction> list)
        {
            GenericMenu menu = new GenericMenu();
            Dictionary<string, List<Type>> entries = new Dictionary<string, List<Type>>();

            foreach (Type scriptableActionType in ScriptableActionTypes)
            {
                if (scriptableActionType == null
                    || string.IsNullOrWhiteSpace(scriptableActionType.FullName)
                    || scriptableActionType.IsAbstract) continue;
                
                List<string> pathSegments = scriptableActionType.FullName.Split('.').ToList();
                pathSegments.RemoveAll(test => test is "Action" or "Actions" or "ScriptableActions");
                
                string root = pathSegments.Count > 1 ? pathSegments[0] : string.Empty;
                if (entries.ContainsKey(root))
                {
                    entries[root].Add(scriptableActionType);
                }
                else entries.Add(root, new List<Type> { scriptableActionType });
            }
            
            foreach (string key in entries.Keys)
            {
                if (key == string.Empty) continue;
                menu.AddSeparator(AddSpacesToSentence(key));
                foreach (var derivedType in entries[key])
                {
                    AddType(menu, derivedType, parent, list);
                }
            }

            if (entries.ContainsKey(string.Empty))
            {
                menu.AddSeparator("Other");
                foreach (var derivedType in entries[string.Empty])
                {
                    AddType(menu, derivedType, parent, list);
                }
            }

            menu.ShowAsContext();
        }
        
        private static void AddType(GenericMenu menu, Type type, Object parent, List<ScriptableAction> list)
        {
            if (type == null) return;
            List<string> pathSegments = type.FullName.Split('.').ToList();
            pathSegments.RemoveAll(test => test is "Action" or "Actions" or "ScriptableActions");
            if (pathSegments.Count > 1) pathSegments.RemoveAt(0);
            
            string typeName = pathSegments[^1];
            typeName = AddSpacesToSentence(typeName);
            List<string> typeNameSegments = typeName.Split(' ').ToList();
            typeNameSegments.RemoveAll(test => test is "Action" or "Actions");
            pathSegments[^1] = "Add " + string.Join(' ', typeNameSegments);
                
            string path = string.Join('/', pathSegments);
            AddMenuItem(menu, type, typeName, path, parent, list);
        }

        private static void AddMenuItem(GenericMenu menu, Type type, string typeName, string path, Object parent, List<ScriptableAction> list)
        {
            menu.AddItem(new GUIContent(path), false,
                () =>
                {
                    string parentObjectPath = AssetDatabase.GetAssetPath(parent);
                    if (string.IsNullOrEmpty(parentObjectPath)) return;
                    ScriptableAction scriptableAction = ScriptableObject.CreateInstance(type) as ScriptableAction;
                    if (!scriptableAction) return;
                    scriptableAction.name = "New " + typeName;
                    list?.Add(scriptableAction);
                    AssetDatabase.AddObjectToAsset(scriptableAction, parent);
                    EditorUtility.SetDirty(parent);
                    AssetDatabase.SaveAssets();
                });
        }

        private static string AddSpacesToSentence(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;
            StringBuilder newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);
            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]))
                    if (text[i - 1] != ' ' && !char.IsUpper(text[i - 1]))
                        newText.Append(' ');
                newText.Append(text[i]);
            }
            return newText.ToString();
        }
    }
}