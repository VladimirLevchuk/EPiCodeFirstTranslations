using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading;

namespace Creuna.EPiCodeFirstTranslations.Utils
{
    // TODO romanm: think about creating separate caching lib buget package and use it.

    /// <summary>
    /// Simple memory-based cache with statistics and fixed (non-sliding) record lifetime configurable from the config file. 
    /// </summary>
    public class SimpleMemoryCache
    {
        private MemoryCache m_Cache;

        private readonly string m_Name;
        private readonly HashSet<string> m_Keys = new HashSet<string>();

        private readonly object SyncRoot = new object();

        private int m_Hits = 0;
        private int m_Reads = 0;

        const int DefaultLifetime = 60;

        public virtual string Name
        {
            get { return m_Name; }
        }

        public virtual int Hits
        {
            get { return m_Hits; }
        }

        public virtual int Reads
        {
            get { return m_Reads; }
        }

        public virtual bool Debug
        {
#if DEBUG
            get { return true; }
#else
            get { return false; }
#endif
        }

        public virtual HashSet<string> Keys
        {
            get { return m_Keys; }
        }

        protected virtual MemoryCache Cache { get { return m_Cache; } }

        public virtual string ConfigurationKeyName
        {
            get { return string.Format("Cache.{0}.LifetimeInSeconds", Name); }
        }

        public virtual int RecordLifetimeInSeconds
        {
            get
            {
                var lifetime = ConfigurationManager.AppSettings[ConfigurationKeyName] ?? DefaultLifetime.ToString(CultureInfo.InvariantCulture);
                return int.Parse(lifetime, CultureInfo.InvariantCulture);
            }
        }

        public SimpleMemoryCache(string name)
        {
            m_Name = name ?? GetType().AssemblyQualifiedName;
            if (m_Name.Length > 60)
            {
                m_Name = m_Name.Substring(0, 57) + "...";
            }

            m_Cache = new MemoryCache(m_Name);
        }

        public override string ToString()
        {
            return string.Format("Name: '{0}', Size: {1}, Hits: {2}, Reads: {3}, Hit Rate: {4}, Tracked Keys: {5}"
                , Name
                , m_Cache.GetCount()
                , m_Hits
                , m_Reads
                , Math.Round(100.0 * m_Hits / m_Reads)
                , Keys.Count);
        }

        public virtual TItem GetOrLoad<TItem>(string key, Func<TItem> loader)
        {
            Interlocked.Increment(ref m_Reads);
            var cached = Cache.Get(key);

            if (cached != null)
            {
                Interlocked.Increment(ref m_Hits);
                return (TItem)cached;
            }

            var result = loader();

            lock (SyncRoot)
            {
                var policy = new CacheItemPolicy();
                policy.RemovedCallback += arguments =>
                {
                    lock (SyncRoot)
                    {
                        Keys.Remove(arguments.CacheItem.Key);
                    }

                    if (Debug)
                    {
                        System.Diagnostics.Debug.WriteLine("[Cache][debug][{1}] Key removed: {0} [state: {2}]", arguments.CacheItem.Key, Name, ToString());
                    }
                };

                policy.AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(RecordLifetimeInSeconds);
                // policy.ChangeMonitors.Add();

                Cache.Add(key, result, policy);
                Keys.Add(key);
                if (Debug)
                {
                    System.Diagnostics.Debug.WriteLine("[Cache][debug][{1}] Key added: {0} (={2}) [state: {3}]", key, Name, result, ToString());
                }
            }

            return result;
        }

        public virtual void RemoveKey(string key)
        {
            lock (SyncRoot)
            {
                Cache.Remove(key);
            }
        }

        public virtual void Clear()
        {
            lock (SyncRoot)
            {
                m_Cache = new MemoryCache(Name);
                Keys.Clear();
            }
        }

        public virtual void ClearKeys(Func<string, bool> predicate)
        {
            lock (SyncRoot)
            {
                var keysToRemove = Keys.Where(predicate).ToList();
                keysToRemove.ForEach(key => Cache.Remove(key));
            }
        }
    }
}
