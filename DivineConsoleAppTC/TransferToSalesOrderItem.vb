Imports System
Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema
Imports System.Data.Entity.Spatial

<Table("TransferToSalesOrderItem")>
Partial Public Class TransferToSalesOrderItem
    Public Property ID As Integer

    <StringLength(12)>
    Public Property WOID As String

    Public Property LineNbr As Integer

    <StringLength(24)>
    Public Property Itemid As String

    <Column(TypeName:="numeric")>
    Public Property Qty As Decimal

    Public Property LotNo As String

    Public Property Status As String

    Public Property Message As String

    Public Property LastUpdateDate As Date?
End Class
