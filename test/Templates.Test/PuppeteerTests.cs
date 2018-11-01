using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Server.IntegrationTesting;
using Microsoft.Extensions.Logging;
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

        private async Task MvcTemplate_NoAuth(string targetFrameworkOverride, string languageOverride, bool noHttps = false)
        {
            using (StartLog(out var loggerFactory))
            {
                foreach (var publish in new[] { false, true })
                {
                    using (var deploymentResult = await CreateDeployments(loggerFactory, "mvc", targetFrameworkOverride, languageOverride, auth: null, publish: publish, noHttps: noHttps))
                    {
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

                        // Act
                        var testResult = await RunTest(deploymentResult, "mvctests.js");

                        // Assert
                        Assert.Success(testResult);
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

        protected async Task<PuppeteerTestResult> CreateDeployments(
            ILoggerFactory loggerFactory,
            string template,
            string targetFrameworkOverride,
            string languageOverride,
            string auth,
            bool publish,
            bool noHttps = false)
        {
            RunDotNetNew(template, targetFrameworkOverride, auth, language: languageOverride, noHttps);

            var runtimeFlavor = targetFrameworkOverride == null ? RuntimeFlavor.CoreClr : RuntimeFlavor.Clr;
            var applicationType = runtimeFlavor == RuntimeFlavor.Clr ? ApplicationType.Standalone : ApplicationType.Portable;

            var configuration =
#if RELEASE
                "Release";
#else
                "Debug";
#endif

            var parameters = new DeploymentParameters
            {
                RuntimeFlavor = runtimeFlavor,
                ServerType = ServerType.Kestrel,
                ApplicationPath = TemplateOutputDir,
                PublishApplicationBeforeDeployment = publish,
                ApplicationType = applicationType,
                Configuration = configuration
            };

            var factory = ApplicationDeployerFactory.Create(parameters, loggerFactory);
            var deployment = await factory.DeployAsync();

            return new PuppeteerTestResult(factory, deployment);
        }

        protected async Task<ProcessResult> RunTest(PuppeteerTestResult deployment, string testFile)
        {
            ProcessStartInfo processStartInfo;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                processStartInfo = new ProcessStartInfo
                {
                    FileName = "cmd",
                    Arguments = $"/c npm jest --no-color -- {testFile}",
                };
            }
            else
            {
                processStartInfo = new ProcessStartInfo
                {
                    FileName = "npm",
                    Arguments = $"jest {testFile}",
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
