using UnityEngine;
using UnityEngine.Pool;

namespace BattleSimulator
{
    public class HealthUIPool : MonoBehaviour
    {
        public HealthUI Prefab;
        public int OverrideDefaultCapacity = 10;

        public static ObjectPool<HealthUI> Pool;

        private void Awake()
        {
            Pool = new ObjectPool<HealthUI>(
                CreatePooledObject, 
                GetPooledObject, 
                ReturnPooledObject, 
                DestroyPooledObject, 
                false,
                OverrideDefaultCapacity);
        }

        private HealthUI CreatePooledObject()
        {
            var pooledObject = Instantiate(Prefab, transform);
            return pooledObject;
        }

        private void GetPooledObject(HealthUI pooledObject)
        {
            pooledObject.gameObject.SetActive(true);
        }

        private void ReturnPooledObject(HealthUI pooledObject)
        {
            pooledObject.gameObject.SetActive(false);
        }

        private void DestroyPooledObject(HealthUI pooledObject)
        {
            Destroy(pooledObject.gameObject);
        }
    }
}
