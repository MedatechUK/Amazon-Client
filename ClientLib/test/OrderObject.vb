Imports MarketplaceWebServiceOrders
Imports MarketplaceWebServiceOrders.Model
Imports System.Collections.Generic

' Object to hold order data 
' Received from Amazon API

Class order

    Private _order As Global.MarketplaceWebServiceOrders.Model.Order
    Public Sub New(ByRef mp As MarketplaceWebServiceOrders.Marketplace, ByRef order As Model.Order, ByRef cl As OrderAPI)

        Dim response As IMWSResponse

        _order = order
        With _order
            response = mp.InvokeListOrderItems(.AmazonOrderId)
            With TryCast(response, ListOrderItemsResponse)
                For Each i As Model.OrderItem In .ListOrderItemsResult.OrderItems
                    _Items.Add(i)
                Next
                cl.NextToken = .ListOrderItemsResult.NextToken
            End With
            While Not IsNothing(cl.NextToken)
                response = mp.InvokeListOrderItemsByNextToken(cl.NextToken)
                With TryCast(response, ListOrderItemsByNextTokenResponse)
                    For Each i As Model.OrderItem In .ListOrderItemsByNextTokenResult.OrderItems
                        _Items.Add(i)
                    Next
                    cl.NextToken = .ListOrderItemsByNextTokenResult.NextToken
                End With
            End While
        End With

    End Sub

    Public ReadOnly Property Order() As Global.MarketplaceWebServiceOrders.Model.Order
        Get
            Return _order
        End Get
    End Property

    Private _Items As New List(Of Global.MarketplaceWebServiceOrders.Model.OrderItem)
    Public Property Items() As List(Of Global.MarketplaceWebServiceOrders.Model.OrderItem)
        Get
            Return _Items
        End Get
        Set(ByVal value As List(Of Global.MarketplaceWebServiceOrders.Model.OrderItem))
            _Items = value
        End Set
    End Property

End Class