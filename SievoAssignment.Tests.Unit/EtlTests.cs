using NUnit.Framework;
using System.IO;

namespace SievoAssignment.Tests.Unit
{
    public class EtlTests
    {
        private string _testDataPath;
        private Etl _target;

        [SetUp]
        public void Setup()
        {
            _testDataPath = Path.Join(Directory.GetCurrentDirectory(), "ExampleData.tsv");
            _target = new Etl();
        }

        [Test]
        public void Execute_ValidArgs_Pass()
        {
            var args = new string[] { "--file", _testDataPath };
            _target.Execute(args);
            Assert.Pass();
        }
    }
}