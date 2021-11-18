using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PG
{
    public class PlaylistGenerator
    {
        IMusicSource m_musicSource;
        IPlaylistCreator m_playlistCreator;

        public PlaylistGenerator(IMusicSource musicSource, IPlaylistCreator playlistCreator) =>
            (m_musicSource, m_playlistCreator) = (musicSource, playlistCreator);

        public IEnumerable<ISong> Search(DateTime start, DateTime end, string filter)
        {
            IEnumerable<ISong> songs = m_musicSource.FindSongs(start, end, filter);
            return songs;
        }

        public void CreatePlaylist(DateTime start, DateTime end, string filter, string playlistName)
        {
            //m_playlistCreator.Login
        }
    }
}
