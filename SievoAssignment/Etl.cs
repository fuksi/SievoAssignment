using CommandLine;
using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace SievoAssignment
{
    public class Etl
    {
        private ISievoLogger _sievoLogger;
        private string[] _headerFields = new string[]
        {
            "Project",
            "Description",
            "Start date",
            "Category",
            "Responsible",
            "Savings amount",
            "Currency",
            "Complexity"
        };
        private string[] _allowedComplexities = new string[]
        {
            "Simple",
            "Moderate",
            "Hazardous"
        };

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

                    if (_headerFields.Any(headerField => line.StartsWith(headerField)))
                    {
                        csv.ReadHeader();
                        _sievoLogger.Info(line);
                        continue;
                    }

                    var headerColumns = csv.Context.HeaderRecord;
                    var valuesOrderedSameAsHeaders = headerColumns
                        .Select(columnName =>
                        {
                            var cellValue = csv.GetField<string>(columnName);

                            if ((columnName == "Savings amount" || columnName == "Currency")
                                && cellValue == "NULL")
                            {
                                cellValue = "";
                            }

                            if (columnName == "Complexity" && !_allowedComplexities.Contains(cellValue))
                            {
                                throw new ArgumentException("");
                            }

                            return cellValue;
                        }).ToArray();

                    _sievoLogger.Info(valuesOrderedSameAsHeaders);
                }
            });
        }
    }
}
