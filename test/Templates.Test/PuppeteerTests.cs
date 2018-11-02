using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Server.IntegrationTesting;
using Microsoft.AspNetCore.Testing;
using Templates.Test.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Templates.Test
{
    public class MvcTests : PuppeteerTestsBase
    {
        public MvcTests(ITestOutputHelper output)
            : base(output)
        {
        }

        [Fact]
        public async Task MvcTemplate_NoAuth_NoHttps_Works_NetCore_ForDefaultTemplate()
            => await MvcTemplate_NoAuth(targetFrameworkOverride: null, languageOverride: default, noHttps: true);

        private async Task MvcTemplate_NoAuth(string targetFrameworkOverride, string languageOverride, string auth = null, bool noHttps = false)
        {
            using (StartLog(out var loggerFactory))
            {
                RunDotNetNew("mvc", targetFrameworkOverride, auth, language: languageOverride, noHttps);

                AssertDirectoryExists("Areas", false);
                AssertDirectoryExists("Extensions", false);
                AssertFileExists("urlRewrite.config", false);
                AssertFileExists("Controllers/AccountController.cs", false);

                var projectExtension = languageOverride == "F#" ? "fsproj" : "csproj";
                var projectFileContents = ReadFile($"{ProjectName}.{projectExtension}");
                Assert.DoesNotContain(".db", projectFileContents);
                Assert.DoesNotContain("Microsoft.EntityFrameworkCore.Tools", projectFileContents);
                Assert.DoesNotContain("Microsoft.VisualStudio.Web.CodeGeneration.Design", projectFileContents);
                Assert.DoesNotContain("Microsoft.EntityFrameworkCore.Tools.DotNet", projectFileContents);
                Assert.DoesNotContain("Microsoft.Extensions.SecretManager.Tools", projectFileContents);

                foreach (var publish in new[] { false, true })
                {
                    // Arrange
                    using (var aspNetProcess = StartAspNetProcess(targetFrameworkOverride, publish))
                    {
                        // Act
                        var testResult = await RunTest("mvc");

                        // Assert
                        AssertNpmTest.Success(testResult);
                        Assert.Contains("Test Suites: 1 passed, 1 total", testResult.Output);
                    }
                }
            }
        }
    }

    public class PuppeteerTestsBase : TemplateTestBase
    {
        public PuppeteerTestsBase(ITestOutputHelper output) : base(output)
        {
        }

        private static readonly string TestDir = Path.Join(TestPathUtilities.GetSolutionRootDirectory("Templating"), "test", "Templates.Test");
        protected static readonly string PuppeteerDir = Path.Join(TestDir, "PuppeteerTests");

        protected async Task<ProcessResult> RunTest(string test)
        {
            var testDir = Path.Join(PuppeteerDir, test);
            ProcessStartInfo processStartInfo;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                processStartInfo = new ProcessStartInfo
                {
                    FileName = "cmd",
                    WorkingDirectory = testDir,
                    Arguments = $"/c npm test"
                };
            }
            else
            {
                processStartInfo = new ProcessStartInfo
                {
                    FileName = "npm",
                    WorkingDirectory = testDir,
                    Arguments = $"test"
                };
            }

            // Act
            return await ProcessManager.RunProcessAsync(processStartInfo);
        }
    }

    public readonly struct PuppeteerTestResult : IDisposable
    {
        public PuppeteerTestResult(ApplicationDeployer deployer, DeploymentResult result)
        {
            Deployer = deployer;
            Result = result;
        }

        public ApplicationDeployer Deployer { get; }

        public DeploymentResult Result { get; }

        public void Dispose()
        {
            Deployer.Dispose();
        }
    }
}
