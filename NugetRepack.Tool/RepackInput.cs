// Copyright (c) Oak Aged LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NugetRepack.Tool
{
    using Oakton;

    using Serilog.Events;

    public class RepackInput
    {
        [Description("The NuGet package to repack.")]
        public string? PackageFile { get; set; }

        [Description("The logging level to use. Default: Information")]
        public LogEventLevel LogLevelFlag { get; set; } = LogEventLevel.Information;

        [Description("Strip the prerelease portion of the version number.")]
        public bool StripPrereleaseFlag { get; set; }
    }
}
