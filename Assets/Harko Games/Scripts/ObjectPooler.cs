using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour {
    public static ObjectPooler SharedInstance;

    public List<GameObject> pooledObjects;
    public GameObject objectToPool;
    public int amountToPool;
    public Transform parentTransform;

    private void Awake()
    {
        SharedInstance = this;
    }

    void Start () {
        pooledObjects = new List<GameObject>();
        for (int lcv = 0; lcv < amountToPool; lcv++)
        {
            var obj = (GameObject)Instantiate(objectToPool);
            obj.SetActive(false);
            obj.transform.SetParent(parentTransform);
            pooledObjects.Add(obj);
        }
	}
	
    public GameObject GetPooledObject()
    {
        for (int lcv = 0, length = pooledObjects.Count; lcv < length; lcv++)
        {
            if (!pooledObjects[lcv].activeInHierarchy)
            {
                return pooledObjects[lcv];
            }
        }
        return null;
    }
	// Update is called once per frame
	void Update () {
		
	}
}
