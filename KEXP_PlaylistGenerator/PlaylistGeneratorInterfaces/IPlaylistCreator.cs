using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PG
{
    public interface IPlaylistCreator
    {
        public abstract void Login();
        public abstract void SearchForSongs(IEnumerable<ISong> songs);
        public abstract void LogFoundElements(ILogger logger);
        public abstract void CreatePlaylist();
    }
}
