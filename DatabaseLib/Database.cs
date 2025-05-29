using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Database.SkuDatabase;

namespace Database
{
    public abstract class Database<Key, Value, T> : Dictionary<Key, Value>, IDatabase<Key, Value, T>
        where Key : notnull
        where T : IDatabaseEntry<T>
    {
        public Database(Dictionary<Key, Value> dictionary) : base(dictionary){}
        public bool GetAllKeys(out List<Key> keys)
        {
            keys = Keys.ToList();
            return keys.Count != 0;
        }
        public IEnumerable<T>? GetAllValues()
        {
             var valueList = Values.ToList();

             List<IEnumerable<T>>? list = valueList.Cast<IEnumerable<T>>().ToList();
             if (list != null && list.Any())
             { //If Value is a list of IDatabaseEntry
                 return list.SelectMany(entryList => entryList).ToList();
             }
             else if(valueList.Any() && valueList[0] is IDatabaseEntry<T>)
             { //Otherwise, Value is the IDatabaseEntry
                 return valueList.Cast<T>().ToList();
             }
             else
             {
                 //If it doesn't inherit from IDatabaseEntry, there's an error
                 return null;
             }
        }
    }
}
