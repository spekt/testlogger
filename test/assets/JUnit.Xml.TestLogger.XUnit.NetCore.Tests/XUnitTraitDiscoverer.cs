using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;
using Xunit.Abstractions;

namespace NUnit.Xml.TestLogger.Tests2
{
    [TraitDiscoverer("NUnit.Xml.TestLogger.Tests2.ApiTestTraitDiscoverer", "JUnit.Xml.TestLogger.XUnit.NetCore.Tests")]
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class ApiTestAttribute : Attribute, ITraitAttribute
    {
    }

    public class ApiTestTraitDiscoverer : ITraitDiscoverer
    {
        private const string CategoryKey = "Category";
        private const string CategoryName = "ApiTest";

        public IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute)
        {
            yield return new KeyValuePair<string, string>(CategoryKey, CategoryName);
        }
    }
}
