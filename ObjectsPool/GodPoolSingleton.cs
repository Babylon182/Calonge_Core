﻿﻿using System;
using System.Collections.Generic;
using UnityEngine;
  using Object = UnityEngine.Object;

namespace CalongeCore.ObjectsPool
{
	public class GodPoolSingleton : MonoBehaviour , IPool
	{
		public static GodPoolSingleton Instance => instance;
		private static GodPoolSingleton instance;
		
		[SerializeField] private PoolAmount[] initialPoolObjects;
		private Dictionary<string, Stack<PoolObject>> inactivePoolObjects;
		private Dictionary<string, Dictionary<int, PoolObject>> activePoolObjects;
		private Dictionary<string, GameObject> containers;

		private const string CONTAINER = " Container";
		
		private void Awake()
		{
			if (instance == null)
			{
				instance = this;
				inactivePoolObjects = new Dictionary<string, Stack<PoolObject>>();
				activePoolObjects = new Dictionary<string, Dictionary<int, PoolObject>>();
				containers = new Dictionary<string, GameObject>();
				CreateInitialPoolObjects();
			}
			else
			{
				Destroy(this);
			}
		}

		public GameObject Instantiate(GameObject poolObjectType, Vector3 initialPosition, Quaternion intialRotation)
		{
			CheckContainers(poolObjectType);
			GameObject poolObject = GetObjectFromPool(poolObjectType).PoolGameObject;
			poolObject.transform.position = initialPosition;
			poolObject.transform.rotation = intialRotation;

			return poolObject;
		}
		
		public GameObject Instantiate(GameObject poolObjectType)
		{
			return Instantiate(poolObjectType, Vector3.zero, Quaternion.identity);
		}

		public void Destroy(GameObject poolObjectType)
		{
			if (!activePoolObjects.ContainsKey(poolObjectType.name))
				return;
			
			Dictionary<int, PoolObject> dictionaryOfActiveGameobjects = activePoolObjects[poolObjectType.name];
			PoolObject poolObject = dictionaryOfActiveGameobjects[poolObjectType.GetInstanceID()];
			poolObject.IsActive = false;
			
			dictionaryOfActiveGameobjects.Remove(poolObject.Id);
			inactivePoolObjects[poolObjectType.name].Push(poolObject);	
		}

		private PoolObject GetObjectFromPool(GameObject poolObjectType)
		{
			Stack<PoolObject> stackOfInactiveGameobjects = inactivePoolObjects[poolObjectType.name];

			if (stackOfInactiveGameobjects.Count > 0)
			{
				PoolObject poolObject = stackOfInactiveGameobjects.Pop();
				poolObject.IsActive = true;
				
				activePoolObjects[poolObjectType.name].Add(poolObject.Id, poolObject);	
				return poolObject;
			}
			else
			{
				GameObject newGameObject = Object.Instantiate(poolObjectType);
				newGameObject.name = poolObjectType.name;
				newGameObject.transform.parent = containers[newGameObject.name].transform;
	
				PoolObject newPoolObject = CreateGenericPoolObject(newGameObject);
				newPoolObject.IsActive = true;
				activePoolObjects[poolObjectType.name].Add(newPoolObject.Id, newPoolObject);
				
				return newPoolObject;	
			}
		}

		private PoolObject CreateGenericPoolObject(GameObject poolObjectType)
		{
			IPoolable poolInterface = poolObjectType.GetComponent<IPoolable>();

			Action onInit = () => { }; 
			onInit += () => poolObjectType.SetActive(true);
			onInit += () => poolInterface?.Init();

			Action onDispose = () => { }; 
			onDispose += () => poolInterface?.Dispose(); 
			onDispose += () => poolObjectType.SetActive(false);

			return new PoolObject(poolObjectType, onInit, onDispose);
		}

		private void CheckContainers(GameObject poolObjectType)
		{
			if (inactivePoolObjects.ContainsKey(poolObjectType.name))
				return;
			
			string poolObjectName = poolObjectType.name;
			inactivePoolObjects.Add(poolObjectName, new Stack<PoolObject>());
			activePoolObjects.Add(poolObjectName, new Dictionary<int, PoolObject>());
			
			GameObject newContainer = new GameObject(poolObjectName + CONTAINER);
			newContainer.transform.parent = transform;
			containers.Add(poolObjectName, newContainer);	
		}

		private void CreateInitialPoolObjects()
		{
			for (int i = initialPoolObjects.Length - 1; i >= 0; i--)
			{
				PoolAmount poolAmount = initialPoolObjects[i];
				GameObject poolGameObject = poolAmount.gameObject;
				CheckContainers(poolGameObject);
				for (int j = poolAmount.ammount; j > 0; j--)
				{
					GameObject newGameObject = Object.Instantiate(poolGameObject, Vector3.zero, Quaternion.identity);
					newGameObject.name = poolGameObject.name;
					newGameObject.transform.parent = containers[newGameObject.name].transform;
					
					PoolObject newPoolObject = CreateGenericPoolObject(newGameObject);
					newPoolObject.IsActive = false;
					inactivePoolObjects[poolGameObject.name].Push(newPoolObject);
				}
			}
			initialPoolObjects = null;
		}
	}
}