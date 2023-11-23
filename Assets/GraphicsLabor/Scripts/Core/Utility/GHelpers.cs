using System.Collections.Generic;

namespace GraphicsLabor.Scripts.Core.Utility
{
    public static class GHelpers
    {
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
    }
}