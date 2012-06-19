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
    public class HttpSessionStateProxy : HttpSessionStateWrapper
    {
        private string _sessionId;
        private SessionStateItemCollection _items;
        private bool _isNewSession;

        public HttpSessionStateProxy(HttpSessionState session)
            : base(session)
        {
        }

        internal void OverrideSessionState(ExecutorSettings settings)
        {
            _sessionId = !String.IsNullOrEmpty(settings.SessionId) ? settings.SessionId : NewSessionId();
            _items = settings.SessionStateItems != null ? settings.SessionStateItems : new SessionStateItemCollection();
            _isNewSession = settings.IsNewSession;
        }

        public override int Count
        {
            get { return ExecutorScope.Current != null ? _items.Count : base.Count; }
        }

        public override bool IsNewSession 
        { 
            get {  return ExecutorScope.Current != null ? _isNewSession : base.IsNewSession; } 
        }

        public override bool IsReadOnly { get { return false; } }

        public override bool IsSynchronized { get { return false; } }

        public override NameObjectCollectionBase.KeysCollection Keys 
        { 
            get { return ExecutorScope.Current != null ? _items.Keys : base.Keys; } 
        }

        public override string SessionID 
        { 
            get { return ExecutorScope.Current != null ?  _sessionId : base.SessionID; } 
        }


        public override object this[int index]
        {
            get { return ExecutorScope.Current != null ? _items[index] : base[index]; }
            set
            {
                if (ExecutorScope.Current != null)
                    _items[index] = value;
                else
                    base[index] = value;
            }
        }

        public override object this[string name]
        {
            get { return ExecutorScope.Current != null ? _items[name] : base[name]; }
            set 
            {
                if (ExecutorScope.Current != null)
                    _items[name] = value;
                else
                    base[name] = value;
            }
        }

        public override void Add(string name, object value)
        {
            if (ExecutorScope.Current != null)
                _items[name] = value;
            else
                base.Add(name, value);
        }

        public override IEnumerator GetEnumerator()
        {
            if (ExecutorScope.Current != null)
                return _items.GetEnumerator();
            else
                return base.GetEnumerator();
        }

        public override void Remove(string name)
        {
            if (ExecutorScope.Current != null)
                _items.Remove(name);
            else
                base.Remove(name);
        }

        public override void RemoveAll()
        {
            if (ExecutorScope.Current != null)
                _items.Clear();
            else
                base.RemoveAll();
        }

        public override void RemoveAt(int index)
        {
            if (ExecutorScope.Current != null)
                _items.RemoveAt(index);
            else
                base.RemoveAt(index);
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
