#!/usr/bin/env powershell
#requires -version 4

[CmdletBinding(PositionalBinding = $false)]
param()

Set-StrictMode -Version 2
$ErrorActionPreference = 'Stop'

npm install -g uglifycss
npm install -g uglify-js

$projectContentDir = "$PSScriptRoot/../src/Microsoft.DotNet.Web.ProjectTemplates/content"

$contentDirs = Get-ChildItem -Path $projectContentDir -Directory

foreach ($contentDir in $contentDirs) {
    $wwwRoot = "$projectContentDir\$contentDir\wwwroot"

    $cssFolder = Join-Path $wwwRoot "css"
    $siteCss = Join-Path $cssFolder "site.css"
    $siteMinCss = Join-Path $cssFolder "site.min.css"
    if (Test-Path $siteCss) {
        uglifycss $siteCss > $siteMinCss
    }

    $jsFolder = Join-Path $wwwRoot "js"
    $siteJs = Join-Path $jsFolder "site.js"
    $siteMinJs = Join-Path $jsFolder "site.min.js"
    if (Test-Path $siteJs) {
        uglifyjs $siteJs --output $siteMinJs
    }
}
