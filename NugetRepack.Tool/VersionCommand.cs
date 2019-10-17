// Copyright (c) Oak Aged LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
            Console.WriteLine(version?.InformationalVersion ?? "unknown");

            return true;
        }
    }
}
