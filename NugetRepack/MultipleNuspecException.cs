namespace NugetRepack
{
    using System;

    public class MultipleNuspecException : NuspecException
    {
        public MultipleNuspecException()
            : this("Multiple nuspec files found.")
        {
        }

        public MultipleNuspecException(string message)
            : base(message)
        {
        }

        public MultipleNuspecException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
