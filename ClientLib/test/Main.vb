
'******************************************************************************
' * Copyright 2009-2015 Amazon Services. All Rights Reserved.
' * Licensed under the Apache License, Version 2.0 (the "License"); 
' *
' * You may not use this file except in compliance with the License. 
' * You may obtain a copy of the License at: http://aws.amazon.com/apache2.0
' * This file is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
' * CONDITIONS OF ANY KIND, either express or implied. See the License for the 
' * specific language governing permissions and limitations under the License.
' *******************************************************************************
' * Marketplace Web Service Orders
' * API Version: 2013-09-01
' * Library Version: 2015-09-24
' * Generated: Fri Sep 25 20:06:25 GMT 2015
' 
Imports MarketplaceWebService
Imports MarketplaceWebService.Model

Imports MarketplaceWebServiceOrders
Imports MarketplaceWebServiceOrders.Model

Imports System.Collections.Generic
Imports System.IO
Imports System.Globalization

Namespace MarketplaceWebServiceOrders

    Public Class Marketplace

        Public Shared ordercl As New OrderAPI
        Public Shared feedcl As New FeedAPI

        Private ReadOnly orderClient As MarketplaceWebServiceOrdersClient
        Private ReadOnly feedClient As MarketplaceWebServiceClient

        Public Sub New(ByVal orderclient As MarketplaceWebServiceOrdersClient, ByVal feedclient As MarketplaceWebServiceClient)
            Me.orderClient = orderclient
            Me.feedClient = feedclient

        End Sub

        Public Shared Sub Main(ByVal args As String())


            Dim doc As New Xml.XmlDocument()
            Dim xmldecl As Xml.XmlDeclaration

            xmldecl = doc.CreateXmlDeclaration("1.0", Nothing, Nothing)
            xmldecl.Encoding = "UTF-8"

            doc.Load("c:\available.xml")
            doc.InsertBefore(xmldecl, doc.DocumentElement)
            doc.Save("c:\available.xml")

            Dim mp As New Marketplace(ordercl.client, feedcl.client)

            Dim Complete As New Queue(Of FeedSubmissionInfo)
            Dim Cancelled As New Queue(Of FeedSubmissionInfo)

            Try
                ' DoOrders(mp)
                WaitProcessing(mp, SubmitFeedID(mp), Cancelled, Complete)
                GetResults(mp, Complete)

            Catch ex As MarketplaceWebServiceOrdersException
                Console.WriteLine("Order API: Unexpected exit.")

            Catch ex As MarketplaceWebServiceException
                Console.WriteLine("Feed API: Unexpected exit.")

            Finally
                Console.Read()

            End Try



        End Sub

