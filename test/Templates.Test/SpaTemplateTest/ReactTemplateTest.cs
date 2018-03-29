using Xunit;
using Xunit.Abstractions;

namespace Templates.Test.SpaTemplateTest
{
    public class ReactTemplateTest : SpaTemplateTestBase
    {
        public ReactTemplateTest(ITestOutputHelper output) : base(output)
        {
        }

        [Fact(Skip = "https://github.com/aspnet/templating/issues/400")]
        public void ReactTemplate_Works_NetCore()
            => SpaTemplateImpl(null, "react");
    }
}
