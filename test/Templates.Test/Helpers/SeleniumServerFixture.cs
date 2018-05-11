// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace Templates.Test.Helpers
{
    public class SeleniumServerFixture : IDisposable
    {
        private string _workingDirectory;
        private Process _serverProcess;
        private static readonly object _serverLock = new object();

        public SeleniumServerFixture()
        {
            _workingDirectory = Directory.GetCurrentDirectory();
            StartSeleniumStandaloneServer();
        }

        public void Dispose()
        {
            _serverProcess.Kill();
            _serverProcess.Dispose();

            // Find the java process running selenium-standalone
            var childProcesses = Process.GetProcessesByName("java");
            foreach (var childProcess in childProcesses)
            {
                childProcess.Kill();
                childProcess.Dispose();
            }
        }

        public void StartSeleniumStandaloneServer()
        {
            lock (_serverLock)
            {
                RunViaShell(_workingDirectory, "npm install -g selenium standalone");
            }
            lock (_serverLock)
            {
                RunViaShell(_workingDirectory, "selenium-standalone install");
            }

            // Starts a java process that runs the selenium server
            _serverProcess = RunViaShell(_workingDirectory, "selenium-standalone start");
        }

        private static Process RunViaShell(string workingDirectory, string commandAndArgs)
        {
            var (shellExe, argsPrefix) = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? ("cmd", "/c")
                : ("bash", "-c");

            return Run(workingDirectory, shellExe, $"{argsPrefix} \"{commandAndArgs}\"");
        }

        private static Process Run(string workingDirectory, string command, string args = null, IDictionary<string, string> envVars = null)
        {
            var startInfo = new ProcessStartInfo(command, args)
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = workingDirectory
            };

            return Process.Start(startInfo);
        }
    }
}
