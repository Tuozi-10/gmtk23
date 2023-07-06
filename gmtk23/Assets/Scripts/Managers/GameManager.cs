using src.Singletons;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class GameManager : MonoSingleton<GameManager>
    {

        public enum Scenes
        {
            Loading = 0,
            Menu = 1,
            Game = 2
        }
        
        public void LoadScene(Scenes sceneIndex)
        {
            SceneManager.LoadScene((int)sceneIndex);
        }
        
    }
}
