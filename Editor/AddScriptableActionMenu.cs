using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace IronMountain.ScriptableActions.Editor
{
    public static class AddScriptableActionMenu
    {
        public static void Open(ScriptableObject parent, List<ScriptableAction> list)
        {
            GenericMenu menu = new GenericMenu();
            
            foreach (Type scriptableActionType in ScriptableActionsEditor.ScriptableActionTypes)
            {
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