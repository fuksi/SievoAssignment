using CommandLine;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.IO;

namespace SievoAssignment
{
    public class Etl
    {
        private ISievoLogger _sievoLogger;

        public Etl(ISievoLogger sievoLogger)
        {
            _sievoLogger = sievoLogger;
        }

        public void Execute(string[] args)
        {
            Parser.Default.ParseArguments<EtlOptions>(args).WithParsed(opt =>
            {
                using var reader = new StreamReader(opt.File);
                using var csv = new CsvReader(reader,
                    new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = "\t" });

                while (csv.Read())
                {
                    var line = csv.Context.RawRecord;
                    if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
                    {
                        continue;
                    }

                    if (line.StartsWith("Project"))
                    {
                        _sievoLogger.Info(line);
                        continue;
                    }

                    // Process as row
                }
            });
        }
    }
}
