// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.IO;
using Xunit;
using Xunit.Abstractions;

namespace Templates.Test
{
    public class ClassLibraryIntegrationTest : TemplateTestBase
    {
        public ClassLibraryIntegrationTest(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void Metapackage_PackageReferences_Maintained_IfAppIsExecutable()
        {
            var testName = "Test";
            var appName = "App";
            var appPath = Path.Combine(TemplateOutputDir, appName, $"{appName}.csproj");
            var testPath = Path.Combine(TemplateOutputDir, testName, $"{testName}.csproj");
            var testProjectAssetsPath = Path.Combine(TemplateOutputDir, testName, "obj", "project.assets.json");

            // Create projects
            RunDotNetNew("xunit", null, outputDirectory: testName, noRestore: true);
            RunDotNetNew("web", null, outputDirectory: appName, noRestore: true);

            // Dependency chain: test -> app
            RunDotNet($"add {testPath} reference {appPath}");

            // Restore the test app
            RunDotNet($"restore {testPath}");

            // Reference to Microsoft.AspNetCore.App should be maintained
            Assert.Contains("Microsoft.AspNetCore.App/", File.ReadAllText(testProjectAssetsPath));
        }
    }
}
