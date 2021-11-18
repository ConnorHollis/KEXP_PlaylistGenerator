using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PG;

namespace KEXP
{
    public class KEXPSong : ISong
    {
        string m_title;
        string m_artist;

        public KEXPSong(string title, string artist)
        {
            m_title = title;
            m_artist = artist;
        }

        public string GetSongArtist()
        {
            return m_artist;
        }

        public string GetSongTitle()
        {
            return m_title;
        }
    }
}
