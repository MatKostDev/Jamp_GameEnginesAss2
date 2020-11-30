using System;
using System.Collections.Generic;
using UnityEngine;

namespace Jampacked.ProjectInca
{
    public abstract class ObjectPool<T> : MonoBehaviour where T : MonoBehaviour
    {
        [SerializeField] 
        protected GameObject objectPrefab;

        [SerializeField] 
        protected int initialQuantity;

        protected Queue<GameObject> m_objectQueue = new Queue<GameObject>();

        private static T s_instance = null;

        public static T Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = (T) FindObjectOfType(typeof(T));
                    if (s_instance == null)
                    {
                        throw new NullReferenceException("YOU MUST CREATE A \"" + typeof(T).Name + "\" IN THE HIERARCHY!");
                    }
                }

                return s_instance;
            }
        }

        void Start()
        {
            BuildPool();
        }

        public GameObject GetObject()
        {
            GameObject objectInstance;

            if (IsPoolEmpty())
            {
                objectInstance = SpawnObject(true);
            }
            else
            {
                objectInstance = m_objectQueue.Dequeue();
                objectInstance.SetActive(true);
            }

            return objectInstance;
        }

        public void ResetObject(GameObject a_object)
        {
            a_object.SetActive(false);

            m_objectQueue.Enqueue(a_object);
        }

        public bool IsPoolEmpty()
        {
            return m_objectQueue.Count <= 0;
        }

        public int GetPoolSize()
        {
            return m_objectQueue.Count;
        }

        GameObject SpawnObject(bool a_activate)
        {
            GameObject newObject = Instantiate(objectPrefab, transform);

            newObject.SetActive(a_activate);

            return newObject;
        }

        void BuildPool()
        {
            for (int i = 0; i < initialQuantity; i++)
            {
                m_objectQueue.Enqueue(SpawnObject(false));
            }
        }
    }
}
