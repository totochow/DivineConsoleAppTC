Imports System
Imports System.ComponentModel.DataAnnotations.Schema
Imports System.Data.Entity
Imports System.Linq

Partial Public Class CompanyDB
    Inherits DbContext

    Public Sub New()
        MyBase.New("name=CompanyDB")
    End Sub

    Public Overridable Property TransferToSalesOrderItems As DbSet(Of TransferToSalesOrderItem)

    Protected Overrides Sub OnModelCreating(ByVal modelBuilder As DbModelBuilder)
        modelBuilder.Entity(Of TransferToSalesOrderItem)() _
            .Property(Function(e) e.Qty) _
            .HasPrecision(20, 6)
    End Sub
End Class
