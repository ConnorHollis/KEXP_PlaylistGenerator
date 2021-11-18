using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestEase;
using PG;

namespace KEXP
{
    public class KEXPMusicSource : IMusicSource
    {
        public IEnumerable<ISong> FindSongs(DateTime searchStart, DateTime searchEnd, string filter)
        {
            SearchShows showSearcher = new SearchShows(searchStart, searchEnd, filter);
            showSearcher.PerformSearch();

            SearchSongs songSearcher = new SearchSongs(searchStart, searchEnd, showSearcher.Shows.results.Select(x => x.id).ToArray(), null);
            songSearcher.PerformSearch();

            return songSearcher.Songs.results.Select(x => new KEXPSong(x.song, x.artist));
        }
    }
}
