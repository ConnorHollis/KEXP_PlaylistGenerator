using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using RestEase;

namespace KEXP
{
    public class DateProgramExtractor
    {
        private string m_date;
        private string m_programName;
        private int m_showID;

        public int ShowID
        {
            get { return m_showID; }
        }

        public DateProgramExtractor(string date, string program)
        {
            m_date = date;
            m_programName = program.ToLower();

            FindMatchingProgram();
        }

        private Dictionary<string, int> GetProgramsForDayOfWeek(string day)
        {
            Dictionary<string, int> results = new Dictionary<string, int>();

            DateTime startTime = DateTime.Parse(m_date).AddHours(-1);
            DateTime endTime = DateTime.Parse(m_date).AddHours(25);

            string startTimeString = XmlConvert.ToString(startTime, XmlDateTimeSerializationMode.Unspecified);
            string endTimeString = XmlConvert.ToString(endTime, XmlDateTimeSerializationMode.Unspecified);

            var settings = new JsonSerializerSettings()
            {
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            IKEXPApi api = new RestClient("https://api.kexp.org/v2")
            {
                JsonSerializerSettings = settings
            }.For<IKEXPApi>();
            Json.ShowList shows = api.GetShowsAsync(startTimeString, endTimeString, 20).Result;

            foreach(var show in shows.results)
            {
                string prog_name = show.program_name.ToLower();
                if (!results.ContainsKey(prog_name))
                {
                    results.Add(prog_name, show.id);
                }
            }

            return results;
        }

        private void FindMatchingProgram()
        {
            DateTime dateValue = DateTime.Parse(m_date);
            string day = dateValue.DayOfWeek.ToString().ToLower();

            Dictionary<string, int> allProgramsForDay = GetProgramsForDayOfWeek(day);
            m_showID = allProgramsForDay.ContainsKey(m_programName) ? allProgramsForDay[m_programName] : 0;
        }
    }
}
