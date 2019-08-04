// Copyright (c) Oak Aged LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
