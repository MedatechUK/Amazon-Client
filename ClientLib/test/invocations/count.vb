
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
    ''' Get Feed Submission Count  Samples
    ''' </summary>
    Public Class GetFeedSubmissionCountSample


        ''' <summary>
        ''' returns the number of feeds matching all of the specified criteria
        ''' 
        ''' </summary>
        ''' <param name="service">Instance of MarketplaceWebService service</param>
        ''' <param name="request">GetFeedSubmissionCountRequest request</param>
        Public Shared Sub InvokeGetFeedSubmissionCount(ByVal service As MarketplaceWebService, ByVal request As GetFeedSubmissionCountRequest)
            Try
                Dim response As GetFeedSubmissionCountResponse = service.GetFeedSubmissionCount(request)


                Console.WriteLine("Service Response")
                Console.WriteLine("=============================================================================")
                Console.WriteLine()

                Console.WriteLine("        GetFeedSubmissionCountResponse")
                If response.IsSetGetFeedSubmissionCountResult() Then
                    Console.WriteLine("            GetFeedSubmissionCountResult")
                    Dim getFeedSubmissionCountResult As GetFeedSubmissionCountResult = response.GetFeedSubmissionCountResult
                    If getFeedSubmissionCountResult.IsSetCount() Then
                        Console.WriteLine("                Count")
                        Console.WriteLine("                    {0}", getFeedSubmissionCountResult.Count)
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
