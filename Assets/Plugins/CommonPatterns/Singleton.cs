using UnityEngine;

namespace WillakeD.CommonPatterns
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static T _instance;
        public bool _isPersistant;

        public static T instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = (T)FindObjectOfType(typeof(T));

                    if (_instance == null)
                    {
                        Debug.LogError("An instance of " + typeof(T) +
                                        " is needed in the scene, but there is none.");
                    }
                }

                return _instance;
            }
        }

        public static bool isExist
        {
            get
            {
                if (_instance == null)
                {
                    return false;
                }

                return true;
            }
        }

        private void Awake()
        {
            if (_isPersistant)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}