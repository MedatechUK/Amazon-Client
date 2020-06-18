/******************************************************************************* 
 *  Copyright 2009 Amazon Services.
 *  Licensed under the Apache License, Version 2.0 (the "License"); 
 *  
 *  You may not use this file except in compliance with the License. 
 *  You may obtain a copy of the License at: http://aws.amazon.com/apache2.0
 *  This file is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
 *  CONDITIONS OF ANY KIND, either express or implied. See the License for the 
 *  specific language governing permissions and limitations under the License.
 * ***************************************************************************** 
 * 
 *  Marketplace Web Service CSharp Library
 *  API Version: 2009-01-01
 *  Generated: Mon Mar 16 17:31:42 PDT 2009 
 * 
 */

using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using MarketplaceWebService;
using MarketplaceWebService.Mock;
using MarketplaceWebService.Model;
using System.IO;

namespace MarketplaceWebService.Samples
{

    /// <summary>
    /// Marketplace Web Service  Samples
    /// </summary>
    public class MarketplaceWebServiceSamples 
    {
    
       /**
        * Samples for Marketplace Web Service functionality
        */
        public static void Main(string [] args)
        {

            String accessKeyId = "<Your AWS Access Key>";
            String secretAccessKey = "<Your AWS Secret Key>";

            /************************************************************************
            * Instantiate  Implementation of Marketplace Web Service 
            ***********************************************************************/

            MarketplaceWebServiceConfig config = new MarketplaceWebServiceConfig();

            /************************************************************************
             * The application name and version are included in each MWS call's
             * HTTP User-Agent field. These are required fields.
             ***********************************************************************/

            const string applicationName = "<Your Application Name>";
            const string applicationVersion = "<Your Application Version>";

            //MarketplaceWebService service =
            //    new MarketplaceWebServiceClient(
            //        accessKeyId,
            //        secretAccessKey,
            //        applicationName,
            //        applicationVersion,
            //        config);


            /************************************************************************
             * All MWS requests must contain the seller's merchant ID and 
             * marketplace ID.
             ***********************************************************************/
            const string merchantId = "<Your Merchant ID>";
            const string marketplaceId = "<Your Marketplace ID>";
                       
            /************************************************************************
             * Uncomment to configure the client instance. Configuration settings
             * include:
             *
             *  - MWS Service endpoint URL
             *  - Proxy Host and Proxy Port
             *  - User Agent String to be sent to Marketplace Web Service  service
             *
             ***********************************************************************/
            MarketplaceWebServiceConfig config = new MarketplaceWebServiceConfig();

            //config.ProxyHost = "https://PROXY_URL";
            //config.ProxyPort = 9090;

            config.ServiceURL = "https://mws.amazonservices.co.uk";

            //config.SetUserAgentHeader(
            //    applicationName,
            //    applicationVersion,
            //    "C#",
            //    "<Parameter 1>", "<Parameter 2>");
            //MarketplaceWebService service = new MarketplaceWebServiceClient(accessKeyId, secretAccessKey, config);



            /************************************************************************
             * Uncomment to invoke Submit Feed Action
             ***********************************************************************/
            {
                // SubmitFeedRequest request = new SubmitFeedRequest();
                // request.Merchant = merchantId;
                // request.MWSAuthToken = "<Your MWS Auth Token>"; // Optional
                // request.MarketplaceIdList = new IdList();
                // request.MarketplaceIdList.Id = new List<string>( new string [] { marketplaceId } );

                // MWS exclusively offers a streaming interface for uploading your feeds. This is because 
                // feed sizes can grow to the 1GB+ range - and as your business grows you could otherwise 
                // silently reach the feed size where your in-memory solution will no longer work, leaving you 
                // puzzled as to why a solution that worked for a long time suddenly stopped working though 
                // you made no changes. For the same reason, we strongly encourage you to generate your feeds to 
                // local disk then upload them directly from disk to MWS.

                //request.FeedContent = File.Open("feed.xml", FileMode.Open, FileAccess.Read);

                // Calculating the MD5 hash value exhausts the stream, and therefore we must either reset the
                // position, or create another stream for the calculation.
                //request.ContentMD5 = MarketplaceWebServiceClient.CalculateContentMD5(request.FeedContent);
                //request.FeedContent.Position = 0;

                //request.FeedType = "FEED_TYPE";

                //SubmitFeedSample.InvokeSubmitFeed(service, request);
            }


            /************************************************************************
             * Uncomment to invoke Get Feed Submission List By Next Token Action
             ***********************************************************************/
            {
                //GetFeedSubmissionListByNextTokenRequest request = new GetFeedSubmissionListByNextTokenRequest();
                //request.Merchant = merchantId;
                //request.MWSAuthToken = "<Your MWS Auth Token>"; // Optional
                //request.NextToken = "NextToken from GetFeedSubmissionList";
                // @TODO: set additional request parameters here
                //GetFeedSubmissionListByNextTokenSample.InvokeGetFeedSubmissionListByNextToken(service, request);
            }           

            /************************************************************************
             * Uncomment to invoke Get Feed Submission Count Action
             ***********************************************************************/
            {
                //GetFeedSubmissionCountRequest request = new GetFeedSubmissionCountRequest();
                //request.Merchant = merchantId;
                //request.MWSAuthToken = "<Your MWS Auth Token>"; // Optional
                // @TODO: set additional request parameters here
                //GetFeedSubmissionCountSample.InvokeGetFeedSubmissionCount(service, request);
            }
           
            /************************************************************************
             * Uncomment to invoke Get Feed Submission Result Action
             ***********************************************************************/
            {
                //GetFeedSubmissionResultRequest request = new GetFeedSubmissionResultRequest();
                //request.Merchant = merchantId;
                //request.MWSAuthToken = "<Your MWS Auth Token>"; // Optional

                // Note that depending on the size of the feed sent in, and the number of errors and warnings,
                // the result can reach sizes greater than 1GB. For this reason we recommend that you _always_ 
                // program to MWS in a streaming fashion. Otherwise, as your business grows you may silently reach
                // the in-memory size limit and have to re-work your solution.
                // NOTE: Due to Content-MD5 validation, the stream must be read/write.
                //request.FeedSubmissionId = "FEED_SUBMISSION_ID";
                //request.FeedSubmissionResult = File.Open("feedSubmissionResult.xml", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                
                //GetFeedSubmissionResultSample.InvokeGetFeedSubmissionResult(service, request);
            }

            /************************************************************************
             * Uncomment to invoke Get Feed Submission List Action
             ***********************************************************************/
            {
                //GetFeedSubmissionListRequest request = new GetFeedSubmissionListRequest();
                //request.Merchant = merchantId;
                //request.MWSAuthToken = "<Your MWS Auth Token>"; // Optional
                // @TODO: set additional request parameters here
                //GetFeedSubmissionListSample.InvokeGetFeedSubmissionList(service, request);
            }
          

            Console.WriteLine();
            Console.WriteLine("===========================================");
            Console.WriteLine("End of output. You can close this window");
            Console.WriteLine("===========================================");

            System.Threading.Thread.Sleep(50000);
        }

    }


}
