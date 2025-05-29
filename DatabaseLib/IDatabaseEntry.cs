using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Database.SkuDatabase;

namespace Database
{
    public interface IDatabaseEntry<T> : IHasDateRange, ICopyable<T>
    {
    }
}
