using UnityEditor;
using UnityEngine;

namespace ObjectsPool
{
    public class TestingPool : MonoBehaviour
    {
        public GameObject testing;
        public GameObject testingWithoutInterface;
        private int offset;
        
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                offset++;
                GodPool.Instance.InstantiatePoolObject(testing , new Vector3(offset, 0,0), Quaternion.identity);
            }
            
            if (Input.GetKeyDown(KeyCode.K))
            {
                GodPool.Instance.ReturnPoolObject(Selection.activeGameObject);
            }
            
            if (Input.GetKeyDown(KeyCode.N))
            {
                offset++;
                GodPool.Instance.InstantiatePoolObject(testingWithoutInterface , new Vector3(offset, 0,0), Quaternion.identity);
            }
        }
    }
}