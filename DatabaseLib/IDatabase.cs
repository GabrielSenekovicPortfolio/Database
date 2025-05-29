using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Database.SkuDatabase;

namespace Database
{
    public interface IDatabase<Key, Value, T> where T : IDatabaseEntry<T>
    {
        public bool GetAllKeys(out List<Key> keys);
        public IEnumerable<T>? GetAllValues();
    }
}
