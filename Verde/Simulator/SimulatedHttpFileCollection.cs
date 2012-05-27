using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Collections.Specialized;

namespace Verde.Simulator
{
    public class SimulatedHttpFileCollection : HttpFileCollectionBase
    {
        private readonly NameValueCollection _files;

        internal SimulatedHttpFileCollection()
        {
            _files = new NameValueCollection();
        }

        public SimulatedHttpFileCollection(NameValueCollection files) {
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
