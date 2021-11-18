using Newtonsoft.Json;
using RestEase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace KEXP
{
    public class SearchSongs
    {
        private DateTime m_startTime;
        private DateTime m_endTime;
        private List<int> m_showIDs = new List<int>();
        private string m_filter;
        private int m_requestCount = 20;
        private Json.Playlist m_songs;

        public Json.Playlist Songs
        {
            get { return m_songs; }
        }

        public SearchSongs(DateTime startTime, DateTime endTime, int[] showIDs, string filter)
        {
            m_startTime = startTime;
            m_endTime = endTime;
            m_showIDs.AddRange(showIDs);
            m_filter = filter;
        }

        public void PerformSearch()
        {
            string startTimeString = XmlConvert.ToString(m_startTime, XmlDateTimeSerializationMode.Unspecified);
            string endTimeString = XmlConvert.ToString(m_endTime, XmlDateTimeSerializationMode.Unspecified);

            m_songs = new Json.Playlist();

            if (m_showIDs.Count > 0)
            {
                foreach (int showId in m_showIDs)
                {
                    SearchInternal(startTimeString, endTimeString, showId.ToString());
                }
            }
            else
            {
                SearchInternal(startTimeString, endTimeString, null);
            }

            m_songs.results.Reverse();
        }

        private void SearchInternal(string start, string end, string showid)
        {
            var settings = new JsonSerializerSettings()
            {
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            IKEXPApi api = new RestClient("https://api.kexp.org/v2")
            {
                JsonSerializerSettings = settings
            }.For<IKEXPApi>();

            bool has_results = true;
            int offset = 0;

            while (has_results)
            {
                Json.Playlist songs_in_loop = api.GetPlaysAsync(start, end, true, showid, offset).Result;

                if (m_filter != null)
                {
                    Regex regex = new Regex(m_filter, RegexOptions.IgnoreCase);
                    m_songs.results.AddRange(songs_in_loop.results.Where(x => regex.IsMatch(x.song) || regex.IsMatch(x.album) || regex.IsMatch(x.artist)));
                }
                else
                {
                    m_songs.results.AddRange(songs_in_loop.results);
                }

                offset += m_requestCount;
                has_results = songs_in_loop.results.Count >= m_requestCount;
            }
        }
    }
}
