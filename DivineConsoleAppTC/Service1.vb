Imports System.Configuration
Imports System.IO
Imports Microsoft.VisualBasic.Devices

Public Class Service1
    Dim tmr As Timers.Timer
       Dim isRunning As Boolean = False ' Add a boolean variable to keep track of whether the process is running.

    Protected Overrides Sub OnStart(ByVal args() As String)
        ' Add code here to start your service. This method should set things
        ' in motion so your service can do its work.
        tmr = New Timers.Timer()
        tmr.Interval = 1000 * 60 * 1
        AddHandler tmr.Elapsed, AddressOf mytickhandler
        tmr.Enabled = True
    End Sub

    Protected Overrides Sub OnStop()
        ' Add code here to perform any tear-down necessary to stop your service.
        tmr.Enabled = False
    End Sub

    Private Sub MyTickHandler(obj As Object, e As EventArgs)
        If Not isRunning Then ' Check if the process is already running.
            Try
                isRunning = True ' Set the isRunning flag to indicate that the process is now running.
                DivineConsoleAppTC.DvineApp.Main()

            Catch ex As Exception ' Catch any exceptions thrown by the AutoPost method.
                isRunning = False ' Set the isRunning flag to indicate that the process is now running.
            Finally
                isRunning = False ' Set the isRunning flag to indicate that the process is no longer running.
            End Try
         End If

    End Sub
End Class
