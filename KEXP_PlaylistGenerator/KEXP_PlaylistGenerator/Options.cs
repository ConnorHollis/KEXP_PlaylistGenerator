using System;
using CommandLine;

namespace KEXP
{
    // kexp_grabber search --date_min=ddd --date_max=ddd --type=show/artist/track/host --textfilter="regex" --output=full/id
    // output: show_ids/artist_ids/host_ids
    //         000011 - show name - start time - end time - # tracks - host
    //         000022 
    // output: artist_ids
    //         005151 - artist_name - # tracks
    //
    // kexp_grabber playlist --date_min=ddd --date_max=ddd --type=show/artist/track/host --showfilter=id --target=youtube

    public enum OptionsSearchType
    {
        Show,
        Song
    }

    public enum OptionsOutputType
    {
        Full,
        ID
    }

    [Verb("search", HelpText = "Finds shows, tracks, artists, and hosts within a given time range.")]
    public class SearchOptions
    {
        [Option('s', "start", Required = true, HelpText = "The start time for the search. Format is in ISO8601, https://timestampgenerator.com/ EX: -s \"2021-11-14T12:00:54-08:00\" is November 14th 2021 at noon and 54 seconds in PST, hence the -8")]
        public DateTime startDate { get; set; }

        [Option('e', "end", Required = true, HelpText = "The end time for the search. Format is in ISO8601, https://timestampgenerator.com/ EX: -s \"2021-11-14T12:00:54-08:00\" is November 14th 2021 at noon and 54 seconds in PST, hence the -8")]
        public DateTime endDate { get; set; }

        [Option('t', "type", Required = true, HelpText = "What are we looking for? Options are: host/show/song")]
        public OptionsSearchType type { get; set; }

        [Option('f', "filter", Required = false, HelpText = "Text filter for searches this is a regex expression and will be compared against every applicable field. Shows will compare title, hosts will compare name, songs will compare name, album, and artist.")]
        public string filter { get; set; }

        [Option('m', "musicsource", Required = false, HelpText = "What music source are we using to find tracks? (KEXP is the only option so far)")]
        public string generator { get; set; }

        [Option('o', "output", Required = true, HelpText = "The level of verbosity for output Full/ID.")]
        public OptionsOutputType output { get; set; }
    }
}
