using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CalongeCore.Events
{
	public static class EventsManager
	{
		private static Dictionary<Type , HashSet<IInvoker>> events = new Dictionary<Type, HashSet<IInvoker>>();
        private static HashSet<IGameEvent> cachedEvents = new HashSet<IGameEvent>();
		
		public static void SubscribeToEvent<T>(Action listener) where T : IGameEvent, new()
		{
			Type typeOfT = typeof(T);
			
			if (!events.ContainsKey(typeOfT))
			{
				events.Add(typeOfT, new HashSet<IInvoker>());
                cachedEvents.Add(new T());

            }

			events[typeOfT].Add(CreateCallback(listener));
		}
		
		public static void SubscribeToEvent<T>(Action<T> listener) where T : IGameEvent, new()
		{
            Type typeOfT = typeof(T);

			if (!events.ContainsKey(typeOfT))
			{
				events.Add(typeOfT, new HashSet<IInvoker>());
                cachedEvents.Add(new T());
            }

            events[typeOfT].Add(CreateCallback(listener));
		}

		public static void UnsubscribeToEvent<T>(Action listener) where T : IGameEvent
		{
            Type typeOfT = typeof(T);

			if (events.ContainsKey(typeOfT))
			{
                HashSet<IInvoker> hashSet = events[typeOfT];
				var invokerToRemove = hashSet.SingleOrDefault(x => ((ParameterlessInvoker) x).Handler.Equals(listener));

				if (invokerToRemove != null)
				{
					hashSet.Remove(invokerToRemove);	
				}

				if (hashSet.Count == 0)
				{
					events.Remove(typeOfT);
                    cachedEvents.Remove(cachedEvents.First(x => x.GetType() == typeof(T)));
				}
			}
		}
		
		public static void UnsubscribeToEvent<T>(Action<T> listener) where T : IGameEvent
		{
            Type typeOfT = typeof(T);

			if (events.ContainsKey(typeOfT))
			{
                HashSet<IInvoker> hashSet = events[typeOfT];
				var invokerToRemove = hashSet.SingleOrDefault(x => ((SpecificInvoker<T>) x).Handler.Equals(listener));

				if (invokerToRemove != null)
				{
					hashSet.Remove(invokerToRemove);	
				}

				if (hashSet.Count == 0)
				{
					events.Remove(typeOfT);
                    cachedEvents.Remove(cachedEvents.First(x => x.GetType() == typeof(T)));
                }
            }
		}

		public static void DispatchEvent(IGameEvent gameEvent)
		{
			Type type = gameEvent.GetType();

            if (!events.ContainsKey(type) || events[type] == null)
            {
                return;
            }

            HashSet<IInvoker> invokeList = events[type];
			foreach (var invoke in invokeList)
			{
				invoke.Invoke(gameEvent);	
			}

            gameEvent.Reset();

        }

        public static void DispatchEvent<T>(Action<T> gameEvent) where T : IGameEvent
        {
            T pooledEvent = GetPooledEvent<T>();

            if (pooledEvent == null)
            {
                return;
            }

            Type type = pooledEvent.GetType();

            if (!events.ContainsKey(type) || events[type] == null)
            {
                return;
            }

            gameEvent.Invoke(pooledEvent);

            HashSet<IInvoker> invokeList = events[pooledEvent.GetType()];
            foreach (var invoke in invokeList)
            {
                invoke.Invoke(pooledEvent);
            }

            pooledEvent.Reset();
        }

        public static T GetPooledEvent<T>() where T : IGameEvent
        {
            return (T) cachedEvents.FirstOrDefault(x => typeof(T) == x.GetType());
        }


        public static void ClearDictionary()
		{
			events.Clear();
		}

		public static void ResetDictionary()
		{
			events = new Dictionary<Type, HashSet<IInvoker>>();
		}

		private static IInvoker CreateCallback(Action listener)
		{
			return new ParameterlessInvoker {Handler = listener};
		}
		
		private static IInvoker CreateCallback<T>(Action<T> listener) where T : IGameEvent
		{
			return new SpecificInvoker<T> {Handler = listener};
		}
	}

	public class ParameterlessInvoker : IInvoker
	{
		public Action Handler { get; set; }
		
		public void Invoke(IGameEvent gameEvent)
		{
			Handler.Invoke();
		}
	}

	public class SpecificInvoker<T> : IInvoker where T : IGameEvent
	{
		public Action<T> Handler { get; set; }
		
		public void Invoke(IGameEvent gameEvent)
		{
			Handler.Invoke((T)gameEvent);
		}
	}

	public interface IGameEvent
	{
        void Reset();
	}

	public interface IInvoker
	{
		void Invoke(IGameEvent gameEvent);
	}
}