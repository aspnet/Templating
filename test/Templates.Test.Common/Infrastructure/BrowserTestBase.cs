// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using Templates.Test.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Templates.Test.Infrastructure
{
    [CaptureSeleniumLogs]
    public class BrowserTestBase : TemplateTestBase, IClassFixture<BrowserFixture>
    {
        private static readonly AsyncLocal<IWebDriver> _browser = new AsyncLocal<IWebDriver>();
        private static readonly AsyncLocal<ILogs> _logs = new AsyncLocal<ILogs>();

        private bool _disposed = false;

        public static IWebDriver Browser => _browser.Value;

        public static ILogs Logs => _logs.Value;

        public BrowserTestBase(BrowserFixture browserFixture, ITestOutputHelper output) : base(output)
        {
            _browser.Value = browserFixture.Browser;
            _logs.Value = browserFixture.Logs;
        }

        public void AssertLogsOk()
        {
            var logs = Browser.Manage().Logs.GetLog("browser");

            var badLogs = logs.Where(l => l.Level >= LogLevel.Warning);

            foreach (var badLog in badLogs)
            {
                Output.WriteLine($"[{badLog.Timestamp}] - {badLog.Level} - {badLog.Message}");
            }

            Assert.Empty(badLogs);
        }

        public void TestBasicNavigation(AspNetProcess aspNetProcess, IEnumerable<string> urls)
        {
            foreach (var url in urls)
            {
                aspNetProcess.AssertOk(url);
                if(WebDriverFactory.HostSupportsBrowserAutomation)
                {
                    aspNetProcess.VisitInBrowser(Browser, url);
                    AssertLogsOk();
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            base.Dispose(disposing);
        }
    }
}
