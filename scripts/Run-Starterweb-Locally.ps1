#!/usr/bin/env powershell
#requires -version 4

[CmdletBinding(PositionalBinding = $false)]
param()

# BEWARE: This script makes changes to source files which you will have to seperate from any changes you want to keep before commiting.

Set-StrictMode -Version 2
$ErrorActionPreference = 'Stop'
git clean -xdf


$projects = "$PSScriptRoot/../src/Microsoft.DotNet.Web.ProjectTemplates"

$csproj = Join-Path $projects "StarterWeb-CSharp.csproj.in"
(Get-Content $csproj).replace('<PackageReference Include="Microsoft.AspNetCore.App"', '<PackageReference Include="Microsoft.NETCore.App" Version="${MicrosoftNETCoreApp22PackageVersion}" />
    <PackageReference Include="Microsoft.AspNetCore.App"') | Set-Content $csproj

./build.cmd /t:Package

Push-Location "$PSScriptRoot/../src/Microsoft.DotNet.Web.ProjectTemplates/content/StarterWeb-CSharp"
try {
    $sqlServer = "Data\SqlServer"
    if (Test-Path $sqlServer) {
        Remove-Item -Recurse -Force -Path $sqlServer
    }

    $launchSettings = "Properties\launchSettings.json"
    (Get-Content $launchSettings).replace('"sslPort": 0', '') | Set-Content $launchSettings

    dotnet run
}
finally {
    Pop-Location
}
