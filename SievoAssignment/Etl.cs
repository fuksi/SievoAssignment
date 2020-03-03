using CommandLine;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.IO;

namespace SievoAssignment
{
    public class Etl
    {
        public void Execute(string[] args)
        {
            Parser.Default.ParseArguments<EtlOptions>(args).WithParsed(opt =>
            {
                var etl = new Etl();
                using var reader = new StreamReader(opt.File);
                using var csv = new CsvReader(reader,
                    new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = "\t" });

                while (csv.Read())
                {
                    var line = csv.Context.RawRecord;
                    if (string.IsNullOrEmpty(line))
                    {
                        continue;
                    }

                    // Delegate output to another component so we test down stream
                    if (line.StartsWith("#"))
                    {
                        // Do sth with header line    
                        continue;
                    }

                    // Process as row
                }
            });
        }
    }
}
