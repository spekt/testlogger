// Copyright (c) Spekt Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Spekt.TestLogger.Utilities
{
    public static class StringExtensions
     {
         public static string SubstringAfterDot(this string name)
         {
             if (string.IsNullOrEmpty(name))
             {
                 return string.Empty;
             }

             var idx = name.LastIndexOf('.');
             if (idx != -1)
             {
                 return name.Substring(idx + 1);
             }

             return name;
         }

         public static string SubstringBeforeDot(this string name)
         {
             if (string.IsNullOrEmpty(name))
             {
                 return string.Empty;
             }

             var idx = name.LastIndexOf(".");
             if (idx != -1)
             {
                 return name.Substring(0, idx);
             }

             return string.Empty;
         }
     }
}