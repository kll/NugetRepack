namespace NugetRepack.Tool
{
    using System;
    using System.Threading.Tasks;

    using Lamar;

    using Oakton;

    using Serilog;
    using Serilog.Core;
    using Serilog.Sinks.SystemConsole.Themes;

    internal class Program
    {
        internal static async Task<int> Main(string[] args)
        {
            var loggingLevelSwitch = new LoggingLevelSwitch();
            ConfigureLogger(loggingLevelSwitch);
            var container = new Container(new NugetRepackRegistry(loggingLevelSwitch));
            var executor = CommandExecutor.For(
                config =>
                {
                    config.RegisterCommand<RepackCommand>();
                    config.RegisterCommand<VersionCommand>();
                },
                new LamarCommandCreator(container));

            return await executor.ExecuteAsync(args);
        }

        private static void ConfigureLogger(LoggingLevelSwitch loggingLevelSwitch)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(loggingLevelSwitch)
                .WriteTo.Console(theme: AnsiConsoleTheme.Code)
                .CreateLogger();
            AppDomain.CurrentDomain.ProcessExit += (sender, eventArgs) => Log.CloseAndFlush();
        }
    }
}