#Region "Operations"

        Shared Sub DoOrders(ByRef mp As Marketplace)

            Dim orders As New Queue(Of order)
            Dim response As Object

            With mp
                Using query As New mwsQuery(mwsQuery.Preset.LASTHOUR)
                    With query
                        ' Sat additional query parameters here

                    End With
                    response = .InvokeListOrders(query)
                End Using

                With TryCast(response, ListOrdersResponse)
                    For Each o As Global.MarketplaceWebServiceOrders.Model.Order In .ListOrdersResult.Orders
                        orders.Enqueue(New order(mp, o, ordercl))
                    Next
                    ordercl.NextToken = .ListOrdersResult.NextToken
                End With

                While Not IsNothing(ordercl.NextToken)
                    response = .InvokeListOrdersByNextToken(ordercl.NextToken)
                    With TryCast(response, ListOrdersByNextTokenResponse)
                        For Each o As Global.MarketplaceWebServiceOrders.Model.Order In .ListOrdersByNextTokenResult.Orders
                            orders.Enqueue(New order(mp, o, ordercl))
                        Next
                        ordercl.NextToken = .ListOrdersByNextTokenResult.NextToken
                    End With
                End While

                While orders.Count > 0
                    Using this As New loadOrder(orders.Dequeue)
                        Threading.Thread.Sleep(500)
                    End Using
                End While

            End With
        End Sub

        Shared Function SubmitFeedID(ByVal mp As Marketplace) As List(Of String)

            Dim response As Object
            Dim fq As Queue(Of FeedFile) = feedcl.FileQueue
            Dim Submitted As New List(Of String)

            With mp
                While fq.Count > 0
                    Dim ff As FeedFile = fq.Dequeue
                    response = .InvokeSubmitFeed(ff)
                    With TryCast( _
                        response,  _
                        SubmitFeedResponse _
                    )
                        Submitted.Add(.SubmitFeedResult.FeedSubmissionInfo.FeedSubmissionId)
                        ff.Move( _
                            String.Format( _
                                "{0}.xml", _
                                .SubmitFeedResult.FeedSubmissionInfo.FeedSubmissionId _
                            ) _
                       )
                    End With
                End While
            End With

            Return Submitted

        End Function

        Shared Sub WaitProcessing( _
            ByRef mp As Marketplace, _
            ByVal Submitted As List(Of String), _
            ByRef Cancelled As Queue(Of FeedSubmissionInfo), _
            ByRef Complete As Queue(Of FeedSubmissionInfo) _
        )

            Dim response As Object
            With mp

                Do While Submitted.Count > 0
                    response = .InvokeGetFeedSubmissionList(Submitted)
                    With TryCast(response, GetFeedSubmissionListResponse).GetFeedSubmissionListResult
                        For Each info As FeedSubmissionInfo In .FeedSubmissionInfo
                            Select Case info.FeedProcessingStatus
                                Case "_CANCELLED_"
                                    Submitted.Remove(info.FeedSubmissionId)
                                    Cancelled.Enqueue(info)

                                Case "_DONE_"
                                    Submitted.Remove(info.FeedSubmissionId)
                                    Complete.Enqueue(info)

                            End Select
                        Next
                        feedcl.NextToken = .NextToken
                    End With

                    While Not IsNothing(feedcl.NextToken)
                        response = .InvokeGetFeedSubmissionListByNextToken(feedcl.NextToken)
                        With TryCast(response, GetFeedSubmissionListByNextTokenResponse).GetFeedSubmissionListByNextTokenResult
                            For Each info As FeedSubmissionInfo In .FeedSubmissionInfo
                                Select Case info.FeedProcessingStatus
                                    Case "_CANCELLED_"
                                        Submitted.Remove(info.FeedSubmissionId)
                                        Cancelled.Enqueue(info)

                                    Case "_DONE_"
                                        Submitted.Remove(info.FeedSubmissionId)
                                        Complete.Enqueue(info)

                                End Select
                            Next
                            feedcl.NextToken = .NextToken
                        End With
                    End While

                    If Submitted.Count > 0 Then Threading.Thread.Sleep(10000)

                Loop

            End With
        End Sub

        Shared Sub GetResults(ByRef mp As Marketplace, ByRef Complete As Queue(Of FeedSubmissionInfo))

            Dim response As Object

            While Complete.Count > 0
                With Complete.Dequeue

                    Dim sd As DateTime = DateTime.Parse(.SubmittedDate, Nothing, System.Globalization.DateTimeStyles.RoundtripKind)
                    Dim di As New DirectoryInfo( _
                        Path.Combine( _
                            feedcl.sent.FullName, _
                            String.Format( _
                                "{0}\{1}-{2}-{3}", _
                                .FeedType, _
                                DatePart(DateInterval.Year, sd).ToString, _
                                Right("00" & DatePart(DateInterval.Month, sd).ToString, 2), _
                                Right("00" & DatePart(DateInterval.Day, sd).ToString, 2) _
                            ) _
                        ) _
                    )
                    With di
                        If Not .Exists Then .Create()
                    End With

                    response = mp.InvokeGetFeedSubmissionResult(.FeedSubmissionId, di)

                End With
            End While

        End Sub

#End Region

#Region "Invokations"

