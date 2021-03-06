﻿using System.Text;

namespace JumpKick.HttpLib
{
    using JumpKick.HttpLib.Provider;
    using System;
    using System.IO;
    using System.Net;
    using JumpKick.HttpLib.Streams;

    public class Request
    {
        protected string url;
        protected HttpVerb method = HttpVerb.Get;
        protected HeaderProvider headers;
        protected AuthenticationProvider auth;
        protected BodyProvider body;
        //注释掉用户收集的代码
        //private static BaseCollector use = new Usage();

        protected ActionProvider action;

        public Request()
        {
        }

        public String Url
        {
            set
            {
                this.url = value;
            }
            get
            {
                return this.url;
            }
        }

        public HttpVerb Method
        {
            set
            {
                this.method = value;
            }

            get
            {
                return this.method;
            }
        }

        public HeaderProvider Headers
        {
            set
            {
                this.headers = value;
            }
            get
            {
                return this.headers;
            }
        }

        public AuthenticationProvider Auth
        {
            set
            {
                this.auth = value;
            }
            get
            {
                return this.auth;
            }
        }

        public ActionProvider Action
        {
            set
            {
                this.action = value;
            }
            get
            {
                return this.action;
            }
        }

        public BodyProvider Body
        {
            set
            {
                this.body = value;
            }
            get
            {
                return this.body;
            }
        }

        public void Go()
        {
            MakeRequest();
        }


        protected virtual HttpWebRequest GetWebRequest(string url)
        {
            return (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
        }

        /// <summary>
        /// 发送请求方法
        /// </summary>
        protected void MakeRequest()
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentNullException("url is empty");
            }
            try
            {
                HttpWebRequest request = this.GetWebRequest(url);
                request.CookieContainer = Cookies.Container;
                request.Method = method.ToString().ToUpper();
                request.AllowAutoRedirect = false;
                if (method == HttpVerb.Get || method == HttpVerb.Head) 
                {
                    ExecuteRequestWithoutBody(request);
                } 
                else 
                {
                    request.ContentType = body.GetContentType();
                    ExecuteRequestWithBody(request);
                }
            }
            catch (WebException webEx)
            {
                action.Fail(webEx);
            }
        }

        protected virtual void ExecuteRequestWithoutBody(HttpWebRequest request)
        {
            //if (use != null && request.RequestUri != null) use.Collect(new Collection { slug = request.RequestUri.DnsSafeHost, method = request.Method });
            request.BeginGetResponse(ProcessCallback(action.Success, action.Fail), request);
        }

        protected virtual void ExecuteRequestWithBody(HttpWebRequest request)
        {
            //if(use!=null && request.RequestUri!=null) use.Collect(new Collection { slug = request.RequestUri.DnsSafeHost, method = request.Method });
            request.BeginGetRequestStream(new AsyncCallback((IAsyncResult callbackResult) =>
            {
                HttpWebRequest tmprequest = (HttpWebRequest)callbackResult.AsyncState;
                ProgressCallbackHelper copy = body.GetBody().CopyToProgress(tmprequest.EndGetRequestStream(callbackResult),null);

                copy.ProgressChanged += (bytesSent, totalBytes) => { body.OnProgressChange(bytesSent, totalBytes); };
                copy.Completed += (totalBytes) => { body.OnCompleted(totalBytes); };
                copy.Go();
                // Start the asynchronous operation to get the response
                tmprequest.BeginGetResponse(ProcessCallback(action.Success, action.Fail), tmprequest);

            }), request);
        }


        protected AsyncCallback ProcessCallback(Action<WebHeaderCollection, Stream, HttpWebResponse> success, Action<WebException> fail)
        {
            return new AsyncCallback((callbackResult) =>
            {
                HttpWebRequest webRequest = (HttpWebRequest)callbackResult.AsyncState;

                try
                {
                    using (HttpWebResponse response = (HttpWebResponse)webRequest.EndGetResponse(callbackResult))
                    {
                       // if (response.ContentLength > 0) { response.Headers.Add("Content-Length", response.ContentLength.ToString()); }
                        if (success != null)
                        {
                            success(response.Headers, response.GetResponseStream(),response);
                        }
                    }
                }
                catch (WebException webEx)
                {
                    fail(webEx);
                }
            });
        }


       
    }

   
}
