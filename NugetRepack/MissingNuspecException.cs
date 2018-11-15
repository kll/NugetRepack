namespace NugetRepack
{
    using System;

    public class MissingNuspecException : NuspecException
    {
        public MissingNuspecException()
            : this("No nuspec file was found.")
        {
        }

        public MissingNuspecException(string message)
            : base(message)
        {
        }

        public MissingNuspecException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
