using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sand_Breaker
{
    public interface IEvent
    {
        
    }
    public static class EventBus
    {
        private static readonly Dictionary<Type, List<Action<IEvent>>> callBackDict = new Dictionary<Type, List<Action<IEvent>>>();

        public static void SubscribeEventCallBack<T>(Action<T> callBack) where T : IEvent
        {
            Type eventType = typeof(T);
            if (!callBackDict.ContainsKey(eventType))
            {
                callBackDict[eventType] = new List<Action<IEvent>>();
            }
            void action(IEvent e) => callBack((T)e);
            callBackDict[eventType].Add(action);
        }

        public static void UnsubscribeEventCallBack<T>(Action<T> callBack) where T : IEvent
        {
            Type eventType = typeof(T);
            if (callBackDict.ContainsKey(eventType))
            {
                void action(IEvent e) => callBack((T)e);
                callBackDict[eventType].Remove(action);
            }
        }

        public static void Publish<T>(T eventToPublish) where T : IEvent
        {
            Type eventType = typeof(T);
            if (callBackDict.ContainsKey(eventType))
            {
                foreach (Action<IEvent> callBack in callBackDict[eventType])
                {
                    callBack(eventToPublish);
                }
            }
        }

    }
}
