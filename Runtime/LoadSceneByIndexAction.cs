using UnityEngine;
using UnityEngine.SceneManagement;

namespace IronMountain.ScriptableActions
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Scriptable Actions/Load Scene by Index")]
    public class LoadSceneByIndexAction : ScriptableAction
    {
        [SerializeField] private int sceneIndex;
        [SerializeField] private LoadSceneMode loadSceneMode;
        
        public override void Invoke() => SceneManager.LoadScene(sceneIndex, loadSceneMode);

        public override string ToString() => "Load Scene: " + sceneIndex;

        public override bool HasErrors() => false;
    }
}