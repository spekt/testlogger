namespace Spekt.TestLogger
{
    using System.Threading.Tasks;
    using Microsoft.Testing.Platform.Extensions;

    internal sealed class JUnitTestReporterExtension : IExtension
    {
        public string Uid => nameof(JUnitTestReporterExtension);

        // TODO:
        public string Version => "1.0.0";

        public string DisplayName => "JUnit test reporter extension";

        public string Description => "JUnit test reporter extension";

        public Task<bool> IsEnabledAsync()
            => Task.FromResult(true);
    }
}
