﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace GOG
{
    public class NetworkController :
        IStringDataRequestController,
        IFileRequestController,
        IStringNetworkController
    {
        public const string postMethod = "POST";
        public const string getMethod = "GET";

        // TODO: nothing is using accept type - confirm it's not needed and remove
        public const string acceptHtml = "text/html, application/xhtml+xml, */*";
        public const string acceptJson = "application/json, text/plain, */*";

        private static HttpWebRequest request;
        private static CookieContainer sharedCookies;

        private ISerializationController serializationController;
        private IUriController uriController;

        public NetworkController(IUriController uriController)
        {
            request = null;
            sharedCookies = new CookieContainer();
            this.uriController = uriController;
        }

        public NetworkController(IUriController uriController, ISerializationController serializationController) :
            this(uriController)
        {
            this.serializationController = serializationController;
        }

        private async Task<WebResponse> RequestResponse(
            string uri,
            string method = getMethod,
            string data = null,
            string accept = acceptJson)
        {
            Uri requestUri = new Uri(uri);

            request = (HttpWebRequest)HttpWebRequest.Create(requestUri);
            request.CookieContainer = sharedCookies;

            ServicePointManager.Expect100Continue = false;
            ServicePointManager.SetTcpKeepAlive(true, 10000, 10000);

            // using IE11 default UA string
            request.UserAgent =
                "Mozilla/5.0 (Windows NT 6.3; WOW64; Trident/7.0; rv:11.0) like Gecko";
            request.Method = method;
            request.Accept = accept;

            if (!string.IsNullOrEmpty(data) &&
                method == postMethod)
            {
                var postData = Encoding.ASCII.GetBytes(data);

                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = postData.Length;

                using (var stream = request.GetRequestStream())
                    stream.Write(postData, 0, postData.Length);
            }

            WebResponse response = null;

            try
            {
                response = await Task.Factory.FromAsync<WebResponse>(
                    request.BeginGetResponse,
                    request.EndGetResponse,
                    null);
            }
            catch (WebException)
            {
                return null;
            }

            return response;
        }

        public async Task RequestFile(
            string fromUri,
            string toFile,
            IStreamWritableController streamWriteableController)
        {
            WebResponse response = await RequestResponse(fromUri);
            int bufferSize = 64 * 1024; // 64K
            byte[] buffer = new byte[bufferSize];
            int bytesRead = 0;

            if (response == null) return;

            using (Stream writeableStream = streamWriteableController.OpenWritable(toFile))
            using (Stream responseStream = response.GetResponseStream())
                while ((bytesRead = await responseStream.ReadAsync(buffer, 0, bufferSize)) > 0)
                    await writeableStream.WriteAsync(buffer, 0, bytesRead);
        }

        public async Task<string> RequestString(
            string baseUri,
            IDictionary<string, string> parameters = null)
        {
            string uri = uriController.CombineUri(baseUri, parameters);

            WebResponse response = await RequestResponse(uri, getMethod, null, acceptJson);

            if (response == null) return null;

            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                return await reader.ReadToEndAsync();
        }

        public async Task<string> PostString(
            string baseUri,
            IDictionary<string, string> parameters = null,
            string data = null)
        {
            string uri = uriController.CombineUri(baseUri, parameters);

            WebResponse response = await RequestResponse(uri, postMethod, data, acceptJson);

            if (response == null) return null;

            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                return await reader.ReadToEndAsync();
        }

        public async Task<T> RequestData<T>(
            string baseUri,
            Dictionary<string, string> parameters = null)
        {
            if (serializationController == null)
            {
                throw new InvalidOperationException("You need to instantiate NetworkController instance with ISerializationController to use RequestData");
            }

            var dataString = await RequestString(baseUri, parameters);
            return serializationController.Parse<T>(dataString);
        }
    }
}