#Region "Order API"

        Public Function InvokeListOrders(ByRef Query As mwsQuery) As ListOrdersResponse

            ' Create a request.
            Dim ret As ListOrdersResponse = Nothing
            Dim request As New ListOrdersRequest()
            With request
                .SellerId = ordercl.sellerId
                .MWSAuthToken = ordercl.mwsAuthToken
                For Each Market In ordercl.marketplaceId
                    .MarketplaceId.Add(Market)
                Next
                Query.QueryRequest(request)
            End With

            Do
                Try
                    ret = Me.orderClient.ListOrders(request)
                    LogResult(ret)
                    Return ret

                Catch ex As MarketplaceWebServiceOrdersException
                    LogException(ex, ordercl.Throttled)

                End Try
            Loop While ordercl.Throttled

        End Function

        Public Function InvokeListOrdersByNextToken(ByVal nextToken As String) As ListOrdersByNextTokenResponse
            ' Create a request.
            Dim ret As ListOrdersByNextTokenResponse
            Dim request As New ListOrdersByNextTokenRequest()
            With request
                .SellerId = ordercl.sellerId
                .MWSAuthToken = ordercl.mwsAuthToken
                .NextToken = nextToken
            End With

            Do
                Try
                    ret = Me.orderClient.ListOrdersByNextToken(request)
                    LogResult(ret)
                    Return ret

                Catch ex As MarketplaceWebServiceOrdersException
                    LogException(ex, ordercl.Throttled)

                End Try
            Loop While ordercl.Throttled

        End Function

        Public Function InvokeListOrderItems(ByVal amazonOrderId As String) As ListOrderItemsResponse
            ' Create a request.
            Dim ret As ListOrderItemsResponse
            Dim request As New ListOrderItemsRequest()
            With request
                .SellerId = ordercl.sellerId
                .MWSAuthToken = ordercl.mwsAuthToken
                .AmazonOrderId = amazonOrderId
            End With

            Do
                Try
                    ret = Me.orderClient.ListOrderItems(request)
                    LogResult(ret)
                    Return ret

                Catch ex As MarketplaceWebServiceOrdersException
                    LogException(ex, ordercl.Throttled)

                End Try
            Loop While ordercl.Throttled

        End Function

        Public Function InvokeListOrderItemsByNextToken(ByVal nextToken As String) As ListOrderItemsByNextTokenResponse
            ' Create a request.
            Dim ret As ListOrderItemsByNextTokenResponse

            Dim request As New ListOrderItemsByNextTokenRequest()
            With request
                .SellerId = ordercl.sellerId
                .MWSAuthToken = ordercl.mwsAuthToken
                .NextToken = nextToken
            End With

            Do
                Try
                    ret = Me.orderClient.ListOrderItemsByNextToken(request)
                    LogResult(ret)
                    Return ret

                Catch ex As MarketplaceWebServiceOrdersException
                    LogException(ex, ordercl.Throttled)

                End Try
            Loop While ordercl.Throttled

        End Function

#End Region

