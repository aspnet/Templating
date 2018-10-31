// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.AspNetCore.Testing.xunit;
using Templates.Test.Infrastructure;
using Xunit;
using Xunit.Abstractions;

[assembly: AssemblyFixture(typeof(SeleniumServerFixture))]
// Turn off parallel test run for Edge as the driver does not support multiple Selenium tests at the same time
#if EDGE
[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly)]
#endif
[assembly: TestFramework("Templates.Test.Helpers.XunitExtensions.XunitTestFrameworkWithAssemblyFixture", "Templates.Test.Common")]
namespace Templates.Test
{
    public class MvcTemplateTest : BrowserTestBase
    {
        public MvcTemplateTest(BrowserFixture browserFixture, ITestOutputHelper output) : base(browserFixture, output)
        {
        }

        [ConditionalFact]
        [OSSkipCondition(OperatingSystems.Linux | OperatingSystems.MacOSX)]
        public void MvcTemplate_NoAuth_Works_NetFramework_ForDefaultTemplate()
            => MvcTemplate_NoAuthImpl("net461", languageOverride: default);

        [ConditionalFact]
        [OSSkipCondition(OperatingSystems.Linux | OperatingSystems.MacOSX)]
        public void MvcTemplate_NoAuth_Works_NetFramework_ForFSharpTemplate()
            => MvcTemplate_NoAuthImpl("net461", languageOverride: "F#");

        [ConditionalFact]
        [OSSkipCondition(OperatingSystems.Linux | OperatingSystems.MacOSX)]
        public void MvcTemplate_NoAuth_NoHttps_Works_NetFramework_ForDefaultTemplate()
            => MvcTemplate_NoAuthImpl("net461", languageOverride: default, true);

        [ConditionalFact]
        [OSSkipCondition(OperatingSystems.Linux | OperatingSystems.MacOSX)]
        public void MvcTemplate_NoAuth_NoHttps_Works_NetFramework_ForFSharpTemplate()
            => MvcTemplate_NoAuthImpl("net461", languageOverride: "F#", true);

        [Fact]
        public void MvcTemplate_NoAuth_Works_NetCore_ForDefaultTemplate()
            => MvcTemplate_NoAuthImpl(null, languageOverride: default);

        [Fact(Skip = "https://github.com/aspnet/Templating/issues/673")]
        public void MvcTemplate_NoAuth_Works_NetCore_ForFSharpTemplate()
            => MvcTemplate_NoAuthImpl(null, languageOverride: "F#");

        private static readonly IEnumerable<string> NoAuthUrls = new string[] {
            "/",
            "/Home/Privacy"
        };

        private void MvcTemplate_NoAuthImpl(string targetFrameworkOverride, string languageOverride, bool noHttps = false)
        {
            RunDotNetNew("mvc", targetFrameworkOverride, language: languageOverride, noHttps: noHttps);

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

            if (targetFrameworkOverride != null)
            {
                if (noHttps)
                {
                    Assert.DoesNotContain("Microsoft.AspNetCore.HttpsPolicy", projectFileContents);
                }
                else
                {
                    Assert.Contains("Microsoft.AspNetCore.HttpsPolicy", projectFileContents);
                }
            }

            foreach (var publish in new[] { false, true })
            {
                using (var aspNetProcess = StartAspNetProcess(targetFrameworkOverride, publish))
                {
                    TestBasicNavigation(aspNetProcess, NoAuthUrls);
                }
            }
        }

        [ConditionalFact]
        [OSSkipCondition(OperatingSystems.Linux | OperatingSystems.MacOSX)]
        public void MvcTemplate_IndividualAuth_Works_NetFramework()
            => MvcTemplate_IndividualAuthImpl("net461");

        [ConditionalFact]
        [OSSkipCondition(OperatingSystems.Linux | OperatingSystems.MacOSX)]
        public void MvcTemplate_WithIndividualAuth_NoHttpsSetToTrue_UsesHttps_NetFramework()
            => MvcTemplate_IndividualAuthImpl("net461", false, true);

        [Fact]
        public void MvcTemplate_IndividualAuth_Works_NetCore()
            => MvcTemplate_IndividualAuthImpl(null);

        [Fact]
        public void MvcTemplate_IndividualAuth_UsingLocalDB_Works_NetCore()
            => MvcTemplate_IndividualAuthImpl(null, true);

        private static readonly IEnumerable<string> AuthUrls = new string[] {
            "/",
            "/Home/Privacy",
            "/Identity/Account/Register",
            "/Identity/Account/Login",
            "/Identity/Account/ForgotPassword"
        };

        private void MvcTemplate_IndividualAuthImpl(string targetFrameworkOverride, bool useLocalDB = false, bool noHttps = false)
        {
            RunDotNetNew("mvc", targetFrameworkOverride, auth: "Individual", useLocalDB: useLocalDB);

            AssertDirectoryExists("Extensions", shouldExist: false);
            AssertFileExists("urlRewrite.config", shouldExist: false);
            AssertFileExists("Controllers/AccountController.cs", shouldExist: false);

            var projectFileContents = ReadFile($"{ProjectName}.csproj");
            if (!useLocalDB)
            {
                Assert.Contains(".db", projectFileContents);
            }

            if (targetFrameworkOverride != null)
            {
                Assert.Contains("Microsoft.AspNetCore.HttpsPolicy", projectFileContents);
            }

            RunDotNetEfCreateMigration("mvc");

            AssertEmptyMigration("mvc");

            foreach (var publish in new[] { false, true })
            {
                using (var aspNetProcess = StartAspNetProcess(targetFrameworkOverride, publish))
                {
                    TestBasicNavigation(aspNetProcess, AuthUrls);
                }
            }
        }
    }
}
