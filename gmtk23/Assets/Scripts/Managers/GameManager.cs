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
            if (sceneIndex == Scenes.Game)
            {
                Destroy(PackHeroManager.instance.gameObject);
                PackHeroManager.instance = null;
            }
            SceneManager.LoadScene((int)sceneIndex);
        }
        
    }
}