#Region "Feed API"

        Public Function InvokeSubmitFeed(ByVal ff As FeedFile) As SubmitFeedResponse

            Dim ret As SubmitFeedResponse
            Dim request As New SubmitFeedRequest()

            With request
                .Merchant = feedcl.sellerId
                .MWSAuthToken = feedcl.mwsAuthToken

                ' Optional
                '.MarketplaceIdList = New IdList()
                '.MarketplaceIdList.Id = feedcl.marketplaceId

                ' MWS exclusively offers a streaming interface for uploading your feeds. This is because 
                ' feed sizes can grow to the 1GB+ range - and as your business grows you could otherwise 
                ' silently reach the feed size where your in-memory solution will no longer work, leaving you 
                ' puzzled as to why a solution that worked for a long time suddenly stopped working though 
                ' you made no changes. For the same reason, we strongly encourage you to generate your feeds to 
                ' local disk then upload them directly from disk to MWS.

                ff.fStream = File.Open(ff.Source.FullName, FileMode.Open, FileAccess.Read)

                .FeedContent = ff.fStream

                ' Calculating the MD5 hash value exhausts the stream, and therefore we must either reset the
                ' position, or create another stream for the calculation.
                .ContentMD5 = MarketplaceWebServiceClient.CalculateContentMD5(.FeedContent)
                .FeedContent.Position = 0

                .FeedType = ff.Source.Directory.Name

            End With

            Do
                Try
                    ret = feedClient.SubmitFeed(request)
                    LogResult(ret)
                    Return ret

                Catch ex As MarketplaceWebServiceException
                    LogException(ex, feedcl.Throttled)

                End Try

            Loop While feedcl.Throttled

        End Function

        Public Function InvokeGetFeedSubmissionResult(ByVal FeedSubmissionId As String, ByRef Di As DirectoryInfo) As GetFeedSubmissionResultResponse

            Dim ret As GetFeedSubmissionResultResponse
            Dim request As New GetFeedSubmissionResultRequest()

            With request
                .Merchant = feedcl.sellerId
                .MWSAuthToken = feedcl.mwsAuthToken

                ' Optional
                ' Note that depending on the size of the feed sent in, and the number of errors and warnings,
                ' the result can reach sizes greater than 1GB. For this reason we recommend that you _always_ 
                ' program to MWS in a streaming fashion. Otherwise, as your business grows you may silently reach
                ' the in-memory size limit and have to re-work your solution.
                ' NOTE: Due to Content-MD5 validation, the stream must be read/write.
                .FeedSubmissionId = FeedSubmissionId
                .FeedSubmissionResult = File.Open( _
                    Path.Combine( _
                        Di.FullName, _
                        String.Format( _
                            "{0}_result.xml", _
                            FeedSubmissionId _
                        ) _
                    ), _
                    FileMode.OpenOrCreate, _
                    FileAccess.ReadWrite _
                )

            End With

            Do
                Try
                    ret = feedClient.GetFeedSubmissionResult(request)
                    LogResult(ret)
                    Return ret

                Catch ex As MarketplaceWebServiceException
                    LogException(ex, feedcl.Throttled)

                End Try

            Loop While feedcl.Throttled

        End Function

        Public Function InvokeGetFeedSubmissionList(ByVal Submitted As List(Of String)) As GetFeedSubmissionListResponse

            Dim ret As GetFeedSubmissionListResponse
            Dim request As New GetFeedSubmissionListRequest

            With request
                .Merchant = feedcl.sellerId
                .MWSAuthToken = feedcl.mwsAuthToken
                Dim L As New MarketplaceWebService.Model.IdList()
                L.Id = New List(Of String)
                For Each s As String In Submitted
                    L.Id.Add(s)
                Next
                .FeedSubmissionIdList = L

            End With

            Do
                Try
                    ret = feedClient.GetFeedSubmissionList(request)
                    LogResult(ret)
                    Return ret

                Catch ex As MarketplaceWebServiceException
                    LogException(ex, feedcl.Throttled)

                End Try

            Loop While feedcl.Throttled

        End Function

        Public Function InvokeGetFeedSubmissionListByNextToken(ByVal Token As String) As GetFeedSubmissionListByNextTokenResponse

            Dim ret As GetFeedSubmissionListByNextTokenResponse
            Dim request As New GetFeedSubmissionListByNextTokenRequest

            With request
                .Merchant = feedcl.sellerId
                .MWSAuthToken = feedcl.mwsAuthToken
                .NextToken = Token

            End With

            Do
                Try
                    ret = feedClient.GetFeedSubmissionListByNextToken(request)
                    LogResult(ret)
                    Return ret

                Catch ex As MarketplaceWebServiceException
                    LogException(ex, feedcl.Throttled)

                End Try

            Loop While feedcl.Throttled

        End Function

#End Region

#End Region

