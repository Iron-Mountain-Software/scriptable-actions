using System;
using UnityEngine;

namespace IronMountain.ScriptableActions
{
    [Serializable]
    public abstract class ScriptableAction : ScriptableObject
    {
        public abstract void Invoke();
        public abstract override string ToString();
        public abstract bool HasErrors();

#if UNITY_EDITOR

        [ContextMenu("Invoke")]
        private void InvokeFromContextMenu() => Invoke();

#endif
        
    }
}