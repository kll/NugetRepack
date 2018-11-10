namespace NugetRepack
{
    using System;

    public abstract class NuspecException : Exception
    {
        protected NuspecException(string message)
            : base(message)
        {
        }

        protected NuspecException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
