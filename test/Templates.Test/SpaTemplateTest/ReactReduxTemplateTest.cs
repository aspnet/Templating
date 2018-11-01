// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Xunit;
using Xunit.Abstractions;

namespace Templates.Test.SpaTemplateTest
{
    public class ReactReduxTemplateTest : SpaTemplateTestBase
    {
        public ReactReduxTemplateTest(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void ReactReduxTemplate_Works_NetCore()
            => SpaTemplateImpl(null, "reactredux");
    }
}
