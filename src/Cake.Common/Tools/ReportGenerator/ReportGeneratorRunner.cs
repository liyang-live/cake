﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Cake.Core;
using Cake.Core.IO;
using Cake.Core.Tooling;

namespace Cake.Common.Tools.ReportGenerator
{
    /// <summary>
    /// ReportGenerator runner.
    /// </summary>
    public sealed class ReportGeneratorRunner : Tool<ReportGeneratorSettings>
    {
        private readonly ICakeEnvironment _environment;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportGeneratorRunner"/> class.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="environment">The environment.</param>
        /// <param name="processRunner">The process runner.</param>
        /// <param name="globber">The globber.</param>
        public ReportGeneratorRunner(IFileSystem fileSystem, ICakeEnvironment environment, IProcessRunner processRunner, IGlobber globber) : base(fileSystem, environment, processRunner, globber)
        {
            _environment = environment;
        }

        /// <summary>
        /// Converts the specified coverage reports into human readable form according to the specified settings.
        /// </summary>
        /// <param name="reports">The coverage reports.</param>
        /// <param name="targetDir">The output directory.</param>
        /// <param name="settings">The settings.</param>
        public void Run(IEnumerable<FilePath> reports, DirectoryPath targetDir, ReportGeneratorSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            if (reports == null)
            {
                throw new ArgumentNullException("reports");
            }

            if (!reports.Any())
            {
                throw new ArgumentException("reports must not be empty", "reports");
            }

            if (targetDir == null)
            {
                throw new ArgumentNullException("targetDir");
            }

            Run(settings, GetArgument(settings, reports, targetDir));
        }

        private ProcessArgumentBuilder GetArgument(ReportGeneratorSettings settings, IEnumerable<FilePath> reports, DirectoryPath targetDir)
        {
            var builder = new ProcessArgumentBuilder();

            var joinedReports = string.Join(";", reports.Select((r) => r.MakeAbsolute(_environment).FullPath));
            AppendQuoted(builder, "reports", joinedReports);

            AppendQuoted(builder, "targetdir", targetDir.MakeAbsolute(_environment).FullPath);

            if (settings.ReportTypes != null && settings.ReportTypes.Any())
            {
                var joined = string.Join(";", settings.ReportTypes);
                AppendQuoted(builder, "reporttypes", joined);
            }

            if (settings.SourceDirectories != null && settings.SourceDirectories.Any())
            {
                var joined = string.Join(";", settings.SourceDirectories.Select((d) => d.MakeAbsolute(_environment).FullPath));
                AppendQuoted(builder, "sourcedirs", joined);
            }

            if (settings.HistoryDirectory != null)
            {
                AppendQuoted(builder, "historydir", settings.HistoryDirectory.MakeAbsolute(_environment).FullPath);
            }

            if (settings.AssemblyFilters != null && settings.AssemblyFilters.Any())
            {
                var joined = string.Join(";", settings.AssemblyFilters);
                AppendQuoted(builder, "assemblyfilters", joined);
            }

            if (settings.ClassFilters != null && settings.ClassFilters.Any())
            {
                var joined = string.Join(";", settings.ClassFilters);
                AppendQuoted(builder, "classfilters", joined);
            }

            if (settings.Verbosity != null)
            {
                AppendQuoted(builder, "verbosity", settings.Verbosity.ToString());
            }

            return builder;
        }

        /// <summary>
        /// Gets the name of the tool.
        /// </summary>
        /// <returns>The name of the tool.</returns>
        protected override string GetToolName()
        {
            return "ReportGenerator";
        }

        /// <summary>
        /// Gets the possible names of the tool executable.
        /// </summary>
        /// <returns>The tool executable name.</returns>
        protected override IEnumerable<string> GetToolExecutableNames()
        {
            return new[] { "ReportGenerator.exe" };
        }

        private void AppendQuoted(ProcessArgumentBuilder builder, string key, string value)
        {
            builder.AppendQuoted(string.Format(CultureInfo.InvariantCulture, "-{0}:{1}", key, value));
        }
    }
}