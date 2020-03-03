using CommandLine;

namespace SievoAssignment
{
    public class EtlOptions
    {
        [Option("file", Required = true, HelpText = "full path to the input file")]
        public string File { get; set; }

        [Option("sortByStartDate", Required = false, HelpText = "sort results by column \"Start date\" in ascending order")]
        public bool SortByStartDate { get; set; }

        [Option("project", Required = false, HelpText = "filter results by column \"Project\"")]
        public string Project { get; set; }
    }
}
