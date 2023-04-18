// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.Core
{
    public interface IInputSanitizer
    {
        string Sanitize(string input);
    }
}
