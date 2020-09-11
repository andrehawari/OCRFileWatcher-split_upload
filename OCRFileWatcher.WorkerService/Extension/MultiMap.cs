using System;
using System.Collections.Generic;
using System.Text;

namespace OCRFileWatcher.WorkerService.Extension
{
    public class MultiMap<V>
    {
        Dictionary<string, List<V>> _dictionary =
                        new Dictionary<string, List<V>>();

        public void Add(string key, V value)
        {
            // Add a key.
            List<V> list;
            if (this._dictionary.TryGetValue(key, out list))
            {
                list.Add(value);
            }
            else
            {
                list = new List<V>();
                list.Add(value);
                this._dictionary[key] = list;
            }
        }

        public IEnumerable<string> Keys
        {
            get
            {
                // Get all keys.
                return this._dictionary.Keys;
            }
        }

        public List<V> this[string key]
        {
            get
            {
                // Get list at a key.
                List<V> list;
                if (!this._dictionary.TryGetValue(key, out list))
                {
                    list = new List<V>();
                    this._dictionary[key] = list;
                }
                return list;
            }
        }


    }
}
