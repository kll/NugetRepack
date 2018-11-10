namespace NugetRepack
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
