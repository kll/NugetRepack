// Copyright (c) Oak Aged LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NugetRepack.Tool
{
    using System;
    using System.Threading.Tasks;

    using Oakton;

    using Serilog;
    using Serilog.Core;

    public class RepackCommand : OaktonAsyncCommand<RepackInput>
    {
        public RepackCommand(LoggingLevelSwitch loggingLevelSwitch, INugetRepacker nugetRepacker)
        {
            this.LoggingLevelSwitch = loggingLevelSwitch;
            this.NugetRepacker = nugetRepacker;
        }

        private static ILogger Logger { get; } = Log.ForContext<RepackCommand>();

        private LoggingLevelSwitch LoggingLevelSwitch { get; }

        private INugetRepacker NugetRepacker { get; }

        public override async Task<bool> Execute(RepackInput input)
        {
            this.LoggingLevelSwitch.MinimumLevel = input.LogLevelFlag;

            if (string.IsNullOrWhiteSpace(input.PackageFile))
            {
                Logger.Fatal("missing required package file parameter");

                return false;
            }

            Logger.Verbose("Repacking file: {File}", input.PackageFile);

            try
            {
                await this.NugetRepacker.RepackPackage(
                    input.PackageFile,
                    input.NewPackageIdFlag,
                    input.StripPrereleaseFlag);

                return true;
            }
            catch (Exception exception)
            {
                Logger.Fatal(exception.Message, exception);

                return false;
            }
        }
    }
}
