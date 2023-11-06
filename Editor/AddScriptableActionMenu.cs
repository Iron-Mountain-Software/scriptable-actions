using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace IronMountain.ScriptableActions.Editor
{
    public static class AddScriptableActionMenu
    {
        public static readonly List<Type> ScriptableActionTypes;

        static AddScriptableActionMenu()
        {
            ScriptableActionTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsSubclassOf(typeof(ScriptableAction)))
                .ToList();
        }
        
        public static void Open(ScriptableObject parent, List<ScriptableAction> list)
        {
            GenericMenu menu = new GenericMenu();
            
            foreach (Type scriptableActionType in ScriptableActionTypes)
            {
                if (scriptableActionType == null || scriptableActionType.IsAbstract) continue;
                
                menu.AddItem(new GUIContent("Add " + scriptableActionType.Name), false,
                    () =>
                    {
                        string parentObjectPath = AssetDatabase.GetAssetPath(parent);
                        if (string.IsNullOrEmpty(parentObjectPath)) return;
                        ScriptableAction scriptableAction = ScriptableObject.CreateInstance(scriptableActionType) as ScriptableAction;
                        if (!scriptableAction) return;
                        scriptableAction.name = "New " + scriptableActionType.Name;
                        list?.Add(scriptableAction);
                        AssetDatabase.AddObjectToAsset(scriptableAction, parent);
                        EditorUtility.SetDirty(parent);
                        AssetDatabase.SaveAssets();
                    });
            }

            menu.ShowAsContext();
        }
    }
}