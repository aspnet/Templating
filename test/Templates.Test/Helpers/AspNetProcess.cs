﻿using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.CommandLineUtils;
using Xunit;
using Xunit.Abstractions;
using Microsoft.AspNetCore.Certificates.Generation;
using System.Threading;

namespace Templates.Test.Helpers
{
    public class AspNetProcess : IDisposable
    {
        private const string DefaultFramework = "netcoreapp2.1";
        private const string ListeningMessagePrefix = "Now listening on: ";
        private static int Port = 5000 + new Random().Next(3000);

        private readonly ProcessEx _process;
        private readonly Uri _listeningUri;
        private readonly HttpClient _httpClient;
        private readonly ITestOutputHelper _output;
        private readonly int _httpsPort;
        private readonly int _httpPort;

        public AspNetProcess(ITestOutputHelper output, string workingDirectory, string projectName, string targetFrameworkOverride, bool publish)
        {
            _output = output;
            _httpClient = new HttpClient(new HttpClientHandler()
            {
                AllowAutoRedirect = true,
                UseCookies = true,
                CookieContainer = new CookieContainer(),
                ServerCertificateCustomValidationCallback = (m, c, ch, p) => true
            });

            var now = DateTimeOffset.Now;
            new CertificateManager().EnsureAspNetCoreHttpsDevelopmentCertificate(now, now.AddYears(1));

            var framework = string.IsNullOrEmpty(targetFrameworkOverride) ? DefaultFramework : targetFrameworkOverride;
            if (publish)
            {
                output.WriteLine("Publishing ASP.NET application...");

                // Workaround for issue with runtime store not yet being published
                // https://github.com/aspnet/Home/issues/2254#issuecomment-339709628
                var extraArgs = "-p:PublishWithAspNetCoreTargetManifest=false";

                ProcessEx
                    .Run(output, workingDirectory, DotNetMuxer.MuxerPathOrDefault(), $"publish -c Release {extraArgs}")
                    .WaitForExit(assertSuccess: true);
                workingDirectory = Path.Combine(workingDirectory, "bin", "Release", framework, "publish");
            }
            else
            {
                output.WriteLine("Building ASP.NET application...");
                ProcessEx
                    .Run(output, workingDirectory, DotNetMuxer.MuxerPathOrDefault(), "build --no-restore -c Debug")
                    .WaitForExit(assertSuccess: true);
            }

            _httpPort = Interlocked.Increment(ref Port);
            _httpsPort = Interlocked.Increment(ref Port);
            var envVars = new Dictionary<string, string>
            {
                { "ASPNETCORE_URLS", $"http://localhost:{_httpPort};https://localhost:{_httpsPort}" },
                { "ASPNETCORE_HTTPS_PORT", $"{_httpsPort}" }
            };

            if (!publish)
            {
                envVars["ASPNETCORE_ENVIRONMENT"] = "Development";
            }

            output.WriteLine("Running ASP.NET application...");
            if (framework.StartsWith("netcore"))
            {
                var dllPath = publish ? $"{projectName}.dll" : $"bin/Debug/{framework}/{projectName}.dll";
                _process = ProcessEx.Run(output, workingDirectory, DotNetMuxer.MuxerPathOrDefault(), $"exec {dllPath}", envVars: envVars);
            }
            else
            {
                var exeFullPath = publish
                    ? Path.Combine(workingDirectory, $"{projectName}.exe")
                    : Path.Combine(workingDirectory, "bin", "Debug", framework, $"{projectName}.exe");
                _process = ProcessEx.Run(output, workingDirectory, exeFullPath, envVars: envVars);
            }

            // Wait until the app is accepting HTTP requests
            output.WriteLine("Waiting until ASP.NET application is accepting connections...");
            var listeningMessage = _process
                .OutputLinesAsEnumerable
                .Where(line => line != null)
                .FirstOrDefault(line => line.StartsWith(ListeningMessagePrefix, StringComparison.Ordinal));
            Assert.True(!string.IsNullOrEmpty(listeningMessage), $"ASP.NET process exited without listening for requests.\nOutput: { _process.Output }\nError: { _process.Error }");

            // Verify we have a valid URL to make requests to
            var listeningUrlString = listeningMessage.Substring(ListeningMessagePrefix.Length);
            _listeningUri = new Uri(listeningUrlString, UriKind.Absolute);
            output.WriteLine($"Detected that ASP.NET application is accepting connections on: {listeningUrlString}");
        }

        public void AssertOk(string requestUrl)
            => AssertStatusCode(requestUrl, HttpStatusCode.OK);

        public void AssertNotFound(string requestUrl)
            => AssertStatusCode(requestUrl, HttpStatusCode.NotFound);

        public void AssertStatusCode(string requestUrl, HttpStatusCode statusCode, string acceptContentType = null)
        {
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                new Uri(_listeningUri, requestUrl));

            if (!string.IsNullOrEmpty(acceptContentType))
            {
                request.Headers.Add("Accept", acceptContentType);
            }

            var response = _httpClient.SendAsync(request).Result;
            Assert.Equal(statusCode, response.StatusCode);
        }

        public IWebDriver VisitInBrowser()
        {
            _output.WriteLine($"Opening browser at {_listeningUri}...");
            var driver = WebDriverFactory.CreateWebDriver();
            driver.Navigate().GoToUrl(_listeningUri);
            return driver;
        }

        public void Dispose()
        {
            _httpClient.Dispose();
            _process.Dispose();
        }
    }
}
