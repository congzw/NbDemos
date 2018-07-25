using System;
using System.Collections;
using System.Collections.Generic;

namespace ZQNB.Common
{
    public class Singleton
    {
        private static readonly IDictionary<Type, object> _allSingletons;

        static Singleton()
        {
            _allSingletons = new Dictionary<Type, object>();
        }

        public static IDictionary<Type, object> AllSingletons
        {
            get { return _allSingletons; }
        }
    }

    /// <summary>
    /// 为了使用方便，定义了此类。用以包装Singleton.AllSingletons[typeof(T)]
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Singleton<T> : Singleton
    {
        static T _instance;

        public static T Instance
        {
            get { return _instance; }
            set
            {
                _instance = value;
                AllSingletons[typeof(T)] = value;
            }
        }
    }

    /// <summary>
    /// 为了使用方便，定义了此类。用以包装Singleton[IList[T]]
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SingletonList<T> : Singleton<IList<T>>, IList<T>
    {
        static SingletonList()
        {
            Singleton<IList<T>>.Instance = new List<T>();
        }

        public new static IList<T> Instance
        {
            get { return Singleton<IList<T>>.Instance; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Instance.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            Instance.Add(item);
        }

        public void Clear()
        {
            Instance.Clear();
        }

        public bool Contains(T item)
        {
            return Instance.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Instance.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return Instance.Remove(item);
        }

        public int Count {
            get { return Instance.Count; }
        }
        public bool IsReadOnly {
            get { return Instance.IsReadOnly; }
        }
        public int IndexOf(T item)
        {
            return Instance.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            Instance.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            Instance.RemoveAt(index);
        }

        public T this[int index]
        {
            get { return Instance[index]; }
            set { Instance[index] = value; }
        }
    }

    /// <summary>
    /// 为了使用方便，定义了此类。用以包装SingletonDictionary[TKey, TValue]
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class SingletonDictionary<TKey, TValue> : Singleton<IDictionary<TKey, TValue>>
    {
        static SingletonDictionary()
        {
            Singleton<Dictionary<TKey, TValue>>.Instance = new Dictionary<TKey, TValue>();
        }

        /// <summary>
        /// 单例
        /// </summary>
        public new static IDictionary<TKey, TValue> Instance
        {
            get { return Singleton<Dictionary<TKey, TValue>>.Instance; }
        }
    }
}
