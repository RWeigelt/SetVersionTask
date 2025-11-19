using System;
using System.IO;
using NUnit.Framework;
using SetVersionTask;

namespace SetVersionTaskTests
{
    [TestFixture]
    public class VersionStringTests
    {
        [TestCase("1.2.3.4", "1", "2", "3", "4")]
        [TestCase("1.2.3.*", "1", "2", "3", "*")]
        [TestCase("1.2.3", "1", "2", "3", "")]
        [TestCase("1.2.*", "1", "2", "*", "")]
        [TestCase("11.22.33.44", "11", "22", "33", "44")]
        public void CanParseVersionString(string input, string major, string minor, string build, string revision)
        {
            var versionString = new VersionString(input);
            Assert.That(major, Is.EqualTo(versionString.Major), "Major");
            Assert.That(minor, Is.EqualTo(versionString.Minor), "Minor");
            Assert.That(build, Is.EqualTo(versionString.Build), "Build");
            Assert.That(revision, Is.EqualTo(versionString.Revision), "Revision");
        }

        [TestCase("MinimalVersionFile.txt")]
        [TestCase("VersionFileWithComments.txt")]
        public void CanReadVersionStringFromFile(string fileName)
        {
            var testDataDirectoryPath = GetTestDataDirectoryPath();
            var filePath = Path.Combine(testDataDirectoryPath, fileName);
            var versionString = VersionString.FromFile(filePath);
            Assert.That("1", Is.EqualTo(versionString.Major), "Major");
            Assert.That("2", Is.EqualTo(versionString.Minor), "Minor");
            Assert.That("3", Is.EqualTo(versionString.Build), "Build");
            Assert.That("4", Is.EqualTo(versionString.Revision), "Revision");
        }

        [Test]
        public void CannotReadInvalidVersionStringFromFile()
        {
            var testDataDirectoryPath = GetTestDataDirectoryPath();
            var filePath = Path.Combine(testDataDirectoryPath, "InvalidVersionFile.txt");
            string[] expectedMessageParts = Array.Empty<string>();
            try
            {
                var versionString = VersionString.FromFile(filePath);
            }
            catch (Exception exception)
            {
                expectedMessageParts = new[]
                    {
                    exception.Message,
                    exception.InnerException?.Message ?? ""
                };
            }
            Assert.That(expectedMessageParts.Length, Is.EqualTo(2), "Inner exception");
            Assert.That(expectedMessageParts[0].StartsWith("Error reading version from file"), Is.True, "Outer message");
            Assert.That(expectedMessageParts[1], Is.EqualTo("Invalid version string"), "Inner message");
        }


        private string GetTestDataDirectoryPath()
        {
            return Path.Combine(Path.GetDirectoryName(this.GetType().Assembly.Location) ?? "", "TestData");
        }
    }
}
