using System;
using Microsoft.Build.Framework;

namespace SetVersionTask
{
    public class SetVersion : Microsoft.Build.Utilities.Task
    {
        [Required]
        public string FileName { get; set; }

        public string Version { get; set; }

        public string VersionFile { get; set; }

        public override bool Execute()
        {
            try
            {
                var version = Version;
                if (!String.IsNullOrEmpty(VersionFile))
                {
                    version= VersionString.FromFile(VersionFile).ToString();
                }

                if (this.FileName.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
                {
                    var updater = new CSharpUpdater(version);
                    updater.UpdateFile(FileName);
                }
                else if (this.FileName.EndsWith(".appxmanifest", StringComparison.OrdinalIgnoreCase))
                {
                    var updater = new AppxManifestUpdater(version);
                    updater.UpdateFile(FileName);
                }
                else if (this.FileName.EndsWith(".nuspec", StringComparison.OrdinalIgnoreCase))
                {
                    UpdateNuSpec();
                }
            }
            catch (Exception e)
            {
                Log.LogError(e.Message);
                return false;
            }
            return true;
        }

        private void UpdateNuSpec()
        {


        }

        private void ValidateArguments()
        {

        }

    }
}
