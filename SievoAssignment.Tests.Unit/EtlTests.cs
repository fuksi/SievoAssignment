using Moq;
using NUnit.Framework;
using System.IO;

namespace SievoAssignment.Tests.Unit
{
    public class EtlTests
    {
        private string _testDataPath;
        private Etl _target;
        private Mock<ISievoLogger> _sievoLoggerMock;


        [SetUp]
        public void Setup()
        {
            _testDataPath = Path.Join(Directory.GetCurrentDirectory(), "ExampleData.tsv");

            _sievoLoggerMock = new Mock<ISievoLogger>();
            _target = new Etl(_sievoLoggerMock.Object);
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
        }
    }
}