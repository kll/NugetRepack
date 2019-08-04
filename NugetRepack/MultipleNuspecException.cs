// Copyright (c) Oak Aged LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
