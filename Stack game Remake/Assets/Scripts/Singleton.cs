using UnityEngine;

namespace CoolBears
{
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        public static T Instance { get; private set; }

        private void Awake()
        {
            if (!Instance) Instance = (T)this;
        }
    }
}