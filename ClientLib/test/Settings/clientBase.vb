Public MustInherit Class clientBase

    Public MustOverride ReadOnly Property serviceURL() As String

    Public ReadOnly Property appName() As String
        Get
            ' The client application name
            Return "PriorityERP"
        End Get
    End Property

    Public ReadOnly Property appVersion() As String
        Get
            Return "1.0"
        End Get
    End Property

    Public ReadOnly Property sellerId() As String
        Get
            Return "ATGN7EVTRL8EE"
        End Get
    End Property

    Public ReadOnly Property mwsAuthToken() As String
        Get
            Return "amzn.mws.86a4492d-ec55-dea3-17a5-337e307c4db7"
        End Get
    End Property

    Public ReadOnly Property marketplaceId() As List(Of String)
        Get
            Dim ret As New List(Of String)
            With ret
                .Add("A1F83G8C2ARO7P")
            End With
            Return ret
        End Get
    End Property

    Public ReadOnly Property accessKey() As String
        Get
            ' Developer AWS access key
            Return "AKIAJ43GSFWXU4R2AMLQ"
        End Get
    End Property

    Public ReadOnly Property secretKey() As String
        Get
            ' Developer AWS secret key
            Return "U8afJRjVNf04tJt8ni+nodEn5rWN5kjQoZIkVWSH"
        End Get
    End Property

    Private _NextToken As String = Nothing
    Public Property NextToken() As String
        Get
            Return _NextToken
        End Get
        Set(ByVal value As String)
            _NextToken = value
        End Set
    End Property

    Private _Throttled As Boolean = False
    Public Property Throttled() As Boolean
        Get
            Return _Throttled
        End Get
        Set(ByVal value As Boolean)
            _Throttled = value
        End Set
    End Property

End Class
