using System;
using System.Collections.Generic;
using UnityEditor;
using Object = UnityEngine.Object;

namespace GraphicsLabor.Scripts.Core.Utility
{
    /// <summary>
    /// Collection of helper methods
    /// </summary>
    public static class GHelpers
    {
        /// <summary>
        /// Concatenates two IEnumerable together and returns the resulting IEnumerable 
        /// </summary>
        /// <param name="listA">First IEnumerable</param>
        /// <param name="listB">Second IEnumerable</param>
        /// <param name="allowDuplicates">If true, will concatenate without checking for duplicate values</param>
        /// <typeparam name="T">The type of data held by the given IEnumerables</typeparam>
        /// <returns></returns>
        public static IEnumerable<T> ConcatenateLists<T>(IEnumerable<T> listA, IEnumerable<T> listB,
            bool allowDuplicates = false)
        {
            List<T> concatenatedList = new List<T>();
            concatenatedList.AddRange(listA);

            if (allowDuplicates)
            {
                concatenatedList.AddRange(listB);
            }
            else
            {
                foreach (T s in listB)
                {
                    if (concatenatedList.Contains(s)) continue;
                    
                    concatenatedList.Add(s);
                }
            }

            return concatenatedList;
        }
        
        public static bool AreKeysEqual<TKey>(TKey key, object otherKey)
        {
            return (object)key == otherKey || key.Equals(otherKey);
        }
        
        public static bool IsKeyValid(object key)
        {
            try
            {
                return !(key == null || (key is Object unityObject && unityObject == null));
            }
            catch
            {
                return false;
            }
        }
      
    }
}