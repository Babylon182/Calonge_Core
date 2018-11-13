﻿﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace ObjectsPool
{
	//TODO: Usar Zenject para inyectar esta clase en la escena
	
	public class GodPool : MonoBehaviour
	{
		public static GodPool Instance => instance;
		private static GodPool instance;
		
		[SerializeField] private PoolAmount[] initialPoolObjects;
		private Dictionary<string, Stack<PoolObject>> inactivePoolObjects;
		private Dictionary<string, Dictionary<int, PoolObject>> activePoolObjects;
		private Dictionary<string, GameObject> containers;

		private const string CONTAINER = "Container";
		
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

		public GameObject InstantiatePoolObject(GameObject poolObjectType, Vector3 initialPosition, Quaternion intialRotation)
		{
			CheckContainers(poolObjectType);
			GameObject poolObject = GetObjectFromPool(poolObjectType).PoolGameObject;
			poolObject.transform.position = initialPosition;
			poolObject.transform.rotation = intialRotation;

			return poolObject;
		}
		
		public GameObject InstantiatePoolObject(GameObject poolObjectType)
		{
			return InstantiatePoolObject(poolObjectType, Vector3.zero, Quaternion.identity);
		}

		public void ReturnPoolObject(GameObject poolObjectType)
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
				GameObject newGameObject = Instantiate(poolObjectType);
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
					GameObject newGameObject = Instantiate(poolGameObject, Vector3.zero, Quaternion.identity);
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

	public class PoolObject
	{   
		private bool isActive;
		private Action initCallback;
		private Action disposeCallback;

		public PoolObject(GameObject poolGameObject, Action initCallback, Action disposeCallback)
		{
			this.PoolGameObject = poolGameObject;
			this.initCallback = initCallback;
			this.disposeCallback = disposeCallback;
			Id = poolGameObject.GetInstanceID();
		}

		public GameObject PoolGameObject { get;}
		public int Id { get;}

		public bool IsActive
		{
			get
			{
				return isActive;
			}
			set 
			{ 
				isActive = value;

				if (isActive) 
					initCallback.Invoke();
				else 
					disposeCallback.Invoke();
			}
		}
	}

	[Serializable]
	public class PoolAmount
	{
		public GameObject gameObject;
		public int ammount;
	}

	public interface IPoolable
	{
		void Init();
		void Dispose();
	}
}