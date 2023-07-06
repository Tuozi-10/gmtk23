using src.Log;
using UnityEngine;

namespace src.Singletons
{
    public class MonoSingleton<T> : MonoBehaviour
    {
        public static T instance;

        private void Awake()
        {
            if (instance is not null)
            {
                Logs.Log("Destroyed re instantiated singleton");
                DestroyImmediate(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
            instance = GetComponent<T>();
        }
    }
}