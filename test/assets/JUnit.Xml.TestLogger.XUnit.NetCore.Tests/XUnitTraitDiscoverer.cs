using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;
#if USES_XUNIT3
using Xunit.v3;
#else
using Xunit.Abstractions;
#endif

namespace NUnit.Xml.TestLogger.Tests2
{
#if !USES_XUNIT3
    [TraitDiscoverer("NUnit.Xml.TestLogger.Tests2.ApiTestTraitDiscoverer", "JUnit.Xml.TestLogger.XUnit.NetCore.Tests")]
#endif
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class ApiTestAttribute : Attribute, ITraitAttribute
    {
        private const string CategoryKey = "Category";
        private const string CategoryName = "ApiTest";

#if USES_XUNIT3
        public IReadOnlyCollection<KeyValuePair<string, string>> GetTraits()
        {
            return ImmutableArray.Create(new KeyValuePair<string, string>(CategoryKey, CategoryName));
        }
#endif
    }

#if !USES_XUNIT3
    public class ApiTestTraitDiscoverer : ITraitDiscoverer
    {
        private const string CategoryKey = "Category";
        private const string CategoryName = "ApiTest";

        public IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute)
        {
            yield return new KeyValuePair<string, string>(CategoryKey, CategoryName);
        }
    }
#endif
}
