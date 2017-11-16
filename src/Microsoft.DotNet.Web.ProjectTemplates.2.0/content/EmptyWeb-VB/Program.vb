Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Threading.Tasks
Imports Microsoft.AspNetCore
Imports Microsoft.AspNetCore.Hosting
Imports Microsoft.Extensions.Configuration
Imports Microsoft.Extensions.Logging

Namespace Company.WebApplication1
	Public Class Program
		Public Shared Sub Main(args As String())
			BuildWebHost(args).Run()
		End Sub

		Public Shared Function BuildWebHost(args As String()) As IWebHost
			Return WebHost.CreateDefaultBuilder(args).UseStartup(Of Startup)().Build()
		End Function
	End Class
End Namespace
