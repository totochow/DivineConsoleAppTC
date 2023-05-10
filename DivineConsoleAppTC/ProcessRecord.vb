
Imports Newtonsoft.Json
''' <summary>
''' Process Record for the new MISys REST API 
''' </summary>
Public Class ProcessRecord

    ''' <summary>
    ''' The processing mode to use (0 = PROCESS, 1 = WO_PROCESS)
    ''' </summary>
    ''' <returns></returns>
    Public Property Mode() As String


    ''' <summary>
    ''' The DB transaction mode to use (0 = automatic)
    ''' </summary>
    Private _DBMode As String
    Public Property DBMode() As String
        Get
            Return _DBMode
        End Get
        Set(ByVal value As String)
            _DBMode = value
        End Set
    End Property

    Private _ProcessType As String
    Public Property ProcessType() As String
        Get
            Return _ProcessType
        End Get
        Set(ByVal value As String)
            _ProcessType = value
        End Set
    End Property

    Private _WorkOrderID As String
    Public Property WorkOrderID() As String
        Get
            Return _WorkOrderID
        End Get
        Set(ByVal value As String)
            _WorkOrderID = value
        End Set
    End Property

    Private _WorkOrderDetailID As Integer
    Public Property WorkOrderDetailID() As Integer
        Get
            Return _WorkOrderDetailID
        End Get
        Set(ByVal value As Integer)
            _WorkOrderDetailID = value
        End Set
    End Property

    Private _Quantity As Integer
    Public Property Quantity() As Integer
        Get
            Return _Quantity
        End Get
        Set(ByVal value As Integer)
            _Quantity = value
        End Set
    End Property

    <JsonProperty(PropertyName:="Date")>
    Public Property _Date() As String

End Class
