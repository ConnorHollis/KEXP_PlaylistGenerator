using Newtonsoft.Json;
using RestEase;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace KEXP
{
    public class SearchShows
    {
        private DateTime m_startTime;
        private DateTime m_endTime;
        private string m_filter;
        private int m_requestCount = 20;
        private Json.ShowList m_shows;

        public Json.ShowList Shows
        {
            get { return m_shows; }
        }
            
        public SearchShows(DateTime startTime, DateTime endTime, string filter)
        {
            m_startTime = startTime;
            m_endTime = endTime;
            m_filter = filter;
        }

        public void PerformSearch()
        {
            string startTimeString = XmlConvert.ToString(m_startTime, XmlDateTimeSerializationMode.Unspecified);
            string endTimeString = XmlConvert.ToString(m_endTime, XmlDateTimeSerializationMode.Unspecified);

            var settings = new JsonSerializerSettings()
            {
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            IKEXPApi api = new RestClient("https://api.kexp.org/v2")
            {
                JsonSerializerSettings = settings
            }.For<IKEXPApi>();

            m_shows = new Json.ShowList();
            bool has_results = true;
            int offset = 0;

            while (has_results)
            {
                Json.ShowList loop_shows = api.GetShowsAsync(startTimeString, endTimeString, offset).Result;

                if (m_filter != null)
                {
                    Regex regex = new Regex(m_filter, RegexOptions.IgnoreCase);
                    m_shows.results.AddRange(loop_shows.results.Where(x => regex.IsMatch(x.program_name)));
                }
                else
                {
                    m_shows.results.AddRange(loop_shows.results);
                }

                offset += m_requestCount;
                has_results = loop_shows.results.Count >= m_requestCount;
            }

            m_shows.results.Reverse();
        }
    }
}
