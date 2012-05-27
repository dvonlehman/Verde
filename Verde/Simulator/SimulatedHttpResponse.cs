using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using System.Web.Caching;
using System.Collections;
using System.Web.Routing;

namespace Verde.Simulator
{
    public class SimulatedHttpResponse : HttpResponseBase
    {
        private readonly RequestSimulatorSettings _settings;
        private TextWriter _output;
        private Stream _outputStream;
        private HttpCookieCollection _cookies;
        private NameValueCollection _headers;
        private HttpCachePolicyBase _cache;
        private HttpRequestBase _request;

        public SimulatedHttpResponse(RequestSimulatorSettings settings)
        {
            _settings = settings;
        }

        /// <summary>
        /// <see cref="HttpResponseBase.Cache"/>
        /// </summary>
        public override HttpCachePolicyBase Cache
        {
            get
            {
                return base.Cache;
                //TODO: Need to create a SimulatedHttpCache, no public constructor on HttpCachePolicy
                //if (_cache == null) _cache = new HttpCachePolicy(); return _cache;
            }
        }

        /// <summary>
        /// <see cref="HttpResponseBase.CacheControl"/>
        /// </summary>
        public override string CacheControl { get; set; }
        
        /// <summary>
        /// <see cref="HttpResponseBase.Charset"/>
        /// </summary>
        public override string Charset { get; set; }

        /// <summary>
        /// <see cref="HttpRequestBase.ContentEncoding"/>
        /// </summary>
        public override Encoding ContentEncoding { get; set; }

        /// <summary>
        /// <see cref="HttpRequestBase.ContentType"/>
        /// </summary>
        public override string ContentType { get; set; }

        /// <summary>
        /// <see cref="HttpReqeustBase.Cookies"/>
        /// </summary>
        public override HttpCookieCollection Cookies
        {
            get { if (_cookies == null) _cookies = new HttpCookieCollection(); return _cookies; }
        }

       
        /// <summary>
        /// <see cref="HttpRequestBase.Expires"/>
        /// </summary>
        public override int Expires { get; set; }
               
        /// <summary>
        /// <see cref="HttpRequestBase.ExpiresAbsolute"/>
        /// </summary>
        public override DateTime ExpiresAbsolute { get; set; }

        /// <summary>
        /// <see cref="HttpRequestBase.Filter"/>
        /// </summary>
        public override Stream Filter { get; set; }

       /// <summary>
       /// <see cref="HttpRequestBase.HeaderEncoding"/>
       /// </summary>
        public override Encoding HeaderEncoding { get; set; }

        /// <summary>
        /// <see cref="HttpRequestBase.Headers"/>
        /// </summary>
        public override NameValueCollection Headers
        {
            get
            {
                if (_headers == null) _headers = new NameValueCollection(); return _headers;
            }
        }

        /// <summary>
        /// <see cref="HttpRequestBase.IsClientConnected"/>
        /// </summary>
        public override bool IsClientConnected { get { return true; } }

        /// <summary>
        /// <see cref="HttpRequestBase.IsRequestBeingRedirected"/>
        /// </summary>
        public override bool IsRequestBeingRedirected { get { return base.IsRequestBeingRedirected; } }

        /// <summary>
        /// <see cref="HttpRequestBase.Output"/>
        /// </summary>
        public override TextWriter Output {
            get
            {
                if (_output == null) _output = new StreamWriter(this.OutputStream);
                return _output;
            }
            set { _output = value; }
        }

        /// <summary>
        /// <see cref="HttpRequestBase.OutputStream"/>
        /// </summary>
        public override Stream OutputStream { 
            get 
            {
                if (_outputStream == null)
                    _outputStream = new MemoryStream();

                return _outputStream; 
            } 
        }
        
        /// <summary>
        /// <see cref="HttpResponseBase.RedirectLocation"/>
        /// </summary>
        public override string RedirectLocation { get; set; }

        /// <summary>
        /// <see cref="HttpResponseBase.Status"/>
        /// </summary>
        public override string Status { get; set; }
        
