using System.Collections.Generic;

namespace MnistConverter
{
    public class CounterList<T>
    {
        Dictionary<T, int> counterListDictionary = new Dictionary<T, int>();

        public void Put(T iObject)
        {
            if (counterListDictionary.ContainsKey(iObject))
                counterListDictionary[iObject] = counterListDictionary[iObject] + 1;
            else
                counterListDictionary.Add(iObject, 1);
        }

        public int GetValue(T iObject)
        {
            if (counterListDictionary.ContainsKey(iObject))
                return counterListDictionary[iObject];
            else
                return 0;
        }
    }
}
