﻿using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Unity.Version;
using System.Text.RegularExpressions;
using LanguageExt;

namespace Cake.Unity.SeekersOfEditors
{
    internal class LinuxSeekerOfEditors : SeekerOfEditors
    {
        private readonly Regex VersionRegex = new Regex("(?<major>\\d+)\\.(?<minor>\\d+)\\.(?<patch>\\d+)(?<branch>\\w)(?<build>\\d+)");

        public LinuxSeekerOfEditors(ICakeEnvironment environment, IGlobber globber, ICakeLog log)
            : base(environment, globber, log)
        {
        }

        protected override string[] SearchPatterns => new[] {
            "/home/*/Unity/Hub/Editor/*/Editor/Unity"
            };

        protected override Option<UnityVersion> DetermineVersion(FilePath editorPath)
        {
            log.Debug($"Determining version of Unity Editor at path {editorPath}...");

            var versionMatch = VersionRegex.Match(editorPath.FullPath);

            if(!versionMatch.Success)
            {
                log.Debug($"Can't find UnityVersion for {editorPath}");
                return Option<UnityVersion>.None;
            }

            var major = int.Parse(versionMatch.Groups["major"].Value);
            var minor = int.Parse(versionMatch.Groups["minor"].Value);
            var patch = int.Parse(versionMatch.Groups["patch"].Value);
            var branch = char.Parse(versionMatch.Groups["branch"].Value);
            var build = int.Parse(versionMatch.Groups["build"].Value);

            var unityVersion = new UnityVersion(major, minor, patch, branch, build);

            log.Debug($"Result Unity Editor version (full): {unityVersion}");
            return unityVersion;
        }
    }
}
