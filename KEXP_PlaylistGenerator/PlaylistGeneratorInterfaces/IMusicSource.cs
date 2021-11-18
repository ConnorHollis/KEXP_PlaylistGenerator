using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PG
{
    public interface IMusicSource
    {
        public IEnumerable<ISong> FindSongs(DateTime searchStart, DateTime searchEnd, string filter);
    }
}
