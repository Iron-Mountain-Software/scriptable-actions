using UnityEngine;
using UnityEngine.SceneManagement;

namespace IronMountain.ScriptableActions
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Scriptable Actions/Load Scene by Name")]
    public class LoadSceneByNameAction : ScriptableAction
    {
        [SerializeField] private string sceneName;
        [SerializeField] private LoadSceneMode loadSceneMode;
        
        public override void Invoke() => SceneManager.LoadScene(sceneName, loadSceneMode);

        public override string ToString() => "Load Scene: " + sceneName;

        public override bool HasErrors() => false;
    }
}