#Region "Logging"

        Private Sub LogResult(ByRef response As IMWSResponse)
            Console.WriteLine("Response:")
            Dim rhmd As Global.MarketplaceWebServiceOrders.Model.ResponseHeaderMetadata = response.ResponseHeaderMetadata
            ' We recommend logging the request id and timestamp of every call.
            Console.WriteLine("RequestId: " + rhmd.RequestId)
            Console.WriteLine("Timestamp: " + rhmd.Timestamp)
            Dim responseXml As String = response.ToXML()
            Console.WriteLine(responseXml)
        End Sub

        Private Sub LogResult(ByRef response As SubmitFeedResponse)

            Console.WriteLine("Service Response")
            Console.WriteLine("=============================================================================")
            Console.WriteLine()

            Console.WriteLine("        SubmitFeedResponse")
            If response.IsSetSubmitFeedResult() Then
                Console.WriteLine("            SubmitFeedResult")
                Dim submitFeedResult As SubmitFeedResult = response.SubmitFeedResult
                If submitFeedResult.IsSetFeedSubmissionInfo() Then
                    Console.WriteLine("                FeedSubmissionInfo")
                    Dim feedSubmissionInfo As FeedSubmissionInfo = submitFeedResult.FeedSubmissionInfo
                    If feedSubmissionInfo.IsSetFeedSubmissionId() Then
                        Console.WriteLine("                    FeedSubmissionId")
                        Console.WriteLine("                        {0}", feedSubmissionInfo.FeedSubmissionId)
                    End If
                    If feedSubmissionInfo.IsSetFeedType() Then
                        Console.WriteLine("                    FeedType")
                        Console.WriteLine("                        {0}", feedSubmissionInfo.FeedType)
                    End If
                    If feedSubmissionInfo.IsSetSubmittedDate() Then
                        Console.WriteLine("                    SubmittedDate")
                        Console.WriteLine("                        {0}", feedSubmissionInfo.SubmittedDate)
                    End If
                    If feedSubmissionInfo.IsSetFeedProcessingStatus() Then
                        Console.WriteLine("                    FeedProcessingStatus")
                        Console.WriteLine("                        {0}", feedSubmissionInfo.FeedProcessingStatus)
                    End If
                    If feedSubmissionInfo.IsSetStartedProcessingDate() Then
                        Console.WriteLine("                    StartedProcessingDate")
                        Console.WriteLine("                        {0}", feedSubmissionInfo.StartedProcessingDate)
                    End If
                    If feedSubmissionInfo.IsSetCompletedProcessingDate() Then
                        Console.WriteLine("                    CompletedProcessingDate")
                        Console.WriteLine("                        {0}", feedSubmissionInfo.CompletedProcessingDate)
                    End If
                End If
            End If
            If response.IsSetResponseMetadata() Then
                Console.WriteLine("            ResponseMetadata")
                Dim responseMetadata As Global.MarketplaceWebService.Model.ResponseMetadata = response.ResponseMetadata
                If responseMetadata.IsSetRequestId() Then
                    Console.WriteLine("                RequestId")
                    Console.WriteLine("                    {0}", responseMetadata.RequestId)
                End If
            End If

            Console.WriteLine("            ResponseHeaderMetadata")
            Console.WriteLine("                RequestId")
            Console.WriteLine("                    " + response.ResponseHeaderMetadata.RequestId)
            Console.WriteLine("                ResponseContext")
            Console.WriteLine("                    " + response.ResponseHeaderMetadata.ResponseContext)
            Console.WriteLine("                Timestamp")

            Console.WriteLine("                    " + response.ResponseHeaderMetadata.Timestamp)

        End Sub

        Private Sub LogResult(ByRef response As GetFeedSubmissionListByNextTokenResponse)
            Console.WriteLine("Service Response")
            Console.WriteLine("=============================================================================")
            Console.WriteLine()

            Console.WriteLine("        GetFeedSubmissionListByNextTokenResponse")
            If response.IsSetGetFeedSubmissionListByNextTokenResult() Then
                Console.WriteLine("            GetFeedSubmissionListByNextTokenResult")
                Dim getFeedSubmissionListByNextTokenResult As GetFeedSubmissionListByNextTokenResult = response.GetFeedSubmissionListByNextTokenResult
                If getFeedSubmissionListByNextTokenResult.IsSetNextToken() Then
                    Console.WriteLine("                NextToken")
                    Console.WriteLine("                    {0}", getFeedSubmissionListByNextTokenResult.NextToken)
                End If
                If getFeedSubmissionListByNextTokenResult.IsSetHasNext() Then
                    Console.WriteLine("                HasNext")
                    Console.WriteLine("                    {0}", getFeedSubmissionListByNextTokenResult.HasNext)
                End If
                Dim feedSubmissionInfoList As List(Of FeedSubmissionInfo) = getFeedSubmissionListByNextTokenResult.FeedSubmissionInfo
                For Each feedSubmissionInfo As FeedSubmissionInfo In feedSubmissionInfoList
                    Console.WriteLine("                FeedSubmissionInfo")
                    If feedSubmissionInfo.IsSetFeedSubmissionId() Then
                        Console.WriteLine("                    FeedSubmissionId")
                        Console.WriteLine("                        {0}", feedSubmissionInfo.FeedSubmissionId)
                    End If
                    If feedSubmissionInfo.IsSetFeedType() Then
                        Console.WriteLine("                    FeedType")
                        Console.WriteLine("                        {0}", feedSubmissionInfo.FeedType)
                    End If
                    If feedSubmissionInfo.IsSetSubmittedDate() Then
                        Console.WriteLine("                    SubmittedDate")
                        Console.WriteLine("                        {0}", feedSubmissionInfo.SubmittedDate)
                    End If
                    If feedSubmissionInfo.IsSetFeedProcessingStatus() Then
                        Console.WriteLine("                    FeedProcessingStatus")
                        Console.WriteLine("                        {0}", feedSubmissionInfo.FeedProcessingStatus)
                    End If
                    If feedSubmissionInfo.IsSetStartedProcessingDate() Then
                        Console.WriteLine("                    StartedProcessingDate")
                        Console.WriteLine("                        {0}", feedSubmissionInfo.StartedProcessingDate)
                    End If
                    If feedSubmissionInfo.IsSetCompletedProcessingDate() Then
                        Console.WriteLine("                    CompletedProcessingDate")
                        Console.WriteLine("                        {0}", feedSubmissionInfo.CompletedProcessingDate)
                    End If
                Next
            End If
            If response.IsSetResponseMetadata() Then
                Console.WriteLine("            ResponseMetadata")
                Dim responseMetadata As Global.MarketplaceWebService.Model.ResponseMetadata = response.ResponseMetadata
                If responseMetadata.IsSetRequestId() Then
                    Console.WriteLine("                RequestId")
                    Console.WriteLine("                    {0}", responseMetadata.RequestId)
                End If
            End If

            Console.WriteLine("            ResponseHeaderMetadata")
            Console.WriteLine("                RequestId")
            Console.WriteLine("                    " + response.ResponseHeaderMetadata.RequestId)
            Console.WriteLine("                ResponseContext")
            Console.WriteLine("                    " + response.ResponseHeaderMetadata.ResponseContext)
            Console.WriteLine("                Timestamp")

            Console.WriteLine("                    " + response.ResponseHeaderMetadata.Timestamp)
        End Sub

        Private Sub LogResult(ByRef response As GetFeedSubmissionListResponse)
            Console.WriteLine("Service Response")
            Console.WriteLine("=============================================================================")
            Console.WriteLine()

            Console.WriteLine("        GetFeedSubmissionListResponse")
            If response.IsSetGetFeedSubmissionListResult() Then
                Console.WriteLine("            GetFeedSubmissionListResult")
                Dim getFeedSubmissionListResult As GetFeedSubmissionListResult = response.GetFeedSubmissionListResult
                If getFeedSubmissionListResult.IsSetNextToken() Then
                    Console.WriteLine("                NextToken")
                    Console.WriteLine("                    {0}", getFeedSubmissionListResult.NextToken)
                End If
                If getFeedSubmissionListResult.IsSetHasNext() Then
                    Console.WriteLine("                HasNext")
                    Console.WriteLine("                    {0}", getFeedSubmissionListResult.HasNext)
                End If
                Dim feedSubmissionInfoList As List(Of FeedSubmissionInfo) = getFeedSubmissionListResult.FeedSubmissionInfo
                For Each feedSubmissionInfo As FeedSubmissionInfo In feedSubmissionInfoList
                    Console.WriteLine("                FeedSubmissionInfo")
                    If feedSubmissionInfo.IsSetFeedSubmissionId() Then
                        Console.WriteLine("                    FeedSubmissionId")
                        Console.WriteLine("                        {0}", feedSubmissionInfo.FeedSubmissionId)
                    End If
                    If feedSubmissionInfo.IsSetFeedType() Then
                        Console.WriteLine("                    FeedType")
                        Console.WriteLine("                        {0}", feedSubmissionInfo.FeedType)
                    End If
                    If feedSubmissionInfo.IsSetSubmittedDate() Then
                        Console.WriteLine("                    SubmittedDate")
                        Console.WriteLine("                        {0}", feedSubmissionInfo.SubmittedDate)
                    End If
                    If feedSubmissionInfo.IsSetFeedProcessingStatus() Then
                        Console.WriteLine("                    FeedProcessingStatus")
                        Console.WriteLine("                        {0}", feedSubmissionInfo.FeedProcessingStatus)
                    End If
                    If feedSubmissionInfo.IsSetStartedProcessingDate() Then
                        Console.WriteLine("                    StartedProcessingDate")
                        Console.WriteLine("                        {0}", feedSubmissionInfo.StartedProcessingDate)
                    End If
                    If feedSubmissionInfo.IsSetCompletedProcessingDate() Then
                        Console.WriteLine("                    CompletedProcessingDate")
                        Console.WriteLine("                        {0}", feedSubmissionInfo.CompletedProcessingDate)
                    End If
                Next
            End If
            If response.IsSetResponseMetadata() Then
                Console.WriteLine("            ResponseMetadata")
                Dim responseMetadata As Global.MarketplaceWebService.Model.ResponseMetadata = response.ResponseMetadata
                If responseMetadata.IsSetRequestId() Then
                    Console.WriteLine("                RequestId")
                    Console.WriteLine("                    {0}", responseMetadata.RequestId)
                End If
            End If

            Console.WriteLine("            ResponseHeaderMetadata")
            Console.WriteLine("                RequestId")
            Console.WriteLine("                    " + response.ResponseHeaderMetadata.RequestId)
            Console.WriteLine("                ResponseContext")
            Console.WriteLine("                    " + response.ResponseHeaderMetadata.ResponseContext)
            Console.WriteLine("                Timestamp")

            Console.WriteLine("                    " + response.ResponseHeaderMetadata.Timestamp)

        End Sub

        Private Sub LogResult(ByRef response As GetFeedSubmissionResultResponse)

            Console.WriteLine("Service Response")
            Console.WriteLine("=============================================================================")
            Console.WriteLine()

            Console.WriteLine("        GetFeedSubmissionResultResponse")
            If response.IsSetGetFeedSubmissionResultResult() Then
                Console.WriteLine("            GetFeedSubmissionResult")
                Dim getFeedSubmissionResultResult As GetFeedSubmissionResultResult = response.GetFeedSubmissionResultResult
                If getFeedSubmissionResultResult.IsSetContentMD5() Then
                    Console.WriteLine("                ContentMD5")
                    Console.WriteLine("                    {0}", getFeedSubmissionResultResult.ContentMD5)
                End If
            End If

            If response.IsSetResponseMetadata() Then
                Console.WriteLine("            ResponseMetadata")
                Dim responseMetadata As Global.MarketplaceWebService.Model.ResponseMetadata = response.ResponseMetadata
                If responseMetadata.IsSetRequestId() Then
                    Console.WriteLine("                RequestId")
                    Console.WriteLine("                    {0}", responseMetadata.RequestId)
                End If
            End If

            Console.WriteLine("            ResponseHeaderMetadata")
            Console.WriteLine("                RequestId")
            Console.WriteLine("                    " + response.ResponseHeaderMetadata.RequestId)
            Console.WriteLine("                ResponseContext")
            Console.WriteLine("                    " + response.ResponseHeaderMetadata.ResponseContext)
            Console.WriteLine("                Timestamp")

            Console.WriteLine("                    " + response.ResponseHeaderMetadata.Timestamp)

        End Sub

        Private Sub LogException(ByRef ex As MarketplaceWebServiceOrdersException, ByRef Throttled As Boolean)
            ' Exception properties are important for diagnostics.
            Throttled = CBool(ex.StatusCode = 503)
            Dim rhmd As Global.MarketplaceWebServiceOrders.Model.ResponseHeaderMetadata = ex.ResponseHeaderMetadata
            Console.WriteLine("Service Exception:")
            If rhmd IsNot Nothing Then
                Console.WriteLine("RequestId: " + rhmd.RequestId)
                Console.WriteLine("Timestamp: " + rhmd.Timestamp)
            End If
            Console.WriteLine("Message: " + ex.Message)
            Console.WriteLine("StatusCode: " + ex.StatusCode.ToString)
            Console.WriteLine("ErrorCode: " + ex.ErrorCode)
            Console.WriteLine("ErrorType: " + ex.ErrorType)

            If Throttled Then
                Console.WriteLine("Waiting 10 seconds...")
                Threading.Thread.Sleep(10000)
            Else
                Throw ex
            End If
        End Sub

        Private Sub LogException(ByRef ex As MarketplaceWebServiceException, ByRef Throttled As Boolean)
            ' Exception properties are important for diagnostics.
            Throttled = CBool(ex.StatusCode = 503)
            Dim rhmd As Global.MarketplaceWebService.Model.ResponseHeaderMetadata = ex.ResponseHeaderMetadata
            Console.WriteLine("Service Exception:")
            If rhmd IsNot Nothing Then
                Console.WriteLine("RequestId: " + rhmd.RequestId)
                Console.WriteLine("Timestamp: " + rhmd.Timestamp)
            End If
            Console.WriteLine("Message: " + ex.Message)
            Console.WriteLine("StatusCode: " + ex.StatusCode.ToString)
            Console.WriteLine("ErrorCode: " + ex.ErrorCode)
            Console.WriteLine("ErrorType: " + ex.ErrorType)

            If Throttled Then
                Console.WriteLine("Waiting 10 seconds...")
                Threading.Thread.Sleep(10000)
            Else
                Throw ex
            End If
        End Sub

#End Region

    End Class

End Namespace
