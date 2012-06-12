using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;

namespace Verde.Executor
{
    /// <summary>
    /// Used to simulate the session state in a simulated MVC http request.
    /// </summary>
    public class ExecutorHttpSessionState : HttpSessionStateBase
    {
        private readonly ExecutorSettings _settings;

        public ExecutorHttpSessionState(ExecutorSettings settings)
        {
            _settings = settings;
            if (String.IsNullOrEmpty(settings.SessionId))
                settings.SessionId = NewSessionId();

            if (settings.SessionStateItems == null)
                settings.SessionStateItems = new SessionStateItemCollection();
        }

        public override int CodePage { get; set; }
        
        public override HttpCookieMode CookieMode { 
            get { return HttpCookieMode.UseCookies; } 
        }

        public override int Count 
        {
            get { return _settings.SessionStateItems.Count; }
        }

        public override bool IsCookieless { get { return false; } }

        public override bool IsNewSession { get { return _settings.IsNewSession; } }

        public override bool IsReadOnly { get { return false; } }

        public override bool IsSynchronized { get { return false; } }

        public override NameObjectCollectionBase.KeysCollection Keys { get { return _settings.SessionStateItems.Keys; } }
              
        public override int LCID { get; set; }

        public override SessionStateMode Mode { get { return SessionStateMode.InProc; } }

        public override string SessionID { get { return _settings.SessionId; } }
        
     
        public override object this[int index] 
        {
            get { return _settings.SessionStateItems[index]; }
            set { _settings.SessionStateItems[index] = value; } 
        }
        
        public override object this[string name] 
        {
            get { return _settings.SessionStateItems[name]; }
            set { _settings.SessionStateItems[name] = value; }
        }

        public override void Abandon()
        {
            this.Clear();
        }

        public override void Add(string name, object value)
        {
            _settings.SessionStateItems[name] = value;
        }
       
        public override void Clear()
        {
            _settings.SessionStateItems.Clear();
        }
       
        public override IEnumerator GetEnumerator()
        {
            return _settings.SessionStateItems.GetEnumerator();
        }
      
        public override void Remove(string name)
        {
            _settings.SessionStateItems.Remove(name);
        }

        public override void RemoveAll()
        {
            _settings.SessionStateItems.Clear();
        }

        public override void RemoveAt(int index)
        {
            _settings.SessionStateItems.RemoveAt(index);
        }

        /// <summary>
        /// Generate a new session ID.
        /// </summary>
        /// <returns></returns>
        public static string NewSessionId()
        {
            var randgen = new System.Security.Cryptography.RNGCryptoServiceProvider();
            byte[] data = new byte[15];
            randgen.GetBytes(data);
            return Convert.ToBase64String(data);
        }
    }
}
