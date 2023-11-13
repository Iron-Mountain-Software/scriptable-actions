using UnityEngine;
using UnityEngine.Events;

namespace IronMountain.ScriptableActions
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Scriptable Actions/UnityEvent")]
    public class UnityEventAction : ScriptableAction
    {
        [SerializeField] private UnityEvent action;
        
        public override void Invoke() => action?.Invoke();

        public override string ToString() => "Unity Event";

        public override bool HasErrors() => false;
    }
}