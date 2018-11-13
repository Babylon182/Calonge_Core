using UnityEngine;

namespace ObjectsPool
{
    public class TestingObject : MonoBehaviour , IPoolable
    {
        public void Init()
        {
            Debug.Log("INIT");
            gameObject.SetActive(true);
        }

        public void Dispose()
        {
            Debug.Log("DISPOSE");
            gameObject.SetActive(false);
        }
    }
}