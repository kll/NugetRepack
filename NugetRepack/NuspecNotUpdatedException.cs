// Copyright (c) Oak Aged LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NugetRepack
{
    using System;

    public class NuspecNotUpdatedException : NuspecException
    {
        public NuspecNotUpdatedException()
            : this("Failed to update nuspec file.")
        {
        }

        public NuspecNotUpdatedException(string message)
            : base(message)
        {
        }

        public NuspecNotUpdatedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
