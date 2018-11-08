#!/usr/bin/env pwsh
#requires -version 4

[CmdletBinding(PositionalBinding = $false)]
param()

Set-StrictMode -Version 2
$ErrorActionPreference = 'Stop'

. $PSScriptRoot\Test-Template.ps1

Test-Template "angular" "angular -f netcoreapp3.0" "Microsoft.DotNet.Web.Spa.ProjectTemplates.3.0.0-alpha1-t000.nupkg" $true
