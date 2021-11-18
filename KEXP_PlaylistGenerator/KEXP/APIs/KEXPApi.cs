using System.Threading.Tasks;
using Newtonsoft.Json;
using RestEase;

namespace KEXP
{
    public interface IKEXPApi
    {
        [Get("shows")]
        Task<Json.ShowList> GetShowsAsync([Query] string start_time_after, [Query] string start_time_before, [Query] int offset);

        [Get("plays")]
        Task<Json.Playlist> GetPlaysAsync([Query] string airdate_after, [Query] string airdate_before, [Query] bool exclude_airbreaks, [Query] string show_ids, [Query] int offset);
    }
}
