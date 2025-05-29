using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database
{
    public interface IHasDateRange
    {
        public DateTime StartDate();
        public DateTime? EndDate();
        public void SetStart(DateTime start);
        public void SetEnd(DateTime? end);
    }
}
