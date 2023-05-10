Imports System.Configuration
Imports System.IO
Imports MISys.API

Module DvineApp

    Sub Main()
        Dim db As New CompanyDB()
        Dim IsLoggedIn As Boolean = False
        Dim IsRESTLoggedIn As Boolean = False
        Dim Issucess As Boolean = True
        Dim FirstRun As Boolean = True
        Dim RestAPI As New RestAPIHelper

        LogService("__________________________", "False")
        LogService("Transfer Service Started", "False")
        Dim WaitingTransfers As List(Of TransferToSalesOrderItem) = db.TransferToSalesOrderItems.Where(Function(a) a.Status = "InProgress").ToList()
        Dim TransQty = WaitingTransfers.Count
        LogService("Found " & TransQty & " Items to transfer", "False")
        For Each oSales In WaitingTransfers
            Try
                If oSales.Qty > 0 Then
                    oSales.LastUpdateDate = Now

                    If FirstRun Then
                        IsRESTLoggedIn = RestAPI.InitAPI().Result()
                        'FirstRun = True
                    End If

                    If IsRESTLoggedIn Then

                        LogService("REST API - Login succesful", "False")

                        Dim processRecord As New ProcessRecord 'New REST API object

                        processRecord._Date = Date.Now.ToString("yyyyMMdd")
                        processRecord.DBMode = 0
                        processRecord.Mode = 1
                        processRecord.ProcessType = 13
                        processRecord.WorkOrderID = oSales.WOID
                        processRecord.WorkOrderDetailID = oSales.LineNbr
                        processRecord.Quantity = oSales.Qty

                        Dim serialLot As New SerialLotAssignment

                        serialLot.itemId = oSales.Itemid
                        serialLot.slId = oSales.LotNo
                        serialLot.quantity = oSales.Qty
                        serialLot.autoCreate = False
                        serialLot.slTransType = 8
                        serialLot.trackType = 1
                        serialLot.wipQuantity = 0
                        serialLot.binTransType = 0
                        'serialLot.binNo = ""
                        serialLot.locId = "DVINE"
                        'serialLot.parentTrackType = 1
                        'serialLot.parentItemId = oSales.Itemid
                        'serialLot.parentSlId = oSales.LotNo

                        Dim errMsg As String = ""

                        Dim result = RestAPI.WOProcess(processRecord, serialLot, errMsg)

                        If result Then
                            Issucess = True
                            oSales.Message = "Sales Transfer Completed Successfully.."
                            oSales.Status = "Completed"
                            LogService(oSales.Message, "False")
                        Else
                            Issucess = False
                            LogService("Error on transaction : " & errMsg, "False")
                            oSales.Message = errMsg
                        End If

                    Else
                        LogService("REST API - Login failed", "False")
                    End If

                    db.SaveChanges()
                    Issucess = True
                End If

            Catch ex As Exception
                oSales.Message = ex.Message
                LogService("Exception - Try Catch", "True")
                LogService(ex.Message, False)
                db.SaveChanges()
            End Try
        Next

        If IsRESTLoggedIn Then
            RestAPI.Logoff()
            IsRESTLoggedIn = False
        End If

        LogService("Ending Transfer Operations", "False")
        LogService("__________________________", "False")
    End Sub

    Public api As MIAPI
    Function InitAPI() As Boolean
        Dim sURL As String = ConfigurationManager.AppSettings("CNS_ServerURL")
        api = New MIAPI(IIf(sURL = "", Nothing, sURL), ConfigurationManager.AppSettings("CNS_CompanyName"), ConfigurationManager.AppSettings("CNS_UserName"), ConfigurationManager.AppSettings("CNS_Password"), useAccountingInterface:=True)
        Dim bRet = api.Logon()
        If bRet Then
            LogService("Login Successful!!!", "False")
            LogService("Logged on as ADMIN with accounting interface.", "True")
            ' here, we're using the AllowedTransactions() call to indirectly confirm
            ' that the accounting interface was loaded. A positive result (that the call
            ' is allowed/available)does indicate that the accounting interface was successfully 
            ' loaded, but a negative result (that the call is unavailable) may be for many reasons,
            ' such as company options, permission checks, and dll load paths as mentioned above.
            Dim transactions As List(Of MIAPI.TransactionType) = api.AllowedTransactions()
            If (transactions.Contains(MIAPI.TransactionType.TransferItemToSales)) Then
                LogService("Status good : TransferItemToSales is allowed", "True")
            Else
                LogService("Status Unexpected: TransferItemToSales is not allowed", True)
            End If
        Else
            If (api.Errors.Count > 0) Then
                LogService("Login Failed : " & api.Errors(0).message, "False")
            End If
        End If
        Return bRet
    End Function

    Public Sub LogService(content As String, debug As String)
        Dim debuglog As Boolean = ConfigurationManager.AppSettings("DebugLogging")
        If debug = False Or debug = debuglog Then
            WriteMessageToFile(content)
        End If
    End Sub

    Private Sub WriteMessageToFile(sMessage As String)
        Dim PostFileLoc As String = ConfigurationManager.AppSettings("LogPath")
        If Not Directory.Exists(PostFileLoc) Then
            Directory.CreateDirectory(PostFileLoc)
        End If
        Dim fs As New FileStream(PostFileLoc & "\DvineConsoleLog.txt", FileMode.OpenOrCreate, FileAccess.Write)
        Dim sw As New StreamWriter(fs)
        sw.BaseStream.Seek(0, SeekOrigin.End)
        sw.WriteLine("----" & Now.ToString() & "----")
        sw.WriteLine(sMessage)
        sw.Flush()
        sw.Close()
    End Sub

End Module
