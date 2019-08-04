// Copyright (c) Oak Aged LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NugetRepack.Tool
{
    using System;

    using Lamar;

    using Oakton;

    public class LamarCommandCreator : ICommandCreator
    {
        public LamarCommandCreator(IContainer container)
        {
            this.Container = container;
        }

        private IContainer Container { get; }

        public IOaktonCommand CreateCommand(Type commandType)
        {
            return (IOaktonCommand)this.Container.GetInstance(commandType);
        }

        public object CreateModel(Type modelType)
        {
            return this.Container.GetInstance(modelType);
        }
    }
}
