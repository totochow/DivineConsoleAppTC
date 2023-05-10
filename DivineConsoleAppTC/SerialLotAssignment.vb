Public Class SerialLotAssignment

    Public Property itemId As String

    Public Property binNo As String

    Public Property quantity As Integer

    Public Property slId As String

    Public Property wipQuantity As Integer

    Public Property autoCreate As Boolean

    Public Property parentItemId As String

    Public Property parentSlId As String

    ''' <summary>
    ''' 8 = Transfer to Sales. 40 = Transfer from sales
    ''' </summary>
    ''' <returns></returns>
    Public Property slTransType As Integer

    Public Property binTransType As Integer = 0

    Public Property locId As String

    ''' <summary>
    ''' 0 = NONE, 1 = LOT, 2 = SERIAL
    ''' </summary>
    ''' <returns></returns>
    Public Property trackType As Integer

    Public Property parentTrackType As Integer

End Class
