ECHO on
echo Make sure to have ran build.cmd once to ensure artifacts have been created.
rd "Company.WebApplication1" /s /q
mkdir "Company.WebApplication1"
dotnet new razor --auth Individual -i ..\artifacts\build\Microsoft.DotNet.Web.ProjectTemplates.2.1.2.1.0-preview3-t000.nupkg -o Company.WebApplication1
copy "Company.WebApplication1\Data\Migrations\*" "..\src\Microsoft.DotNet.Web.ProjectTemplates\content\RazorPagesWeb-CSharp\Data\SqlLite"
rd "Company.WebApplication1" /s /q
mkdir "Company.WebApplication1"
dotnet new razor --auth Individual --use-local-db -i ..\artifacts\build\Microsoft.DotNet.Web.ProjectTemplates.2.1.2.1.0-preview3-t000.nupkg -o Company.WebApplication1
copy "Company.WebApplication1\Data\Migrations\*" "..\src\Microsoft.DotNet.Web.ProjectTemplates\content\RazorPagesWeb-CSharp\Data\SqlServer"
rd "Company.WebApplication1" /s /q
