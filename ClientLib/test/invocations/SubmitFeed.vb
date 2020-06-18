
'****************************************************************************** 
' *  Copyright 2009 Amazon Services.
' *  Licensed under the Apache License, Version 2.0 (the "License"); 
' *  
' *  You may not use this file except in compliance with the License. 
' *  You may obtain a copy of the License at: http://aws.amazon.com/apache2.0
' *  This file is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
' *  CONDITIONS OF ANY KIND, either express or implied. See the License for the 
' *  specific language governing permissions and limitations under the License.
' * ***************************************************************************** 
' * 
' *  Marketplace Web Service CSharp Library
' *  API Version: 2009-01-01
' *  Generated: Mon Mar 16 17:31:42 PDT 2009 
' * 
' 


Imports System.Xml.Serialization
Imports System.Collections.Generic
Imports MarketplaceWebService
Imports MarketplaceWebService.Model



Namespace MarketplaceWebService.Samples

    ''' <summary>
    ''' Submit Feed  Samples
    ''' </summary>
    Public Class SubmitFeedSample


        ''' <summary>
        ''' Uploads a file for processing together with the necessary
        ''' metadata to process the file, such as which type of feed it is.
        ''' PurgeAndReplace if true means that your existing e.g. inventory is
        ''' wiped out and replace with the contents of this feed - use with
        ''' caution (the default is false).
        ''' 
        ''' </summary>
        ''' <param name="service">Instance of MarketplaceWebService service</param>
        ''' <param name="request">SubmitFeedRequest request</param>
        Public Shared Sub InvokeSubmitFeed(ByVal service As MarketplaceWebService, ByVal request As SubmitFeedRequest)
            Try
                Dim response As SubmitFeedResponse = service.SubmitFeed(request)


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
                    Dim responseMetadata As ResponseMetadata = response.ResponseMetadata
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
            Catch ex As MarketplaceWebServiceException
                Console.WriteLine("Caught Exception: " + ex.Message)
                Console.WriteLine("Response Status Code: " + ex.StatusCode)
                Console.WriteLine("Error Code: " + ex.ErrorCode)
                Console.WriteLine("Error Type: " + ex.ErrorType)
                Console.WriteLine("Request ID: " + ex.RequestId)
                Console.WriteLine("XML: " + ex.XML)
                Console.WriteLine("ResponseHeaderMetadata: " + ex.ResponseHeaderMetadata)
            End Try
        End Sub
    End Class
End Namespace

'=======================================================
'Service provided by Telerik (www.telerik.com)
'Conversion powered by NRefactory.
'Twitter: @telerik
'Facebook: facebook.com/telerik
'=======================================================
