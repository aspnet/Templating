#!/usr/bin/env powershell
#requires -version 4

[CmdletBinding(PositionalBinding = $false)]
param()

# BEWARE: This script makes changes to source files which you will have to seperate from any changes you want to keep before commiting.

Set-StrictMode -Version 2
$ErrorActionPreference = 'Stop'
git clean -xdf

$projects = "$PSScriptRoot/../src/Microsoft.DotNet.Web.ProjectTemplates"

./build.cmd /t:Package

Push-Location "$projects/content/React-CSharp"
try {
    dotnet run
}
finally {
    Pop-Location
}
