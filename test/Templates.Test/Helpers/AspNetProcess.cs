// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.CommandLineUtils;
using Xunit.Abstractions;

namespace Templates.Test.Helpers
{
    public class AspNetProcess : IDisposable
    {
        private const string DefaultFramework = "netcoreapp3.0";

        private readonly ProcessEx _process;

        public AspNetProcess(
            ITestOutputHelper output,
            string workingDirectory,
            string projectName,
            string targetFrameworkOverride,
            bool publish,
            int httpPort,
            int httpsPort)
        {
            var now = DateTimeOffset.Now;

            var framework = string.IsNullOrEmpty(targetFrameworkOverride) ? DefaultFramework : targetFrameworkOverride;
            if (publish)
            {
                output.WriteLine("Publishing ASP.NET application...");

                ProcessEx
                    .Run(output, workingDirectory, DotNetMuxer.MuxerPathOrDefault(), $"publish -c Release")
                    .WaitForExit(assertSuccess: true);
                workingDirectory = Path.Combine(workingDirectory, "bin", "Release", framework, "publish");
                if (File.Exists(Path.Combine(workingDirectory, "ClientApp", "package.json")))
                {
                    Npm.RestoreWithRetry(output, Path.Combine(workingDirectory, "ClientApp"));
                }
            }
            else
            {
                output.WriteLine("Building ASP.NET application...");
                ProcessEx
                    .Run(output, workingDirectory, DotNetMuxer.MuxerPathOrDefault(), $"build --no-restore -c Debug -f {framework}")
                    .WaitForExit(assertSuccess: true);
            }

            var envVars = new Dictionary<string, string>
            {
                { "ASPNETCORE_URLS", $"http://127.0.0.1:{httpPort};https://127.0.0.1:{httpsPort}" }
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
                using (new AddFirewallExclusion(exeFullPath))
                {
                    _process = ProcessEx.Run(output, workingDirectory, exeFullPath, envVars: envVars);
                }
            }

        }

        public void Dispose()
        {
            _process.Dispose();
        }
    }
}
