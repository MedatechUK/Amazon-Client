Imports MarketplaceWebServiceOrders
Imports MarketplaceWebServiceOrders.Model
Imports System.Collections.Generic

Public Class OrderAPI : Inherits clientBase

#Region "Initialisation"

    Public Sub New()
        _client = New MarketplaceWebServiceOrdersClient(accessKey, secretKey, appName, appVersion, Config)
    End Sub

    Private _client As MarketplaceWebServiceOrdersClient
    Public ReadOnly Property client() As MarketplaceWebServiceOrdersClient
        Get
            Return _client
        End Get
    End Property

#End Region

#Region "Settings"

    Public ReadOnly Property Config() As MarketplaceWebServiceOrdersConfig
        Get
            ' Create a configuration object
            Static ret As MarketplaceWebServiceOrdersConfig = Nothing
            If IsNothing(ret) Then
                ret = New MarketplaceWebServiceOrdersConfig()
                ret.ServiceURL = serviceURL
            End If
            Return ret
        End Get
    End Property

    Public Overrides ReadOnly Property serviceURL() As String
        Get
            Return "https://mws.amazonservices.co.uk/Orders/2013-09-01"
        End Get
    End Property


#End Region

End Class
