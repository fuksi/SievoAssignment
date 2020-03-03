using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.IO;

namespace SievoAssignment
{
    public class Etl
    {
        public void Execute(EtlOptions options)
        {
            using var reader = new StreamReader(options.File);
            using var csv = new CsvReader(reader, 
                new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = "\t"});

            while (csv.Read())
            {

            }
        }
    }
}
