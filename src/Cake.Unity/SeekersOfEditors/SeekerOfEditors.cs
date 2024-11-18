using System;
using System.Collections.Generic;
using System.Linq;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Unity.Version;
using LanguageExt;
using LanguageExt.Common;

namespace Cake.Unity.SeekersOfEditors
{
    internal abstract class SeekerOfEditors
    {
        protected readonly ICakeEnvironment environment;
        private readonly IGlobber globber;
        protected readonly ICakeLog log;

        public static SeekerOfEditors GetSeeker(ICakeEnvironment environment, IGlobber globber, ICakeLog log, IFileSystem fileSystem)
        {
            if (environment.Platform.Family == PlatformFamily.Windows)
                return new WindowsSeekerOfEditors(environment, globber, log);

            if (environment.Platform.Family == PlatformFamily.OSX)
                return new OSXSeekerOfEditors(environment, globber, log, fileSystem);

            if (environment.Platform.Family == PlatformFamily.Linux)
                return new LinuxSeekerOfEditors(environment, globber, log);

            throw new NotSupportedException("Cannot locate Unity Editors. Only Windows, OSX and Linux is supported.");
        }

        protected SeekerOfEditors(ICakeEnvironment environment, IGlobber globber, ICakeLog log)
        {
            this.environment = environment;
            this.globber = globber;
            this.log = log;
        }

        public IReadOnlyCollection<UnityEditorDescriptor> Seek()
        {
            log.Debug("Searching for available Unity Editors...");
            log.Debug("Search patterns: [{0}]", string.Join(", ", SearchPatterns));
            var candidates = GetCandidates(SearchPatterns);

            log.Debug("Found {0} candidates.", candidates.Count);
            log.Debug(string.Empty);

            var editors = new List<UnityEditorDescriptor>();
            foreach (var candidatePath in candidates)
            {
                var version = DetermineVersion(candidatePath);
                version.IfSome(unityVersion => editors.Add(new UnityEditorDescriptor(unityVersion, candidatePath)));
            }

            return editors.ToList();
        }

        protected abstract string[] SearchPatterns { get; }

        protected abstract Option<UnityVersion> DetermineVersion(FilePath editorPath);

        private List<FilePath> GetCandidates(string[] searchPatterns) =>
            searchPatterns.SelectMany(globber.GetFiles).ToList();
    }
}
