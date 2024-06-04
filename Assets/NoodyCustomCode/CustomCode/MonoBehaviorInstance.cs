using UnityEngine;

namespace NOOD
{
    public class MonoBehaviorInstance <T> : AbstractMonoBehaviour where T : MonoBehaviour
    {
        [SerializeField] protected bool _dontDestroyOnLoad;

        private static T s_instance;
        public static T Instance
        {
            get
            {
                if (s_instance == null || s_instance.gameObject == null) TryGetInstance();
                return s_instance;
            }
        }

        protected void Awake()
        {
            if (s_instance != null && s_instance.gameObject != this.gameObject)
            {
                // Already have an instance and that instance is this gameObject
                Debug.LogError($"Exist 2 {typeof(T)} in the scene {this.gameObject.name} and {s_instance.gameObject.name}");
                Destroy(this.gameObject);
            }

            if (s_instance == null || s_instance.gameObject == null) 
            {
                // Don't have any instance
                TryGetInstance();
            }

            if(_dontDestroyOnLoad)
            {
                DontDestroyOnLoad(s_instance.gameObject);
            }
            ChildAwake();
        }

        private static void TryGetInstance()
        {
            s_instance = GameObject.FindObjectOfType<T>();
        }

        protected virtual void ChildAwake()
        {}
    }
}


