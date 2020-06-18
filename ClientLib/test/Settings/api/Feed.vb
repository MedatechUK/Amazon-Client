Imports MarketplaceWebService
Imports MarketplaceWebService.Model
Imports System.Collections.Generic
Imports System.IO

Public Class FeedAPI : Inherits clientBase

#Region "Initialisation"

    Public outbox As DirectoryInfo
    Public sent As DirectoryInfo

    Public Sub New()
        _client = New MarketplaceWebServiceClient(accessKey, secretKey, appName, appVersion, Config)

        Dim di As New DirectoryInfo(IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "PriAmazon"))
        With di
            If Not .Exists Then .Create()
        End With

        outbox = New DirectoryInfo(Path.Combine(di.FullName, "outbox"))
        With outbox
            If Not .Exists Then .Create()
        End With

        sent = New DirectoryInfo(Path.Combine(di.FullName, "sent"))
        With sent
            If Not .Exists Then .Create()
        End With

    End Sub

    Private _client As MarketplaceWebServiceClient
    Public ReadOnly Property client() As MarketplaceWebServiceClient
        Get
            Return _client
        End Get
    End Property

#End Region

#Region "Settings"

    Public ReadOnly Property Config() As MarketplaceWebServiceConfig
        Get
            ' Create a configuration object
            Static ret As MarketplaceWebServiceConfig = Nothing
            If IsNothing(ret) Then
                ret = New MarketplaceWebServiceConfig()
                ret.ServiceURL = serviceURL
            End If
            Return ret
        End Get
    End Property

    Public Overrides ReadOnly Property serviceURL() As String
        Get
            Return "https://mws.amazonservices.co.uk"
        End Get
    End Property


#End Region

    Public Function FileQueue() As Queue(Of FeedFile)
        Dim ret As New Queue(Of FeedFile)
        outbox.Refresh()
        For Each sf As DirectoryInfo In outbox.GetDirectories
            For Each fi As FileInfo In sf.GetFiles()

                Dim doc As New Xml.XmlDocument()
                Dim xmldecl As Xml.XmlDeclaration

                xmldecl = doc.CreateXmlDeclaration("1.0", Nothing, Nothing)
                xmldecl.Encoding = "UTF-8"

                doc.Load(fi.FullName)
                doc.InsertBefore(xmldecl, doc.DocumentElement)
                doc.Save(fi.FullName)

                ret.Enqueue(New FeedFile(fi, sent))
            Next
        Next
        Return ret
    End Function

End Class

Public Class FeedFile

    Private _fi As FileInfo
    Private _dest As DirectoryInfo

    Sub New(ByVal fi As FileInfo, ByVal dest As DirectoryInfo)
        _fi = fi
        _dest = dest
    End Sub

    Public ReadOnly Property Source() As FileInfo
        Get
            Return _fi
        End Get
    End Property

    Private ReadOnly Property DestinationPath() As DirectoryInfo
        Get
            Static di As DirectoryInfo = Nothing
            If IsNothing(di) Then
                di = New DirectoryInfo( _
                    Path.Combine( _
                        _dest.FullName, _
                        String.Format( _
                            "{0}\{1}-{2}-{3}", _
                            _fi.Directory.Name, _
                            DatePart(DateInterval.Year, Now).ToString, _
                            Right("00" & DatePart(DateInterval.Month, Now).ToString, 2), _
                            Right("00" & DatePart(DateInterval.Day, Now).ToString, 2) _
                        ) _
                    ) _
                )
                With di
                    If Not .Exists Then .Create()
                End With
            End If
            Return di
        End Get
    End Property

    Private _fStream As Stream
    Public Property fStream() As Stream
        Get
            Return _fStream
        End Get
        Set(ByVal value As Stream)
            _fStream = value
        End Set
    End Property

    Public Sub Move(Optional ByVal Newfile As String = Nothing)

        If IsNothing(Newfile) Then _
            Newfile = _fi.Name

        If Not IsNothing(fStream) Then _
            fStream.Close()

        Dim retry As Boolean
        Do
            retry = False
            Try
                File.Move( _
                    _fi.FullName, _
                    Path.Combine( _
                        DestinationPath.FullName, _
                        Newfile _
                    ) _
                )

            Catch ex As Exception
                retry = True
                Threading.Thread.Sleep(500)

            End Try

        Loop While retry

    End Sub

End Class
