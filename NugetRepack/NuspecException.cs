// Copyright (c) Oak Aged LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
