using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Collections.Specialized;

namespace Verde.Executor
{
    public class ExecutorHttpFileCollection : HttpFileCollectionBase
    {
        private readonly NameValueCollection _files;

        internal ExecutorHttpFileCollection()
        {
            _files = new NameValueCollection();
        }

        public ExecutorHttpFileCollection(NameValueCollection files) {
            _files = files;
        }

        public override string[] AllKeys
        {
            get { return _files.AllKeys; }
        }

        public override int Count
        {
            get { return _files.Count; }
        }

    }
}
