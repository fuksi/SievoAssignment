using CsvHelper;
using CsvHelper.Configuration;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace SievoAssignment.Tests.Unit
{
    public class EtlTests
    {
        private string _testDataPath;
        private string _testDataWithInvalidValuePath;
        private Etl _target;
        private Mock<ISievoLogger> _sievoLoggerMock;
        private string[] _headerFields;


        [SetUp]
        public void Setup()
        {
            _testDataPath = Path.Join(Directory.GetCurrentDirectory(), "ExampleData.tsv");
            _testDataWithInvalidValuePath = Path.Join(Directory.GetCurrentDirectory(), "ExampleDataWithInvalidValue.tsv");
            _sievoLoggerMock = new Mock<ISievoLogger>();
            _target = new Etl(_sievoLoggerMock.Object);

            // Get headers and its order to locate target column during tests
            using var reader = new StreamReader(_testDataPath);
            using var csv = new CsvReader(reader,
                new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = "\t" });
            while (csv.Read())
            {
                var line = csv.Context.RawRecord;
                if (line.StartsWith("Project"))
                {
                    csv.ReadHeader();
                    _headerFields = csv.Context.HeaderRecord;
                    break;
                }
            }
        }


        [Test]
        public void Execute_ValidArgs_WriteToOutput()
        {
            var args = new string[] { "--file", _testDataPath };
            _target.Execute(args);

            // Assert comments or empty string NOT in output
            _sievoLoggerMock.Verify(
                m => m.Info(It.Is<string>(msg => string.IsNullOrEmpty(msg) || msg.StartsWith("#"))), 
                Times.Never());

            // Assert header row in output
            _sievoLoggerMock.Verify(m => m.Info(It.Is<string>(msg => msg.StartsWith("Project"))), 
                Times.Once());

            // Assert NULL value for Savings amount, Currency NOT in output
            var savingsAmountIdx = Array.IndexOf(_headerFields, "Savings amount");
            var currencyIdx = Array.IndexOf(_headerFields, "Currency");
            _sievoLoggerMock.Verify(
                m => m.Info(It.Is<string[]>(parts => parts[savingsAmountIdx] == "NULL" || parts[currencyIdx] == "NULL")), 
                Times.Never());
        }

        [Test]
        public void Execute_ValidArgsButInvalidCellValues_ThrowsArgumentException()
        {
            var args = new string[] { "--file", _testDataWithInvalidValuePath };
            var ex = Assert.Throws<ArgumentException>(() => _target.Execute(args));
            Assert.That(ex.Message.Contains("complexity"));
        }
    }
}