' Stan edit this!
Imports priority
Imports MarketplaceWebServiceOrders
Imports MarketplaceWebServiceOrders.Model

Public Class loadOrder : Implements IDisposable

    Sub New(ByRef order As Object)

        Console.WriteLine( _
            String.Format( _
                "Loading order {0}.", _
                TryCast(order, PriAmazon.order).Order.AmazonOrderId _
            ) _
        )

        Using erl As New ldef_Order
            With erl

                Dim ponum As String = TryCast(order, PriAmazon.order).Order.PurchaseOrderNumber
                If IsNothing(ponum) Then ponum = String.Empty

                .AddRecordType(1) = New LoadRow( _
                    ponum, _
                    TryCast(order, PriAmazon.order).Order.OrderStatus, _
                    DateDiff(DateInterval.Minute, #1/1/1988#, TryCast(order, PriAmazon.order).Order.PurchaseDate) _
               )

                For Each line As Model.OrderItem In TryCast(order, PriAmazon.order).Items
                    If Not IsNothing(line.ItemPrice) Then
                        .AddRecordType(2) = New LoadRow( _
                            line.SellerSKU, _
                            line.QuantityOrdered, _
                            line.ItemPrice.Amount, _
                           DateDiff(DateInterval.Minute, #1/1/1988#, NotNothing(TryCast(order, PriAmazon.order).Order.EarliestDeliveryDate)) _
                    )
                    Else
                        .AddRecordType(2) = New LoadRow( _
                            line.SellerSKU, _
                            line.QuantityOrdered, _
                            "0", _
                           DateDiff(DateInterval.Minute, #1/1/1988#, NotNothing(TryCast(order, PriAmazon.order).Order.EarliestDeliveryDate)) _
                        )
                    End If

                    .AddRecordType(3) = New LoadRow( _
                   NotNothing(TryCast(order, PriAmazon.order).Order.BuyerName), _
                   NotNothing(TryCast(order, PriAmazon.order).Order.ShippingAddress.Phone), _
                   NotNothing(TryCast(order, PriAmazon.order).Order.BuyerEmail), _
                   NotNothing(TryCast(order, PriAmazon.order).Order.ShippingAddress.AddressLine1), _
                   NotNothing(TryCast(order, PriAmazon.order).Order.ShippingAddress.AddressLine2), _
                   NotNothing(TryCast(order, PriAmazon.order).Order.ShippingAddress.AddressLine3), _
                   NotNothing(TryCast(order, PriAmazon.order).Order.ShippingAddress.City), _
                   NotNothing(TryCast(order, PriAmazon.order).Order.ShippingAddress.PostalCode) _
               )
                Next

                Dim exp As New Exception
                .Post("http://localhost:8080/loadhandler.ashx", exp)

            End With
        End Using

    End Sub

    Private Function NotNothing(ByRef Value As String) As String
        If IsNothing(Value) Then
            Return String.Empty
        Else
            Return Value
        End If
    End Function

    Private Function NotNothing(ByRef value As DateTime) As DateTime
        If value = #12:00:00 AM# Then
            Return Now
        Else
            Return value
        End If
    End Function

#Region " IDisposable Support "

    Private disposedValue As Boolean = False        ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: free other state (managed objects).
            End If

            ' TODO: free your own state (unmanaged objects).
            ' TODO: set large fields to null.
        End If
        Me.disposedValue = True
    End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub

#End Region

End Class
