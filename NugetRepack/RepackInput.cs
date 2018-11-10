namespace NugetRepack
{
    using Oakton;

    using Serilog.Events;

    public class RepackInput
    {
        [Description("The NuGet package to repack.")]
        public string PackageFile { get; set; }

        [Description("The logging level to use. Default: Information")]
        public LogEventLevel LogLevelFlag { get; set; } = LogEventLevel.Information;
    }
}
