﻿using Templates.Test.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Templates.Test.SpaTemplateTest
{
    public class ReactReduxTemplateTest : SpaTemplateTestBase
    {
        public ReactReduxTemplateTest(BrowserFixture browserFixture, ITestOutputHelper output) : base(browserFixture, output)
        {
        }

        [Fact]
        public void ReactReduxTemplate_Works_NetCore()
            => SpaTemplateImpl(null, "reactredux");
    }
}
