using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace KEXP.Json
{
    public class ShowList
    {
        [JsonProperty("results")]
        public List<Show> results = new List<Show>();
    }

    public class Show
    {
        [JsonProperty("id")]
        public int id;

        [JsonProperty("uri")]
        public string uri;

        [JsonProperty("program")]
        public string program;

        [JsonProperty("program_uri")]
        public string program_uri;

        [JsonProperty("hosts")]
        public int[] hosts;

        [JsonProperty("host_uris")]
        public string[] host_uris;

        [JsonProperty("program_name")]
        public string program_name;

        [JsonProperty("program_tags")]
        public string program_tags;

        [JsonProperty("host_names")]
        public string[] host_names;

        [JsonProperty("tagline")]
        public string tagline;

        [JsonProperty("image_url")]
        public string image_url;

        [JsonProperty("start_time")]
        public DateTime start_time;
    }

    public class Playlist
    {
        [JsonProperty("results")]
        public List<Track> results = new List<Track>();
    }

    public class Track
    {
        [JsonProperty("id")]
        public int id;

        [JsonProperty("uri")]
        public string uri;

        [JsonProperty("airdate")]
        public DateTime airdate;

        [JsonProperty("show")]
        public int show;

        [JsonProperty("show_uri")]
        public string show_uri;

        [JsonProperty("image_uri")]
        public string image_uri;

        [JsonProperty("thumbnail_uri")]
        public string thumbnail_uri;

        [JsonProperty("song")]
        public string song;

        [JsonProperty("track_id")]
        public string track_id;

        [JsonProperty("recording_id")]
        public string recording_id;

        [JsonProperty("artist")]
        public string artist;

        [JsonProperty("artist_ids")]
        public string[] artist_ids;

        [JsonProperty("album")]
        public string album;

        [JsonProperty("release_id")]
        public string release_id;

        [JsonProperty("release_group_id")]
        public string release_group_id;

        [JsonProperty("labels")]
        public string[] labels;

        [JsonProperty("label_ids")]
        public string[] label_ids;

        [JsonProperty("release_date")]
        public string release_date;

        [JsonProperty("rotation_status")]
        public string rotation_status;

        [JsonProperty("is_local")]
        public bool is_local;

        [JsonProperty("is_request")]
        public bool is_request;

        [JsonProperty("is_live")]
        public bool is_live;

        [JsonProperty("comment")]
        public string comment;

        [JsonProperty("play_type")]
        public string play_type;
    }
}
