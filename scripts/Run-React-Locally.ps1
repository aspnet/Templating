#!/usr/bin/env powershell
#requires -version 4

[CmdletBinding(PositionalBinding = $false)]
param()

# BEWARE: This script makes changes to source files which you will have to seperate from any changes you want to keep before commiting.

Set-StrictMode -Version 2
$ErrorActionPreference = 'Stop'

./build.cmd /t:Package

$spaTemplates = "$PSScriptRoot/../src/Microsoft.DotNet.Web.Spa.ProjectTemplates"
Push-Location "$spaTemplates/content/React-CSharp"
try {
    $csproj = "Company.WebApplication1.csproj"
    $dirBuildTarget = "$spaTemplates/content/Directory.Build.targets"
    (Get-Content $dirBuildTarget).Replace("<NETCoreAppMaximumVersion>99.9</NETCoreAppMaximumVersion>", "") | Set-Content $dirBuildTarget

    $launchSettings = "Properties\launchSettings.json"
    (Get-Content $launchSettings).replace('"sslPort": 0', '') | Set-Content $launchSettings 

    Push-Location "ClientApp"
    try {
        npm install
    }
    finally {
        Pop-Location
    }

    dotnet run
}
finally {
    Pop-Location
}
