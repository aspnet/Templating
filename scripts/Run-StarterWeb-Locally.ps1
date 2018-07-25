#!/usr/bin/env pwsh -c
#requires -version 4

[CmdletBinding(PositionalBinding = $false)]
param()

# BEWARE: This script makes changes to source files which you will have to seperate from any changes you want to keep before commiting.

Set-StrictMode -Version 2
$ErrorActionPreference = 'Stop'

./build.cmd /t:Package

Push-Location "$PSScriptRoot/../src/Microsoft.DotNet.Web.ProjectTemplates/content/StarterWeb-CSharp"
try {
    $csproj = "Company.WebApplication1.csproj"
    (Get-Content $csproj).replace('netcoreapp2.2', 'netcoreapp2.1') | Set-Content $csproj
    
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
