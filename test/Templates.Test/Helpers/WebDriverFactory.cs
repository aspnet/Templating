// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;

namespace Templates.Test.Helpers
{
    public static class WebDriverFactory
    {
        // Maximum time any action performed by WebDriver will wait before failing.
        // Any action will have to be completed in at most 10 seconds.
        // Providing a smaller value won't improve the speed of the tests in any
        // significant way and will make them more prone to fail on slower drivers.
        private const int DefaultMaxWaitTimeInSeconds = 10;

        public static IWebDriver CreateWebDriver()
        {
            var options = (IsAppVeyor || UseFirefox) ? CreateFirefoxOptions() : UseEdge ? CreateEdgeOptions() : CreateChromeOptions();

            try
            {
                var browser = new RemoteWebDriver(new Uri("http://127.0.0.1:4444/wd/hub"), options);
                browser.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(DefaultMaxWaitTimeInSeconds);
                return browser;
            }
            catch (WebDriverException ex)
            {
                var message =
                    "Failed to connect to the web driver. Please see the readme and follow the instructions to install selenium." +
                    "Remember to start the web driver with `selenium-standalone start` before running the tests.";
                throw new InvalidOperationException(message, ex);
            }
        }

        public static bool HostSupportsBrowserAutomation
            => string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("ASPNETCORE_BROWSER_AUTOMATION_DISABLED")) &&
               (IsAppVeyor || OSSupportsEdge());

        private static bool IsAppVeyor
            => Environment.GetEnvironmentVariables().Contains("APPVEYOR");

        private static bool UseFirefox 
            => !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("ASPNETCORE_BROWSER_AUTOMATION_FIREFOX"));

        private static bool UseEdge
            => !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("ASPNETCORE_BROWSER_AUTOMATION_EDGE"));

        private static int GetWindowsVersion()
        {
            var osDescription = RuntimeInformation.OSDescription;
            var windowsVersion = Regex.Match(osDescription, "^Microsoft Windows (\\d+)\\..*");
            return windowsVersion.Success ? int.Parse(windowsVersion.Groups[1].Value) : -1;
        }

        private static bool OSSupportsEdge()
        {
            var windowsVersion = GetWindowsVersion();
            return (windowsVersion >= DefaultMaxWaitTimeInSeconds && windowsVersion < 2000)
                || (windowsVersion >= 2016);
        }

        private static DriverOptions CreateChromeOptions()
        {
            var options = new ChromeOptions
            {
                AcceptInsecureCertificates = true
            };

            options.AddArgument("--headless");

            // On Windows/Linux, we don't need to set opts.BinaryLocation
            // But for Travis Mac builds we do
            var binaryLocation = Environment.GetEnvironmentVariable("TEST_CHROME_BINARY");
            if (!string.IsNullOrEmpty(binaryLocation))
            {
                options.BinaryLocation = binaryLocation;
                Console.WriteLine($"Set {nameof(ChromeOptions)}.{nameof(options.BinaryLocation)} to {binaryLocation}");
            }

            return options;
        }

        private static DriverOptions CreateFirefoxOptions()
        {
            var options = new FirefoxOptions
            {
                AcceptInsecureCertificates = true
            };

            options.AddArgument("--headless");
            return options;
        }

        private static DriverOptions CreateEdgeOptions()
        {
            var options = new EdgeOptions
            {
                AcceptInsecureCertificates = true
            };

            return options;
        }
    }
}
