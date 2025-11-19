using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace SetVersionTask
{

    public class VersionString
    {
        public VersionString()
        {
            this.Major = "0";
            this.Minor = "0";
            this.Build = "0";
            this.Revision = "0";
        }

        public static VersionString FromFile(string filePath)
        {
            var rawTextLines = System.IO.File.ReadAllLines(filePath);

            var versionTextLine = rawTextLines.FirstOrDefault(rawTextLine =>
            {
                var textLine = rawTextLine.TrimStart();
                return !(String.IsNullOrEmpty(textLine) || textLine.StartsWith("//"));
            });
            if (versionTextLine == null) throw new Exception($"No version specified in file \"{filePath}\"");
            try
            {
                return new VersionString(versionTextLine);
            }
            catch (Exception exception)
            {
                throw new Exception($"Error reading version from file \"{filePath}\"", exception);
            }
        }

        public VersionString(string version)
        {
            if (!Parse(version))
            {
                throw new ArgumentException("Invalid version string");
            }
        }

        private bool Parse(string input)
        {
            string pattern = @"^(?<Major>\d+)\.(?<Minor>\d+)\.(?:(?:(?<Build>\d+)\.(?<Revision>\*|\d+))|(?<Build>\*|\d+))$";
            Regex regex = new Regex(pattern);
            var match = regex.Match(input);
            if (match.Success)
            {
                Major = match.Groups["Major"].Value;
                Minor = match.Groups["Minor"].Value;
                Build = match.Groups["Build"].Value;
                Revision = match.Groups["Revision"].Value;
            }
            return match.Success;
        }

        public static bool TryParse(string input, out VersionString version)
        {
            VersionString temp = new VersionString();
            version = null;
            if (temp.Parse(input))
            {
                version = temp;
            }
            return version != null;
        }

        public string Major { get; set; }
        public string Minor { get; set; }
        public string Build { get; set; }
        public string Revision { get; set; }

        public override string ToString()
        {
            return String.Format("{0}.{1}{2}{3}",
                Major, Minor,
                string.IsNullOrEmpty(Build) ? "" : "." + Build,
                string.IsNullOrEmpty(Revision) ? "" : "." + Revision);
        }
    }
}