        /// <summary>
        /// <see cref="HttpResponseBase.StatusCode"/>
        /// </summary>
        public override int StatusCode { get; set; }

        /// <summary>
        /// <see cref="HttResponseBase.StatusDescription"/>
        /// </summary>
        public override string StatusDescription { get; set; }

        /// <summary>
        /// <see cref="HttpResponseBase.HttpStatusCode"/>
        /// </summary>
        public override int SubStatusCode { get; set; }
        
        /// <summary>
        /// <see cref="HttpResponseBase.SuppressContent"/>
        /// </summary>
        public override bool SuppressContent { get; set; }

        /// <summary>
        /// <see cref="HttpResponseBase.TrySkipIisCustomErrors"/>
        /// </summary>
        public override bool TrySkipIisCustomErrors { get; set; }

        /// <summary>
        /// Doesn't do anything.
        /// </summary>
        public override void AddCacheDependency(params CacheDependency[] dependencies)
        {
        }

        /// <summary>
        /// Doesn't do anything.
        /// </summary>
        public override void AddCacheItemDependencies(ArrayList cacheKeys) { }

        /// <summary>
        /// Doesn't do anything.
        /// </summary>
        public override void AddCacheItemDependencies(string[] cacheKeys) { }

        /// <summary>
        /// Doesn't do anything.
        /// </summary>
        public override void AddCacheItemDependency(string cacheKey) { }

        /// <summary>
        /// Doesn't do anything.
        /// </summary>
        public override void AddFileDependencies(ArrayList filenames) { }

        /// <summary>
        /// Doesn't do anything.
        /// </summary>
        public override void AddFileDependencies(string[] filenames) { }

        /// <summary>
        /// Doesn't do anything.
        /// </summary>
        public override void AddFileDependency(string filename) { }

        public override void AddHeader(string name, string value)
        {
            this.Headers.Add(name, value);
        }
                
        public override void AppendCookie(HttpCookie cookie)
        {
            this.Cookies.Add(cookie);
        }

        public override void AppendHeader(string name, string value)
        {
            this.AddHeader(name, value);
        }

        /// <summary>
        /// Doesn't do anything.
        /// </summary>
        public override void AppendToLog(string param)
        {
        }

        /// <summary>
        /// Doesn't do anything.
        /// </summary>
        /// <returns>String.Empty</returns>
        public override string ApplyAppPathModifier(string overridePath)
        {
            return string.Empty;
        }
             
        public override void BinaryWrite(byte[] buffer)
        {
            base.BinaryWrite(buffer);
        }

        //
        // Summary:
        //     When overridden in a derived class, clears all headers and content output
        //     from the current response.
        //
        // Exceptions:
        //   System.NotImplementedException:
        //     Always.
        public override void Clear()
        {
            this.ClearHeaders();
            this.ClearContent();
        }
               
        public override void ClearContent()
        {
            this.OutputStream.Position = 0;
            this.Output = null;
        }

        public override void ClearHeaders()
        {
            this.Headers.Clear();
        }

        public override void Close()
        {
            base.Close();
        }

        /// <summary>
        /// Does nothing
        /// </summary>
        public override void DisableKernelCache()
        {
        }

        /// <summary>
        /// Flushes the output
        /// </summary>
        public override void End()
        {
            this.Flush();
        }

        /// <summary>
        /// Flush the output
        /// </summary>
        public override void Flush()
        {
            this.Output.Flush();
        }

        public override void Pics(string value)
        {
            this.AddHeader("PICS-Label", value);
        }

        public override void Redirect(string url)
        {
            this.StatusCode = 302;
            this.RedirectLocation = url;
        }

        public override void Redirect(string url, bool endResponse)
        {
            Redirect(url);
            if (endResponse)
                this.End();
        }


        public override void RedirectPermanent(string url)
        {
            this.StatusCode = 301;
            this.RedirectLocation = url;
        }


        public override void RedirectPermanent(string url, bool endResponse)
        {
            this.RedirectPermanent(url);
            if (endResponse)
                this.End();
        }
       
