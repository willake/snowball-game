using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using WillakeD.CommonPatterns;

namespace Game
{
    public partial class DIContainer : Singleton<DIContainer>
    {
        protected Dictionary<Type, object> _constructorContainer = new Dictionary<Type, object>();
        protected Dictionary<Type, object> _container = new Dictionary<Type, object>();

        /// <summary>For cyclic dependency check</summary>
        protected Stack<Type> constructStack = new Stack<Type>();

        /// <summary>Store locks registered types</summary>
        protected ConcurrentDictionary<Type, object> _locks = new ConcurrentDictionary<Type, object>();

        public DIContainer() { }

        ///<summary>Register a delegate to DI for lazy instatiation.</summary>
        public bool Register<T>(Func<T> constructFunc)
        {
            if (_constructorContainer.ContainsKey(typeof(T)))
            {
                return false;
            }
            _constructorContainer.Add(typeof(T), constructFunc);
            return true;
        }

        ///<summary>Add an instance for DI usage.</summary>
        public bool Add<T>(T value)
        {
            if (_container.ContainsKey(typeof(T)))
            {
                return false;
            }
            _container.Add(typeof(T), value);
            return true;
        }

        public T GetObject<T>()
        {
            lock (GetLock<T>())
            {
                // check if there is existing
                if (_container.TryGetValue(typeof(T), out object value))
                {
                    return (T)value;
                }
                // if not, instantiate one.
                Func<T> constructor = _constructorContainer[typeof(T)] as Func<T>;
                if (constructStack.Contains(typeof(T)))
                {
                    throw new CyclicDependencyException("Cyclic dependency detected: " + string.Join(" -> ", constructStack.ToArray() as object[]) + " -> " + typeof(T));
                }
                constructStack.Push(typeof(T));
                T newValue = constructor();
                constructStack.Pop();
                _container.Add(typeof(T), newValue);
                return newValue;
            }
        }

        /// <summary>Get the private lock for specific type.<summary/>
        protected object GetLock<T>()
        {
            return _locks.GetOrAdd(typeof(T), _ => new object());
        }

        public class CyclicDependencyException : System.Exception
        {
            public CyclicDependencyException(string message) : base(message) { }
        };
    }
}
