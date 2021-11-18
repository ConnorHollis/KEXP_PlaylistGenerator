using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PG
{
    public interface IFoundSong
    {
        public abstract string GetTitle();
        public abstract string GetDescription();
        public abstract string GetUniqueID();
    }
}
