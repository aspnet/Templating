// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Xunit.Abstractions;

namespace Templates.Test.Infrastructure
{
    public class BrowserTestBase : TemplateTestBase
    {
        public BrowserTestBase(ITestOutputHelper output) : base(output)
        {
        }

        public Browser Browser => new Browser();
    }

    public class Browser
    {
        public IEnumerable<string> Title { get; internal set; }

        internal void WaitForElement(string v)
        {
            throw new NotImplementedException();
        }

        internal IEnumerable<char> GetText(string v)
        {
            throw new NotImplementedException();
        }

        internal void Click(ParObj counterComponent, object p)
        {
            throw new NotImplementedException();
        }

        internal void WaitForUrl(string v)
        {
            throw new NotImplementedException();
        }

        internal ParObj FindElement(ParObj fetchDataComponent, string v = null, int? timeoutSeconds = null)
        {
            throw new NotImplementedException();
        }

        internal ParObj FindElement(string v)
        {
            throw new NotImplementedException();
        }

        internal void Click(ParObj parObj)
        {
            throw new NotImplementedException();
        }
    }

    public class ParObj : Browser
    {
        public ParObj Parent()
        {
            throw new NotImplementedException();
        }

        internal List<object> FindElements(object p)
        {
            throw new NotImplementedException();
        }
    }

    public class By
    {
        internal static ParObj PartialLinkText(string v)
        {
            throw new NotImplementedException();
        }

        internal static object CssSelector(string v)
        {
            throw new NotImplementedException();
        }
    }
}
