Imports MarketplaceWebServiceOrders
Imports MarketplaceWebServiceOrders.Model

Public Class mwsQuery : Implements IDisposable

    Public Enum Preset
        LASTHOUR
        LASTDAY
    End Enum

#Region "Instantiation"

    Public Sub New()

    End Sub

    Public Sub New(ByVal createdAfter As DateTime)
        _createdAfter = createdAfter
        _createdAfterSet = True
    End Sub

    Public Sub New(ByVal PresetOption As Preset)

        Select Case PresetOption
            Case Preset.LASTHOUR
                _createdAfter = DateAdd(DateInterval.Minute, -60, Now)
                _createdAfterSet = True

            Case Preset.LASTDAY
                _createdAfter = DateAdd(DateInterval.Day, -1, Now)
                _createdAfterSet = True

        End Select

    End Sub

#End Region

#Region "Query Properties"

    Private _createdAfter As DateTime
    Private _createdAfterSet As Boolean = False
    Public Property createdAfter() As DateTime
        Get
            If _createdAfterSet Then
                Return _createdAfter
            Else
                Return Nothing
            End If
        End Get
        Set(ByVal value As DateTime)
            _createdAfterSet = True
            _createdAfter = value
        End Set
    End Property

    Private _createdBefore As New DateTime
    Private _createdBeforeSet As Boolean = False
    Public Property createdBefore() As DateTime
        Get
            Return _createdBefore
        End Get
        Set(ByVal value As DateTime)
            _createdBeforeSet = True
            _createdBefore = value
        End Set
    End Property

    Private _lastUpdatedAfter As New DateTime
    Private _lastUpdatedAfterSet As Boolean = False
    Public Property lastUpdatedAfter() As DateTime
        Get
            Return _lastUpdatedAfter
        End Get
        Set(ByVal value As DateTime)
            _lastUpdatedAfterSet = True
            _lastUpdatedAfter = value
        End Set
    End Property

    Private _lastUpdatedBefore As New DateTime
    Private _lastUpdatedBeforeSet As Boolean = False
    Public Property lastUpdatedBefore() As DateTime
        Get
            Return _lastUpdatedBefore
        End Get
        Set(ByVal value As DateTime)
            _lastUpdatedBeforeSet = True
            _lastUpdatedBefore = value
        End Set
    End Property

    Private _orderStatus As New List(Of String)
    Public Property Orderstatus() As List(Of String)
        Get
            Return _orderStatus
        End Get
        Set(ByVal value As List(Of String))
            _orderStatus = value
        End Set
    End Property

    Private _fulfillmentChannel As New List(Of String)
    Public Property fulfillmentChannel() As List(Of String)
        Get
            Return _fulfillmentChannel
        End Get
        Set(ByVal value As List(Of String))
            _fulfillmentChannel = value
        End Set
    End Property

    Private _paymentMethod As New List(Of String)
    Public Property paymentMethod() As List(Of String)
        Get
            Return _paymentMethod
        End Get
        Set(ByVal value As List(Of String))
            _paymentMethod = value
        End Set
    End Property

    Private _buyerEmail As String = Nothing
    Public Property buyerEmail() As String
        Get
            Return _buyerEmail
        End Get
        Set(ByVal value As String)
            _buyerEmail = value
        End Set
    End Property

    Private _sellerOrderId As String = Nothing
    Public Property sellerOrderId() As String
        Get
            Return _sellerOrderId
        End Get
        Set(ByVal value As String)
            _sellerOrderId = value
        End Set
    End Property

    Private _maxResultsPerPage As Decimal
    Private _maxResultsPerPageSet As Boolean = False
    Public Property maxResultsPerPage() As Decimal
        Get
            Return _maxResultsPerPage
        End Get
        Set(ByVal value As Decimal)
            _maxResultsPerPageSet = True
            _maxResultsPerPage = value
        End Set
    End Property

    Private _tfmShipmentStatus As New List(Of String)
    Public Property tfmShipmentStatus() As List(Of String)
        Get
            Return _tfmShipmentStatus
        End Get
        Set(ByVal value As List(Of String))
            _tfmShipmentStatus = value
        End Set
    End Property

#End Region

#Region "PublicMethods"

    Public Sub QueryRequest(ByVal Request As ListOrdersRequest)

        With Request
            .CreatedAfter = _createdAfter
            If _createdBeforeSet Then
                .CreatedBefore = createdBefore
            End If
            If _lastUpdatedAfterSet Then
                .LastUpdatedAfter = lastUpdatedAfter
            End If
            If _lastUpdatedBeforeSet Then
                .LastUpdatedBefore = lastUpdatedBefore
            End If
            If Orderstatus.Count > 0 Then
                .OrderStatus = Orderstatus
            End If
            If fulfillmentChannel.Count > 0 Then
                .FulfillmentChannel = fulfillmentChannel
            End If
            If paymentMethod.Count > 0 Then
                .PaymentMethod = paymentMethod
            End If
            If Not IsNothing(buyerEmail) Then
                .BuyerEmail = buyerEmail
            End If
            If Not IsNothing(sellerOrderId) Then
                .SellerOrderId = sellerOrderId
            End If
            If _maxResultsPerPageSet Then
                .MaxResultsPerPage = maxResultsPerPage
            End If
            If tfmShipmentStatus.Count > 0 Then
                .TFMShipmentStatus = tfmShipmentStatus
            End If
        End With

    End Sub

#End Region

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
