using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Master
{
    [Serializable]
    public class MasterElementArray<T>
    {
        public T[] masterArray;
    }


    [Serializable]
    public class MasterElementCollection<TKey, TValue> : ScriptableObject where TValue : MasterElement<TKey>
    {
        [SerializeField]
        protected List<TValue> masterList;

        protected Dictionary<TKey, TValue> masterDictionary;
        protected bool ready = false;

        public virtual void Init()
        {
            masterDictionary = new Dictionary<TKey, TValue>();
            foreach (var data in masterList) {
                if (masterDictionary.ContainsKey(data.id)) {
                    continue;
                }
                masterDictionary.Add(data.id, data);
            }
        }

        public virtual TValue Get(TKey key)
        {
            if (masterDictionary.ContainsKey(key)) {
                return masterDictionary[key];
            }
            Debug.LogWarning(string.Format("{0} not found", key));
            return null;
        }

        public static T LoadFromJson<T>(T instance, string jsonString) where T : MasterElementCollection<TKey, TValue>
        {
            var wrapper = JsonUtility.FromJson<MasterElementArray<TValue>>(jsonString);
            var master = instance;
            master.masterList = new List<TValue>();
            master.masterDictionary = new Dictionary<TKey, TValue>();
            foreach (var data in wrapper.masterArray) {
                if (master.masterDictionary.ContainsKey(data.id)) {
                    Debug.LogError("key duplicated:" + data.id);
                    continue;
                }
                master.masterDictionary.Add(data.id, data);
                master.masterList.Add(data);
            }
            return master;
        }
    }
}
