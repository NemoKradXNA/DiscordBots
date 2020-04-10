using System;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;

namespace TASBot.Utilities
{
    public delegate void HTTPCallBack(string response);

    internal class RequestState
    {
        public static int BufferSize = 1024;
        public StringBuilder RequestData;
        public byte[] BufferRead;
        public WebRequest Request;
        public Stream ResponseStream;
        public HTTPCallBack CallBack;

        // Create Decoder for appropriate encoding type.  
        public Decoder StreamDecode = Encoding.UTF8.GetDecoder();

        public RequestState()
        {
            BufferRead = new byte[BufferSize];
            RequestData = new StringBuilder(string.Empty);
            Request = null;
            ResponseStream = null;
            CallBack = null;
        }
    }

    /// <summary>
    /// This class enables the caller to make asynchronous HTTP requests.
    /// </summary>
    public class HTTPAsyncRequest
    {
        /// <summary>
        /// This is the HttpWebRequest use to make all the calls.
        /// </summary>
        protected static HttpWebRequest request;

        public HTTPAsyncRequest() { }

        /// <summary>
        /// Method to make an asynchronous HTTP request. The asynchronous call is placed in it's own thread.
        /// </summary>
        /// <param name="url">URL to call</param>
        /// <param name="credentials">Credentials if required</param>
        /// <param name="callBack">Call back on result, if required.</param>
        public void AsyncRequest(string url, NetworkCredential credentials = null, HTTPCallBack callBack = null, int timeout = 5000)
        {
            new Thread(new ParameterizedThreadStart(DoAsyncCall)).Start(new object[] { url, credentials, timeout, callBack });
        }

        /// <summary>
        /// Should the user not want to make an asynchronous call..
        /// </summary>
        /// <param name="url">URL to call</param>
        /// <param name="credentials">Credentials if required</param>
        /// <returns>The response as a string from the request.</returns>
        public string Request(string url, NetworkCredential credentials = null, int timeout = 5000)
        {
            string response = null;

            CreateHttpWebRequest(url, credentials);

            request.Timeout = timeout;

            using (HttpWebResponse httpResponse = (HttpWebResponse)request.GetResponse())
            {
                // Get the response.. 
                using (StreamReader sr = new StreamReader(httpResponse.GetResponseStream(), Encoding.UTF8))
                {
                    response = sr.ReadToEnd();
                }
            }

            return response;
        }

        /// <summary>
        /// Method to create the request object.
        /// </summary>
        /// <param name="url">URL to call</param>
        /// <param name="credentials">Credentials if required</param>
        protected virtual void CreateHttpWebRequest(string url, NetworkCredential credentials = null)
        {
            request = (HttpWebRequest)WebRequest.Create(url);
            request.Credentials = credentials;
        }

        /// <summary>
        /// This is the code ran in the thread for a given call.
        /// </summary>
        /// <param name="param">Parameters passed, should be an object[3] array of url, credentials and callback</param>
        protected virtual void DoAsyncCall(object param)
        {
            string url;
            NetworkCredential credentials;
            HTTPCallBack callBack;
            int timeout = 1000;

            object[] paramData = (object[])param;

            url = paramData[0].ToString();
            credentials = paramData[1] as NetworkCredential;
            timeout = (int)paramData[2];
            callBack = paramData[3] as HTTPCallBack;

            CreateHttpWebRequest(url, credentials);

            request.Timeout = timeout;

            RequestState rs = new RequestState();
            rs.Request = request;
            rs.CallBack = callBack;

            request.BeginGetResponse(AsyncResponse, rs);
        }

        /// <summary>
        /// This is the asynchronous call back from the HttpWebRequest BeginGetResponse
        /// </summary>
        /// <param name="result">The returned IAsyncResult</param>
        protected void AsyncResponse(IAsyncResult result)
        {
            RequestState rs = (RequestState)result.AsyncState;
            HttpWebResponse httpResponse = (HttpWebResponse)rs.Request.EndGetResponse(result);
            Stream ResponseStream = httpResponse.GetResponseStream();

            rs.ResponseStream = ResponseStream;

            ResponseStream.BeginRead(rs.BufferRead, 0, RequestState.BufferSize, AsyncResponseRead, rs);
        }

        /// <summary>
        /// This is the asynchronous call back from the Stream BeginRead
        /// </summary>
        /// <param name="result">The returned IAsyncResult</param>
        protected void AsyncResponseRead(IAsyncResult result)
        {
            RequestState rs = (RequestState)result.AsyncState;

            Stream responseStream = rs.ResponseStream;

            // Read rs.BufferRead to verify that it contains data.   
            int read = responseStream.EndRead(result);
            if (read > 0)
            {
                // Prepare a Char array buffer for converting to Unicode.  
                char[] charBuffer = new char[1024];

                // Convert byte stream to Char array and then to String.  
                // length contains the number of characters converted to Unicode.  
                int len = rs.StreamDecode.GetChars(rs.BufferRead, 0, read, charBuffer, 0);

                string str = new string(charBuffer, 0, len);

                // Append the recently read data to the RequestData string builder  
                // object contained in RequestState.  
                rs.RequestData.Append(Encoding.ASCII.GetString(rs.BufferRead, 0, read));

                // Continue reading data until   
                // responseStream.EndRead returns –1.  
                responseStream.BeginRead(rs.BufferRead, 0, 1024, new AsyncCallback(AsyncResponseRead), rs);
            }
            else
            {
                string response = null;
                if (rs.RequestData.Length > 0)
                {
                    // Get the response.
                    response = rs.RequestData.ToString();
                }

                if (rs.CallBack != null)
                    rs.CallBack(response);

                // Close down the response stream.  
                responseStream.Close();
            }
        }
    }
}
