# Templates

AppVeyor: [![AppVeyor](https://ci.appveyor.com/api/projects/status/github/aspnet/templating?branch=dev&svg=true)](https://ci.appveyor.com/project/aspnetci/templating/branch/dev)

## Getting Started
ASP.NET Templates provide project templates which are used in .NET Core for creating ASP.NET Core applications.

This project is part of ASP.NET Core. You can find samples, documentation and getting started instructions for ASP.NET Core at the [Home](https://github.com/aspnet/home) repo.

## Building Templates
- Running `build.cmd` in this repo requires NPM which can be installed from https://nodejs.org/en/.
- The ASP.NET localhost development certificate must also be installed and trusted or else you'll get a test error "Certificate error: Navigation blocked".
- `build.cmd` (or `build /t:package` to avoid tests) will produce NuGet packages for each class of template in the artifacts directory. These can be installed via `dotnet new -i {nugetpackage path}`
- You also need to get the packages these templates depend on into your package cache or else `dotnet new` restore will fail. The easiest way to get them to run is by letting the build run at least 1 test.

## Building and running all Selenium tests
Prerequisites:
- Java 8 or later
- Chrome/Edge/Firefox

Running `build.cmd` will build and run all tests on Chrome by default. 

It will install and run [selenium-standalone](https://www.npmjs.com/package/selenium-standalone) (requires Java 8 or later) by running the following:
  - `npm install -g selenium-standalone`
  - `selenium-standalone install`
  - `selenium-standalone start`

### To run tests on Microsoft EDGE browser:
Open a new Powershell window and run the following command:
$env:ASPNETCORE_BROWSER_AUTOMATION_EDGE="true"

### To run tests on Mozilla FIREFOX browser:
Open a new Powershell window and run the following command:
$env:ASPNETCORE_BROWSER_AUTOMATION_FIREFOX="true"
