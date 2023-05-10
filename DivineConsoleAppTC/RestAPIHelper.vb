Imports System.Configuration
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Text
Imports Newtonsoft.Json

Public Class RestAPIHelper

    Private token As String
    Private httpClient As HttpClient

    Public Async Function InitAPI() As Task(Of Boolean)

        httpClient = New HttpClient()
        httpClient.BaseAddress = New Uri(ConfigurationManager.AppSettings("MISYS_EXT_ServerURL"))
        httpClient.DefaultRequestHeaders.Accept.Add(New MediaTypeWithQualityHeaderValue("application/json"))
        Dim status As String
        status = ""

        Dim loginInfo As New LoginInfo
        loginInfo.Company = ConfigurationManager.AppSettings("CNS_CompanyName")
        loginInfo.Username = ConfigurationManager.AppSettings("CNS_UserName")
        loginInfo.Password = ConfigurationManager.AppSettings("CNS_Password")

        Dim response As HttpResponseMessage
        Dim contents As String
        Dim result As LoginResponse

        Try

            Dim content = JsonConvert.SerializeObject(loginInfo)
            Dim stringContent = New StringContent(content, UnicodeEncoding.UTF8, "application/json")
            response = Await httpClient.PostAsync("api/Login", stringContent)
            contents = Await response.Content.ReadAsStringAsync()
            result = JsonConvert.DeserializeObject(Of LoginResponse)(contents)
            token = result.Message
            status = result.Status
            httpClient.DefaultRequestHeaders.Add("Authorization", token)
        Catch ex As Exception
            Console.WriteLine(ex.Message)
        End Try

        If status = "Success" Then
            Return True
        Else
            Return False
        End If

    End Function

    Public Function WOProcess(record As ProcessRecord, slt As SerialLotAssignment, ByRef errMsg As String) As Boolean

        Dim status As String
        status = ""

        Dim response As HttpResponseMessage
        Dim result As String

        Try

            Dim recordContent = JsonConvert.SerializeObject(record)
            Dim sltContent = JsonConvert.SerializeObject(slt)
            Dim contentJson As String
            contentJson = "{""ProcessRecord"":"
            contentJson = contentJson + recordContent
            contentJson = contentJson + ","
            contentJson = contentJson + """SerialLotList"":["
            contentJson = contentJson '+ sltContent
            contentJson = contentJson + "]}"
            Dim stringContent = New StringContent(contentJson, UnicodeEncoding.UTF8, "application/json")


            Dim request = New HttpRequestMessage(New HttpMethod("PATCH"), "process/WOProcess")
            request.Content = stringContent
            response = httpClient.SendAsync(request).Result()
            Dim contents = response.Content.ReadAsStringAsync()
            Dim jsonResult = JsonConvert.DeserializeObject(Of Dictionary(Of String, Object))(contents.Result)
            Dim Jstatus As String = jsonResult("Status")
            Dim Jmessage As String = jsonResult("Message")
            Dim JstartValue As String = jsonResult("Data")(0)("TransactionLogRange")("Start")
            Dim JendValue As String = jsonResult("Data")(0)("TransactionLogRange")("End")
            Dim JtransactionDate As String = jsonResult("Data")(0)("TransactionDate")
            Dim JuserId As String = jsonResult("Data")(0)("UserId")

            Dim Lotresponse As HttpResponseMessage

            Dim LotJsonContent = "{""TranDt"":""" + record._Date + """,""UserID"":""DVINESVC"",""Entry"":" + JstartValue + ",""Detail"":1,""MISLPRC"":[{""LocId"":""" + slt.locId + """,""ItemId"":""" + slt.itemId + """,""LotId"":""" + slt.slId + """,""BinId"":null,""qStk"":" + slt.quantity.ToString() + ",""qWip"":0,""recQty"":""" + slt.quantity.ToString() + """}]}"
            Dim LotstringContent = New StringContent(LotJsonContent, UnicodeEncoding.UTF8, "application/json")
            Dim Lotrequest = New HttpRequestMessage(New HttpMethod("POST"), "api/SLT/Assign")
            Lotrequest.Content = LotstringContent
            Lotresponse = httpClient.SendAsync(Lotrequest).Result()
            Dim Lotcontents = Lotresponse.Content.ReadAsStringAsync()
            Dim LotjsonResult = JsonConvert.DeserializeObject(Of Dictionary(Of String, Object))(Lotcontents.Result)

            If IsNothing(jsonResult) Then
                result = contents.IsCompleted
            Else
                result = jsonResult.Item("Message")
            End If
            status = result


            If response.StatusCode = Net.HttpStatusCode.OK Then
                Return True
            Else
                If response.StatusCode = Net.HttpStatusCode.Unauthorized Then
                    errMsg = response.StatusCode
                Else
                    If result IsNot Nothing Then
                        errMsg = result
                        '********** need to add logic here to check if qty in MISys Decreased ********
                        If errMsg = "There was an error while processing the sales transfer through the app connector." Then
                            errMsg = Net.HttpStatusCode.OK
                            Return True
                        Else
                            errMsg = result
                        End If
                    Else
                        errMsg = response.StatusCode
                    End If
                End If

                Return False
            End If
        Catch ex As Exception
            errMsg = ex.Message
            Return False
        End Try
    End Function

    Public Sub Logoff()

        Dim status As String = ""
        Dim response As HttpResponseMessage

        Try
            Dim stringContent = New StringContent("", UnicodeEncoding.UTF8, "application/json")
            response = httpClient.PostAsync("api/Logoff", stringContent).Result()
        Catch ex As Exception
            Console.WriteLine(ex.Message)
        End Try
        'Call Reset()
    End Sub

    Public Sub Reset()

        Dim status As String = ""
        Dim response As HttpResponseMessage

        Try
            Dim stringContent = New StringContent("", UnicodeEncoding.UTF8, "application/json")
            response = httpClient.PostAsync("api/Reset", stringContent).Result()
        Catch ex As Exception
            Console.WriteLine(ex.Message)
        End Try

    End Sub

End Class


Public Class LoginInfo

    Private _username As String
    Public Property Username() As String
        Get
            Return _username
        End Get
        Set(ByVal value As String)
            _username = value
        End Set
    End Property

    Private _password As String
    Public Property Password() As String
        Get
            Return _password
        End Get
        Set(ByVal value As String)
            _password = value
        End Set
    End Property

    Private _company As String
    Public Property Company() As String
        Get
            Return _company
        End Get
        Set(ByVal value As String)
            _company = value
        End Set
    End Property

End Class

Public Class LoginResponse

    Private _status As String
    Public Property Status() As String
        Get
            Return _status
        End Get
        Set(ByVal value As String)
            _status = value
        End Set
    End Property

    Private _Message As String
    Public Property Message() As String
        Get
            Return _Message
        End Get
        Set(ByVal value As String)
            _Message = value
        End Set
    End Property

    Private _Data As Object
    Public Property Data() As Object
        Get
            Return _Data
        End Get
        Set(ByVal value As Object)
            _Data = value
        End Set
    End Property

End Class