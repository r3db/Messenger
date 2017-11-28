using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using Contracts;

namespace Messenger.WebServices
{
    public abstract class AbstractSoapRequest
    {
        // Done!
        #region Internal Constants

        private const string Debug = "DEBUG";
        private static readonly string OutputMessage = new string('>', 32);
        private static readonly string SeparatorMessage = new string('-', 32);

        #endregion

        // Done!
        #region Internal Data

        private readonly Uri uri;
        private readonly string action;

        #endregion

        // Done!
        #region .Ctor

        private AbstractSoapRequest(Uri uri, string action)
        {
            uri.NotNull();
            uri.AbsolutePath.NotEmpty();

            this.uri = uri;
            this.action = action;
        }

        protected AbstractSoapRequest(string url, string action)
            : this(new Uri(url, UriKind.Absolute), action)
        {
        }

        #endregion

        // Done!
        #region Debug

        [Conditional(Debug)]
        private static void PrintRequestMessage(byte[] request)
        {
            lock (Console.Out)
            {
                ConsoleColor backup = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(OutputMessage);
                Console.WriteLine(Encoding.UTF8.GetString(request).ToXmlFormat());
                Console.WriteLine(SeparatorMessage);
                Console.ForegroundColor = backup;
            }
        }

        [Conditional(Debug)]
        protected static void PrintResponseMessage(string response)
        {
            lock (Console.Out)
            {
                ConsoleColor backup = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(OutputMessage);
                Console.WriteLine(response.ToXmlFormat());
                Console.WriteLine(SeparatorMessage);
                Console.ForegroundColor = backup;
            }
        }

        #endregion

        // Done!
        #region Methods

        // Done!
        protected HttpWebRequest CreateWebRequest()
        {
            byte[] content = this.GetRequestContent();

            HttpWebRequest request = CreateSoapRequest(content, this.uri, this.action);

            using(Stream stream =request.GetRequestStream())
            {
                PrintRequestMessage(content);
                stream.Write(content, 0, content.Length);
                stream.Flush();
            }

            return request;

        }

        // Done!
        private static HttpWebRequest CreateSoapRequest(ICollection<byte> content, Uri uri, string action)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);

            request.ContentLength = content.Count;
            request.Method = "POST";
            request.Accept = "text/*";
            request.ContentType = "text/xml; charset=utf-8";
            request.ProtocolVersion = HttpVersion.Version11;
            request.Timeout = 3000;

            if (string.IsNullOrEmpty(action) == false)
            {
                request.Headers.Add("SOAPAction", action);
            }

            return request;
        }

        // Done!
        protected Stream GetResponseStream()
        {
            int attempts = 5;

            while (attempts > 0)
            {
                try
                {
                    HttpWebRequest request = this.CreateWebRequest();
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    return response.GetResponseStream();
                }
                catch (Exception ex)
                {
                    attempts--;
                    Console.WriteLine("Could not Connect: " + ex.Message);
                    Console.WriteLine("Trying again");
                }
            }

            return null;

        }

        // Done!
        protected abstract byte[] GetRequestContent();

        #endregion

    }
}
