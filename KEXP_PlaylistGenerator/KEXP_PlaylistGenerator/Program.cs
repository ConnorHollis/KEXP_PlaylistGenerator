using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using Google.Apis.YouTube.v3;
using Shared;
using Google.Apis.Auth.OAuth2;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using Google.Apis.Util.Store;
using Google.Apis.Services;
using Google.Apis.YouTube.v3.Data;
using System.Text.RegularExpressions;

namespace KEXP
{
    public struct SearchQuery
    {
        public string title;
        public string artist;
    }

    class Program
    {
        private static EmbedIOAuthServer _server;

        static int Main(string[] args)
        {
            return CommandLine.Parser.Default.ParseArguments<SearchOptions>(args)
              .MapResult(
                (SearchOptions opts) => RunSearchAndReturnExitCode(opts),
                errs => 1);
        }

        static int RunSearchAndReturnExitCode(SearchOptions opts)
        {
            List<SearchQuery> songsToQuery = new List<SearchQuery>();

            if (opts.type == OptionsSearchType.Show)
            {
                SearchShows shows = new SearchShows(opts.startDate, opts.endDate, opts.filter);
                shows.PerformSearch();

                List<int> showIDs = new List<int>();
                foreach (Json.Show show in shows.Shows.results)
                {
                    showIDs.Add(show.id);
                }

                SearchSongs songs = new SearchSongs(opts.startDate, opts.endDate, showIDs.ToArray(), null);
                songs.PerformSearch();

                Json.Playlist allSongs = songs.Songs;

                Log.Print("Shows Found");
                Log.Print("=====================================================================");
                foreach (var show in shows.Shows.results)
                {
                    Log.Print(show.program_name);
                    Log.Print("  ID: " + show.id);
                    Log.Print("  Host(s): " + string.Join(", ", show.host_names));
                    Log.Print("  Start: " + show.start_time.ToLongDateString() + " " + show.start_time.ToLongTimeString());
                    Log.Print("  Songs: ");
                    Log.Print("  ------------------------------ ");

                    foreach (var song in allSongs.results.Where(x => x.show == show.id))
                    {
                        Log.Print("     Title:  " + song.song);
                        Log.Print("     Artist: " + song.artist);
                        Log.Print("     Album:  " + song.album);
                        Log.Print(" ");

                        SearchQuery query;
                        query.title = song.song;
                        query.artist = song.artist;
                        songsToQuery.Add(query);
                    }
                    Log.Print("========================================================================");
                }
            }
            else if (opts.type == OptionsSearchType.Song)
            {
                SearchSongs songs = new SearchSongs(opts.startDate, opts.endDate, new int[0], opts.filter);
                songs.PerformSearch();

                Json.Playlist allSongs = songs.Songs;

                Log.Print("Songs Found");
                Log.Print("=====================================================================");
                foreach (var song in allSongs.results)
                {
                    Log.Print(" Title:  " + song.song);
                    Log.Print(" Artist: " + song.artist);
                    Log.Print(" Album:  " + song.album);
                    Log.Print(" ");

                    SearchQuery query;
                    query.title = song.song;
                    query.artist = song.artist;
                    songsToQuery.Add(query);
                }
                Log.Print("========================================================================");

            }

            // kexp_grabber search --date_min=ddd --date_max=ddd --type=show/artist/track/host --textfilter="regex" --output=full/id
            // output: show_ids/artist_ids/host_ids
            //         000011 - show name - start time - end time - # tracks - host
            //         000022 
            // output: artist_ids
            //         005151 - artist_name - # tracks
            //
            // kexp_grabber playlist --date_min=ddd --date_max=ddd --type=show/artist/track/host --showfilter=id --target=youtube

            //YoutubeAuth.Login("930469969410-pd4lj2alsoq00ig1sr65dr45nhriv7hk.apps.googleusercontent.com", "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").Wait();


            /*
            SpotifyClient client = new SpotifyClient("BQBZUC6WGbl8eTf6xFfttoKUYw8QoLANDgzveyffUcxgbcTZZK2gYbEFx3-5Wg-oGbuurlsHTRRuzFWoXUhAppHB0GCxiDZkMJcr9UGtb9m3lZFBqZI1VcMWKd6cCc774ISfCGG7gOib_HgToCTJJ-ZrO-CNpqDFkQ2Sq5j6DEbRq2vmrHczjRsCiDUPYzvIP8HK");


            SearchRequest request = new SearchRequest(SearchRequest.Types.Track, ple.Tracks[0].artist + " " + ple.Tracks[0].trackname);

            var searchResultTask = client.Search.Item(request);
            searchResultTask.Wait();

            var searchResult = searchResultTask.Result;
            */
            // search???

            Program p = new Program();
            p.RunSpotify(songsToQuery).Wait();

            return 0;
        }

        static void HandleParseError(IEnumerable<Error> errs)
        {
            Console.WriteLine("Failed to start KEXP Playlist Generator!");
        }

