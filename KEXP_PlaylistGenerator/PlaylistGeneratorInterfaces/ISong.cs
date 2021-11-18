using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PG
{
    public interface ISong
    {
        public abstract string GetSongTitle();
        public abstract string GetSongArtist();
    }
}
