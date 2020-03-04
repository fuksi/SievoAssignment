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
        private string _testDataWithInvalidComplexity;
        private string _testDataWithInvalidStartDate;
        private string _testDataWithInvalidSavingsAmount;
        private Etl _target;
        private Mock<ISievoLogger> _sievoLoggerMock;
        private string[] _headerFields;


        [SetUp]
        public void Setup()
        {
            _testDataPath = Path.Join(Directory.GetCurrentDirectory(), "ExampleData.tsv");
            _testDataWithInvalidComplexity = Path.Join(Directory.GetCurrentDirectory(), "ExampleDataWithInvalidComplexity.tsv");
            _testDataWithInvalidStartDate = Path.Join(Directory.GetCurrentDirectory(), "ExampleDataWithInvalidStartDate.tsv");
            _testDataWithInvalidSavingsAmount = Path.Join(Directory.GetCurrentDirectory(), "ExampleDataWithInvalidSavingsAmount.tsv");
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
        public void Execute_ValidFilePath_WriteToOutput()
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
        public void Execute_ValidFilePathAndSortByStartDateAsc_WriteToOutputWithAscOrder()
        {
            // MockBehavior strict allows us to verify order of calls
            _sievoLoggerMock = new Mock<ISievoLogger>(MockBehavior.Strict);
            _target = new Etl(_sievoLoggerMock.Object);

            // MockBehavior requires setup for all invocation
            // we'll need 9 invocations for the current test data
            _sievoLoggerMock.Setup(x => x.Info(It.IsAny<string>()));
            for (var i = 0; i < 8; i++)
            {
                _sievoLoggerMock.Setup(x => x.Info(It.IsAny<string[]>()));
            }

            var args = new string[] { "--file", _testDataPath, "--sortByStartDate" };
            _target.Execute(args);

            // Verify header row output to follow strict invovation order
            _sievoLoggerMock.Verify(x => x.Info(It.Is<string>(msg => msg.Contains("Project"))));

            // Verify the rest 
            var expectedDatesInAscOrder = new List<string> { 
                "2012-06-01",
                "2012-06-01",
                "2013-01-01",
                "2013-01-01",
                "2013-01-01",
                "2013-01-01",
                "2013-04-01",
                "2014-01-01",
            };

            expectedDatesInAscOrder.ForEach(dateStr =>
            {
                _sievoLoggerMock.Verify(x => x.Info(It.Is<string[]>(parts => string.Join(',', parts).Contains(dateStr))));
            });
        }

        [Test]
        public void Execute_ValidArgsButInvalidCellValues_ThrowsArgumentException()
        {
            var args = new string[] { "--file", _testDataWithInvalidComplexity };
            var ex = Assert.Throws<ArgumentException>(() => _target.Execute(args));
            Assert.That(ex.Message.Contains("complexity"));

            args = new string[] { "--file", _testDataWithInvalidSavingsAmount };
            var ex2 = Assert.Throws<FormatException>(() => _target.Execute(args));

            args = new string[] { "--file", _testDataWithInvalidStartDate };
            var ex3 = Assert.Throws<FormatException>(() => _target.Execute(args));
        }
    }
}