using CommandLine;
using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace SievoAssignment
{
    public class Etl
    {
        private ISievoLogger _sievoLogger;
        private string[] _orderedHeaderFields;
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

                List<string[]> rowsContainer = new List<string[]>();
                while (csv.Read())
                {
                    // Skip empty lines or comments
                    var line = csv.Context.RawRecord;
                    if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
                    {
                        continue;
                    }

                    // Output header row
                    if (_headerFields.Any(headerField => line.StartsWith(headerField)))
                    {
                        csv.ReadHeader();
                        _sievoLogger.Info(line);
                        continue;
                    }

                    if (!string.IsNullOrEmpty(opt.Project)
                        && csv.GetField("Project") != opt.Project)
                    {
                        continue;
                    }

                    _orderedHeaderFields = csv.Context.HeaderRecord;
                    var rowValuesOrderedSameAsHeaders = _orderedHeaderFields
                        .Select(columnName =>
                        {
                            var cellValue = csv.GetField<string>(columnName);

                            if (columnName == "Savings amount")
                            {
                                if (cellValue == "NULL")
                                {
                                    cellValue = "";
                                } 
                                else
                                {
                                    decimal.Parse(cellValue, CultureInfo.InvariantCulture);
                                }
                            }

                            if (columnName == "Currency" && cellValue == "NULL")
                            {
                                cellValue = "";
                            }

                            if (columnName == "Complexity" && !_allowedComplexities.Contains(cellValue))
                            {
                                throw new ArgumentException(@$"'{cellValue}' is an invalid complexity value. Allowed values are {string.Join(', ', _allowedComplexities)}");
                            }

                            if (columnName == "Start date")
                            {
                                DateTime.Parse(cellValue);
                            }

                            return cellValue;
                        }).ToArray();

                    if (opt.SortByStartDate)
                    {
                        rowsContainer.Add(rowValuesOrderedSameAsHeaders);
                    } 
                    else
                    {
                        _sievoLogger.Info(rowValuesOrderedSameAsHeaders);
                    }
                }

                if (opt.SortByStartDate)
                {
                    var startDateColumnIndex = Array.IndexOf(_orderedHeaderFields, "Start date");
                    rowsContainer.Sort((a, b) =>
                    {
                        var aStartDate = DateTime.Parse(a[startDateColumnIndex]);
                        var bStartDate = DateTime.Parse(b[startDateColumnIndex]);
                        return aStartDate < bStartDate ? -1 : 1;
                    });

                    rowsContainer.ForEach(r => _sievoLogger.Info(r));
                }
            });
        }
    }
}
