using UnityEngine;

namespace IronMountain.ScriptableActions
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Scriptable Actions/Debug.Log()")]
    public class DebugLogAction : ScriptableAction
    {
        [SerializeField] private string message;
        
        public override void Invoke() => Debug.Log(message);

        public override string ToString() => message;

        public override bool HasErrors() => false;
    }
}