        private async Task RunSpotify(List<SearchQuery> songs)
        {
            // Make sure "http://localhost:5000/callback" is in your spotify application as redirect uri!
            _server = new EmbedIOAuthServer(new Uri("http://localhost:5000/callback"), 5000);  
            await _server.Start();

            _server.AuthorizationCodeReceived += OnAuthorizationCodeReceived; 
            _server.ErrorReceived += OnErrorReceived;

            var request = new LoginRequest(_server.BaseUri, "73905da1b7b54060a280a3b09e4ce97e", LoginRequest.ResponseType.Code) 
            { 
                Scope = new List<string> { Scopes.PlaylistModifyPrivate, Scopes.PlaylistModifyPublic } 
            }; 
            BrowserUtil.Open(request.ToUri());

            await Task.Delay(100000);
        }

        private static async Task OnAuthorizationCodeReceived(object sender, AuthorizationCodeResponse response)
        {
            await _server.Stop();
            var config = SpotifyClientConfig.CreateDefault();

            var tokenResponse = await new OAuthClient(config).RequestToken(
                new AuthorizationCodeTokenRequest(
                    "73905da1b7b54060a280a3b09e4ce97e",
                    "3d2b27d4533d4d08aaa5f983f3172075", 
                    response.Code, 
                    new Uri("http://localhost:5000/callback")
                )
            );

            var spotify = new SpotifyClient(tokenResponse.AccessToken);

            //SearchRequest request = new SearchRequest(SearchRequest.Types.Track, )
            //spotify.Search.Item()
        }

        private static async Task OnErrorReceived(object sender, string error, string state) 
        { 
            Console.WriteLine($"Aborting authorization, error received: {error}"); 
            await _server.Stop(); 
        }

        private async Task RunYoutube(List<SearchQuery> songs)
        {
            /*
            UserCredential credential;
            using (var stream = new FileStream("client_secrets.json", FileMode.Open, FileAccess.Read))
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    // This OAuth 2.0 access scope allows for full read/write access to the
                    // authenticated user's account.
                    new[] { YouTubeService.Scope.Youtube },
                    "user",
                    CancellationToken.None,
                    new FileDataStore(this.GetType().ToString())
                );
            }

            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = this.GetType().ToString()
            });
            */

            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = "AIzaSyA0u9VIfWgYyjJOz2Dv4xvLeXU7j475n4M",
                ApplicationName = this.GetType().ToString()
            });

            foreach (SearchQuery query in songs)
            {
                Log.Print("Looking for: " + query.artist + " - " + query.title);

                var searchListRequest = youtubeService.Search.List("snippet");
                searchListRequest.Q = query.artist + " " + query.title;
                searchListRequest.MaxResults = 50;

                // Call the search.list method to retrieve results matching the specified query term.
                var searchListResponse = await searchListRequest.ExecuteAsync();

                List<string> videos = new List<string>();

                // Add each result to the appropriate list, and then display the lists of
                // matching videos, channels, and playlists.
                foreach (var searchResult in searchListResponse.Items)
                {
                    switch (searchResult.Id.Kind)
                    {
                        case "youtube#video":
                            Regex regexTitle = new Regex(songs[0].title, RegexOptions.IgnoreCase);
                            Regex regexArtist = new Regex(songs[0].artist, RegexOptions.IgnoreCase);

                            if (regexTitle.IsMatch(searchResult.Snippet.Title) || regexArtist.IsMatch(searchResult.Snippet.Title))
                            {
                                videos.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.VideoId));
                            }

                            break;
                    }
                }

                Console.WriteLine(String.Format("Videos:\n{0}\n", string.Join("\n", videos)));
                await Task.Delay(1000);
            }

            /*
            var newPlaylist = new Playlist();
            newPlaylist.Snippet = new PlaylistSnippet();
            newPlaylist.Snippet.Title = "Test Playlist";
            newPlaylist.Snippet.Description = "A playlist created with the YouTube API v3";
            newPlaylist.Status = new PlaylistStatus();
            newPlaylist.Status.PrivacyStatus = "public";
            newPlaylist = await youtubeService.Playlists.Insert(newPlaylist, "snippet,status").ExecuteAsync();

            // Add a video to the newly created playlist.
            var newPlaylistItem = new PlaylistItem();
            newPlaylistItem.Snippet = new PlaylistItemSnippet();
            newPlaylistItem.Snippet.PlaylistId = newPlaylist.Id;
            newPlaylistItem.Snippet.ResourceId = new ResourceId();
            newPlaylistItem.Snippet.ResourceId.Kind = "youtube#video";
            newPlaylistItem.Snippet.ResourceId.VideoId = "GNRMeaz6QRI";
            newPlaylistItem = await youtubeService.PlaylistItems.Insert(newPlaylistItem, "snippet").ExecuteAsync();
            
            Console.WriteLine("Playlist item id {0} was added to playlist id {1}.", newPlaylistItem.Id, newPlaylist.Id);
            */
        }
    }
}
