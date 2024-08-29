using UnityEngine;

    public class PersistentSingleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                // If the singleton instance does not exist, try to find it in the scene
                if (_instance != null) return _instance;
                
                _instance = FindFirstObjectByType<T>();

                if (_instance == null)
                {
                    Debug.LogError("Singleton not found " + nameof(T));
                }

                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(this.gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
    }
