using System;
using System.Collections.Generic;

namespace BattleSimulator
{
    /// <summary>
    /// This class routes global events throughout the entire application. Use this class to raise events without coupling 
    /// to the handlers of those events and use it to subscribe for such events in your own implementations of IDomainEventHandler.
    /// 
    /// Use DomainEvents when you have classes whose sole purpose is to take action in response to application-wide events.
    /// </summary>
    public static class DomainEvents
    {
        private static readonly Dictionary<Type, List<IDomainEventHandler>> Handlers = new Dictionary<Type, List<IDomainEventHandler>>();

        /// <summary>
        /// Should only be used in unit testing
        /// </summary>
        public static void RemoveAllHandlers()
        {
            Handlers.Clear();
        }

        public static void RegisterDomainEventHandler<T>(IDomainEventHandler<T> handler) where T : IDomainEvent
        {
            RegisterDomainEventHandler(handler, new[] { typeof(T) });
        }

        public static void RegisterDomainEventHandler(IDomainEventHandler handler)
        {
            RegisterDomainEventHandler(handler, GetIDomainEventTypes(handler));
        }

        private static void RegisterDomainEventHandler(IDomainEventHandler handler, IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                if (Handlers.ContainsKey(type) == false)
                {
                    Handlers[type] = new List<IDomainEventHandler>();
                }
                Handlers[type].Add(handler);
            }
        }

        public static void UnregisterDomainEventHandler<T>(IDomainEventHandler<T> handler) where T : IDomainEvent
        {
            UnregisterDomainEventHandler(handler, new[] { typeof(T) });
        }

        public static void UnregisterDomainEventHandler(IDomainEventHandler handler)
        {
            UnregisterDomainEventHandler(handler, GetIDomainEventTypes(handler));
        }

        private static void UnregisterDomainEventHandler(IDomainEventHandler handler, IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                if (Handlers.ContainsKey(type))
                {
                    Handlers[type].Remove(handler);
                }
            }
        }

        public static void Raise<T>(T args) where T : IDomainEvent
        {
            var type = typeof(T);
            if (!Handlers.ContainsKey(type))
            {
                return;
            }
            //make a copy of the list to prevent a handle call then calling register and 
            //us trying to modify a list while enumerating it

            var handlers = Handlers[type];
            foreach (var handler in handlers)
            {
                if (handler is not IDomainEventHandler<T> iDomainEventHandler)
                {
                    continue;
                }
                iDomainEventHandler.Handle(args);
            }
        }

        private static IEnumerable<Type> GetIDomainEventTypes(IDomainEventHandler handler)
        {
            var handlerInterfaces = handler.GetType().GetInterfaces();
            var types = new List<Type>();
            foreach (var handlerInterface in handlerInterfaces)
            {
                if (handlerInterface.IsGenericType && handlerInterface.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>))
                {
                    types.Add(handlerInterface.GetGenericArguments()[0]);
                }
            }
            return types;
        }
    }
}