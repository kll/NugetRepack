namespace NugetRepack.Tool
{
    using System;
    using System.Reflection;

    using Oakton;

    public class VersionCommand : OaktonCommand<VersionInput>
    {
        public override bool Execute(VersionInput input)
        {
            var version = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            Console.WriteLine(version.InformationalVersion);

            return true;
        }
    }
}
