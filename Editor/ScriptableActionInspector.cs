using UnityEditor;

namespace IronMountain.ScriptableActions.Editor
{
    [CustomEditor(typeof(ScriptableAction), true)]
    public class ScriptableActionInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawPropertiesExcluding(serializedObject, "m_Script");
            serializedObject.ApplyModifiedProperties();
        }
    }
}