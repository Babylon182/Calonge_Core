﻿﻿using System;
using System.Collections.Generic;
using UnityEngine;
  using Object = UnityEngine.Object;

namespace CalongeCore.ObjectsPool
{
	public class GodPoolSingleton : Singleton<GodPoolSingleton>, IPool
	{
		[SerializeField] 
		private PoolAmount[] initialPoolObjects;
		
		private Dictionary<string, Stack<PoolObject>> inactivePoolObjects = new Dictionary<string, Stack<PoolObject>>();
		private Dictionary<string, Dictionary<int, PoolObject>> activePoolObjects  = new Dictionary<string, Dictionary<int, PoolObject>>();
		private Dictionary<string, GameObject> containers = new Dictionary<string, GameObject>();

		private const string CONTAINER = " Container";

		protected override void Awake()
		{
			base.Awake();
			CreateInitialPoolObjects();
		}

		public GameObject Instantiate(GameObject poolObjectType, Vector3 initialPosition, Quaternion initialRotation)
		{
			CheckContainers(poolObjectType);
			GameObject poolObject = GetObjectFromPool(poolObjectType).PoolGameObject;
			poolObject.transform.position = initialPosition;
			poolObject.transform.rotation = initialRotation;

			return poolObject;
		}
		
		public T Instantiate<T>(GameObject poolObjectType, Vector3 initialPosition, Quaternion initialRotation)
		{
			return Instantiate(poolObjectType, initialPosition, initialRotation).GetComponent<T>();
		}
		
		public GameObject Instantiate(GameObject poolObjectType)
		{
			return Instantiate(poolObjectType, Vector3.zero, Quaternion.identity);
		}

		public void Destroy(GameObject poolObjectType)
		{
			if (!activePoolObjects.ContainsKey(poolObjectType.name))
				return;
			
			Dictionary<int, PoolObject> dictionaryOfActiveGameObjects = activePoolObjects[poolObjectType.name];
			PoolObject poolObject = dictionaryOfActiveGameObjects[poolObjectType.GetInstanceID()];
			poolObject.IsActive = false;
			
			dictionaryOfActiveGameObjects.Remove(poolObject.Id);
			inactivePoolObjects[poolObjectType.name].Push(poolObject);	
		}

		private PoolObject GetObjectFromPool(GameObject poolObjectType)
		{
			Stack<PoolObject> stackOfInactiveGameObjects = inactivePoolObjects[poolObjectType.name];

			if (stackOfInactiveGameObjects.Count > 0)
			{
				PoolObject poolObject = stackOfInactiveGameObjects.Pop();
				poolObject.IsActive = true;
				
				activePoolObjects[poolObjectType.name].Add(poolObject.Id, poolObject);	
				return poolObject;
			}
			else
			{
				GameObject newGameObject = GameObject.Instantiate(poolObjectType);
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
			IPoolable[] poolInterface = poolObjectType.GetComponents<IPoolable>();
			Action onInit = () => poolObjectType.SetActive(true);
			Action onDispose = () => poolObjectType.SetActive(false);
			
			foreach (var ipooleable in poolInterface)
			{
				onInit += () => ipooleable?.Init();
				onDispose += () => ipooleable?.Dispose(); 
			}

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