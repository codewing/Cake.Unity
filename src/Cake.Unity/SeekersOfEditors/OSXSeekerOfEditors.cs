using System.IO;
using System.Linq;
using System.Xml.Linq;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Unity.Version;
using LanguageExt;

namespace Cake.Unity.SeekersOfEditors
{
    internal class OSXSeekerOfEditors : SeekerOfEditors
    {
        private readonly IFileSystem fileSystem;

        public OSXSeekerOfEditors(ICakeEnvironment environment, IGlobber globber, ICakeLog log, IFileSystem fileSystem)
            : base(environment, globber, log)
        {
            this.fileSystem = fileSystem;
        }

        protected override string[] SearchPatterns => new[] {"/Applications/Unity/Hub/Editor/*/Unity.app/Contents/MacOS/Unity"};

        protected override Option<UnityVersion> DetermineVersion(FilePath editorPath)
        {
            log.Debug($"Determining version of Unity Editor at path {editorPath}...");

            string? version;
            using (var stream = fileSystem.GetFile(PlistPath(editorPath)).OpenRead())
                version = new InfoPlistParser().UnityVersionFromInfoPlist(stream);

            if (version == null)
            {
                log.Debug($"Can't find UnityVersion for {editorPath}");
                return Option<UnityVersion>.None;
            }

            var unityVersion = UnityVersion.Parse(version);

            log.Debug($"Result Unity Editor version (full): {unityVersion}");
            return unityVersion;
        }

        private static string PlistPath(FilePath editorPath) =>
            editorPath.FullPath.Replace("/MacOS/Unity", "/Info.plist");
    }

    internal class InfoPlistParser
    {
        public string? UnityVersionFromInfoPlist(Stream infoPlistStream) =>
            XDocument
                .Load(infoPlistStream)
                .Descendants()
                .SkipWhile((element) => element.Value != "CFBundleVersion")
                .Skip(1)
                .FirstOrDefault()
                ?.Value;
    }
}
