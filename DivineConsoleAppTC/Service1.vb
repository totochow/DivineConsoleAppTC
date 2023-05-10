Public Class Service1
    Dim tmr As Timers.Timer
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
        DivineConsoleAppTC.DvineApp.Main()
    End Sub
End Class
