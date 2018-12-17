using System;
using System.Collections.Generic;
using System.Linq;

namespace CalongeCore.Events
{
	public static class EventsManager
	{
		private static Dictionary<Type , HashSet<IInvoker>> events = new Dictionary<Type, HashSet<IInvoker>>();
		
		public static void SubscribeToEvent<T>(Action listener) where T : IGameEvent
		{
			var typeOfT = typeof(T);
			
			if (!events.ContainsKey(typeOfT))
			{
				events.Add(typeOfT, new HashSet<IInvoker>());
			}

			events[typeOfT].Add(CreateCallback(listener));
		}
		
		public static void SubscribeToEvent<T>(Action<T> listener) where T : IGameEvent
		{
			var typeOfT = typeof(T);

			if (!events.ContainsKey(typeOfT))
			{
				events.Add(typeOfT, new HashSet<IInvoker>());
			}

			events[typeOfT].Add(CreateCallback(listener));
		}

		public static void UnsubscribeToEvent<T>(Action listener) where T : IGameEvent
		{
			var typeOfT = typeof(T);

			if (events.ContainsKey(typeOfT))
			{
				var hashSet = events[typeOfT];
				var invokerToRemove = hashSet.SingleOrDefault(x => ((ParameterlessInvoker) x).Handler.Equals(listener));

				if (invokerToRemove != null)
				{
					hashSet.Remove(invokerToRemove);	
				}

				if (hashSet.Count == 0)
				{
					events.Remove(typeOfT);
				}
			}
		}
		
		public static void UnsubscribeToEvent<T>(Action<T> listener) where T : IGameEvent
		{
			var typeOfT = typeof(T);

			if (events.ContainsKey(typeOfT))
			{
				var hashSet = events[typeOfT];
				var invokerToRemove = hashSet.SingleOrDefault(x => ((SpecificInvoker<T>) x).Handler.Equals(listener));

				if (invokerToRemove != null)
				{
					hashSet.Remove(invokerToRemove);	
				}

				if (hashSet.Count == 0)
				{
					events.Remove(typeOfT);
				}
			}
		}

		public static void DispatchEvent(IGameEvent gameEvent)
		{
			Type type = gameEvent.GetType();
			
			if (!events.ContainsKey(type) || events[type] == null) return;
			
			HashSet<IInvoker> invokeList = events[type];
			foreach (var invoke in invokeList)
			{
				invoke.Invoke(gameEvent);	
			}
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
		
	}

	public interface IInvoker
	{
		void Invoke(IGameEvent gameEvent);
	}
}