        public override void RedirectToRoute(object routeValues)
        {
            this.RedirectToRoute(new RouteValueDictionary(routeValues));
        }

        public override void RedirectToRoute(RouteValueDictionary routeValues)
        {
            this.RedirectToRoute(null, routeValues);
        }

        public override void RedirectToRoute(string routeName)
        {
            this.RedirectToRoute(routeName, null);
        }
       
        public override void RedirectToRoute(string routeName, object routeValues)
        {
            RedirectToRoute(routeName, new RouteValueDictionary(routeValues));
        }   
       
        public override void RedirectToRoute(string routeName, RouteValueDictionary routeValues)
        {
            this.RedirectToRoute(routeName, routeValues, false);
        }

        public override void RedirectToRoutePermanent(object routeValues)
        {
            this.RedirectToRoute(null, routeValues);
        }

        public override void RedirectToRoutePermanent(RouteValueDictionary routeValues)
        {
            this.RedirectToRoute(null, routeValues);
        }
      
        public override void RedirectToRoutePermanent(string routeName)
        {
            this.RedirectToRoutePermanent(routeName, null);
        }

        public override void RedirectToRoutePermanent(string routeName, object routeValues)
        {
            this.RedirectToRoutePermanent(routeName, new RouteValueDictionary(routeValues));
        }
       
        public override void RedirectToRoutePermanent(string routeName, RouteValueDictionary routeValues)
        {
            this.RedirectToRoute(routeName, routeValues, true);
        }

        private void RedirectToRoute(string routeName, RouteValueDictionary routeValues, bool permanant)
        {
            string virtualPath = null;
            VirtualPathData data = RouteTable.Routes.GetVirtualPath(_request.RequestContext, routeName, routeValues);
            if (data != null)
                virtualPath = data.VirtualPath;
            
            if (string.IsNullOrEmpty(virtualPath))
                throw new InvalidOperationException("No route found for redirect");
            
            if (permanant)
                this.RedirectPermanent(virtualPath);
            else
                this.Redirect(virtualPath);
        }

        /// <summary>
        /// Does nothing
        /// </summary>
        /// <param name="path"></param>
        public override void RemoveOutputCacheItem(string path)
        {
        }
        
        /// <summary>
        /// Does nothing
        /// </summary>
        /// <param name="path"></param>
        /// <param name="providerName"></param>
        public override void RemoveOutputCacheItem(string path, string providerName)
        {
        }

        public override void SetCookie(HttpCookie cookie)
        {
            this.Cookies.Add(cookie);
        }
                
        public override void TransmitFile(string filename)
        {
            this.TransmitFile(filename, 0L, -1L);
        }

        public override void TransmitFile(string filename, long offset, long length)
        {
            this.WriteFile(filename, offset, length);
        }
             
        public override void Write(char ch)
        {
            this.Output.Write(ch);
        }

        public override void Write(object obj)
        {
            this.Output.Write(obj);
        }
               
        public override void Write(string s)
        {
            this.Output.Write(s);
        }
                
        public override void Write(char[] buffer, int index, int count)
        {
            this.Output.Write(buffer, index, count);
        }

        public override void WriteFile(string filename)
        {
            this.WriteFile(filename, false);
        }
               
        public override void WriteFile(string filename, bool readIntoMemory)
        {
            if (filename == null)
               throw new ArgumentNullException("filename");
               
            long length;
            using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                length = fs.Length;
            }
            this.WriteFile(filename, 0, length);
        }

        public override void WriteFile(string filename, long offset, long size)
        {
            if (filename == null)
                throw new ArgumentNullException("filename");

            using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                if (size < 0L)
                    size = fs.Length - offset;

                if (size > 0L)
                {
                    if (offset > 0L)
                        fs.Seek(offset, SeekOrigin.Begin);


                    byte[] buffer = new byte[(int)size];
                    int count = fs.Read(buffer, 0, (int)size);
                    this.Output.Write(Encoding.Default.GetChars(buffer, 0, count));
                }
            }
        }
    }
}
