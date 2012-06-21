using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;

namespace Verde.Executor
{
    /// <summary>
    /// Used to represent an HTTP form or querystring collection.
    /// </summary>
    public class HttpValueCollection : NameValueCollection
    {
        public HttpValueCollection(int capacity)
            : base(capacity, StringComparer.CurrentCultureIgnoreCase)
        {
        }

        public HttpValueCollection()
            : base(StringComparer.CurrentCultureIgnoreCase)
        {
        }
        
        public override string ToString()
        {
            return this.ToString(true);
        }

        public string ToString(bool urlEncode)
        {
           var sb = new StringBuilder();
            foreach (string key in this.Keys)
            {
                if (sb.Length > 0)
                    sb.Append('&');

                if (!string.IsNullOrEmpty(key))
                {
                    sb.Append(key);
                    sb.Append('=');    
                }
                sb.Append(urlEncode ? HttpUtility.UrlEncode(this[key].ToString()) : this[key].ToString());
            }
            return sb.ToString();
        }

        internal static HttpValueCollection Parse(string value)
        {
            if (String.IsNullOrEmpty(value)) return new HttpValueCollection();

            string[] pairs = value.Split('&');
            var coll = new HttpValueCollection(pairs.Length);
            for (var i = 0; i < pairs.Length; i++)
            {
                string[] keyValue = pairs[i].Split('=');
                if (keyValue.Length == 2)
                    coll[keyValue[0]] = HttpUtility.UrlDecode(keyValue[1]);
            }

            return coll;
        }
    }
}
