// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;
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
            // Find the java process running selenium-standalone
            foreach (var childProcess in GetChildProcesses(_serverProcess.Id))
            {
                childProcess.Kill();
                childProcess.Dispose();
            }

            _serverProcess.Kill();
            _serverProcess.Dispose();
        }

        private static List<Process> GetChildProcesses(int parentProcessId)
        {
            var results = new List<Process>();

            // find processes with the given parent process id
            var queryText = $"Select ProcessId From Win32_Process Where ParentProcessId = {parentProcessId}";
            using (var searcher = new ManagementObjectSearcher(queryText))
            {
                foreach (var obj in searcher.Get())
                {
                    var data = obj.Properties["processid"].Value;
                    if (data != null)
                    {
                        // retrieve the process
                        var childId = Convert.ToInt32(data);
                        var childProcess = Process.GetProcessById(childId);

                        // ensure the current process is still alive
                        if (childProcess != null)
                        {
                            results.Add(childProcess);
                        }
                    }
                }
            }

            return results;
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
