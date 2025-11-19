using System;
using System.IO;
using NUnit.Framework;
using SetVersionTask;

namespace SetVersionTaskTests
{
    [TestFixture]
    public class CSharpUpdaterTests
    {
        [TestCase("[assembly: AssemblyVersion(\"1.2.3.4\")]", "1.2.3.4")]
        [TestCase("[assembly: AssemblyVersionAttribute(\"1.2.3.*\")]", "1.2.3.*")]
        [TestCase("\t[assembly: AssemblyVersion(\"1.2.3\")] // some comment", "1.2.3")]
        [TestCase("   [assembly: AssemblyVersion(\"1.2.*\")]", "1.2.*")]
        [TestCase("[assembly: AssemblyVersion(\"11.22.33.44\")]", "11.22.33.44")]
        public void CanFindAssemblyVersionAttribute(string input, string version)
        {
            var versionString = CSharpUpdater.GetVersionString(input, "AssemblyVersion");
            Assert.That(versionString, Is.Not.Null, "Expected a match");
            Assert.That(version, Is.EqualTo(versionString.Value), "Version");
        }

        [TestCase("Version(\"11.22.33.44\")")]
        [TestCase("//[assembly: AssemblyVersion(\"1.2.3.4\")]")]
        [TestCase("[assembly: AssemblyCheeseVersion(\"11.22.33.44\")]")]
        [TestCase("[assembly: Version(\"11.22.33.44\")]")]
        [TestCase("[blah: AssemblyVersion(\"11.22.33.44\")]")]
        public void ShouldNotMatch(string input)
        {
            var versionString = CSharpUpdater.GetVersionString(input, "AssemblyVersion");
            Assert.That(versionString, Is.Null, "Expected no match");
        }

        [TestCase("[assembly: AssemblyVersion(\"1.2.3.4\")]", "=.=.+.=", "AssemblyVersion", "[assembly: AssemblyVersion(\"1.2.4.4\")]")]
        [TestCase("[assembly: AssemblyFileVersion(\"1.2.3.4\")]", "+.5.+.7", "AssemblyFileVersion", "[assembly: AssemblyFileVersion(\"2.5.4.7\")]")]
        [TestCase("[assembly: AssemblyFileVersion(\"1.2.3.4\")]", "5.6.7.8", "AssemblyFileVersion", "[assembly: AssemblyFileVersion(\"5.6.7.8\")]")]
        public void CanUpdateRuleWithString(string inLine, string rule, string attributeName, string outLine)
        {
            string line = inLine;
            CSharpUpdater.UpdateLineWithRule(ref line, new CSharpVersionUpdateRule(attributeName, rule));
            Assert.That(outLine, Is.EqualTo(line));
        }

        [Test]
        public void WriteFileIfModified()
        {
            var originalFilePath = Path.Combine(GetTestDataDirectoryPath(), "OriginalFileToBeUpdated.cs");
            var testFilePath = originalFilePath + ".txt";
            File.Copy(originalFilePath, testFilePath, true);

            var updater = new CSharpUpdater("5.6.7.8");
            updater.UpdateFile(testFilePath);

            var originalText = File.ReadAllText(originalFilePath);
            var updatedText = File.ReadAllText(testFilePath);

            Assert.That(originalText, Is.Not.EqualTo(updatedText));
        }

        [Test]
        public void DoNotWriteFileIfNotModified()
        {
            var originalFilePath = Path.Combine(GetTestDataDirectoryPath(), "OriginalFileToBeUpdated.cs");
            var testFilePath = originalFilePath + ".txt";
            File.Copy(originalFilePath, testFilePath, true);
            var fileInfoBefore = new FileInfo(testFilePath);
            var modificationTimeBefore = fileInfoBefore.LastWriteTime;

            var updater = new CSharpUpdater("1.2.3.4");
            updater.UpdateFile(testFilePath);

            var fileInfoAfter = new FileInfo(testFilePath);
            var modificationTimeAfter = fileInfoAfter.LastWriteTime;

            Assert.That(modificationTimeBefore, Is.EqualTo(modificationTimeAfter));
        }

        private string GetTestDataDirectoryPath()
        {
            return Path.Combine(Path.GetDirectoryName(this.GetType().Assembly.Location) ?? "", "TestData");
        }
    }
}
