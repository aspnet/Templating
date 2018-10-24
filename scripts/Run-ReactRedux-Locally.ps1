#!/usr/bin/env pwsh
#requires -version 4

[CmdletBinding(PositionalBinding = $false)]
param()

Set-StrictMode -Version 2
$ErrorActionPreference = 'Stop'

. $PSScriptRoot\Test-Template.ps1

Test-Template "reactredux" "reactredux" "Microsoft.DotNet.Web.Spa.ProjectTemplates.2.2.0-rtm-t000.nupkg" $true
