namespace NugetRepack.Tool
{
    using Lamar;

    using Serilog.Core;

    public class NugetRepackRegistry : ServiceRegistry
    {
        public NugetRepackRegistry(LoggingLevelSwitch levelSwitch)
        {
            this.Scan(
                scan =>
                {
                    scan.TheCallingAssembly();
                    scan.AssembliesAndExecutablesFromApplicationBaseDirectory();
                    scan.WithDefaultConventions();
                });
            this.For<LoggingLevelSwitch>().Use(levelSwitch);
            this.ForConcreteType<RepackCommand>();
            this.ForConcreteType<VersionCommand>();
        }
    }
}
