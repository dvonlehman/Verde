using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Globalization;

namespace Verde.Executor
{
    /// <summary>
    /// Wrapper that defers executing a factory function until the underlying IValueProvider is actually needed.
    /// </summary>
    internal class WrapperValueProvider : IValueProvider
    {
        private readonly Lazy<IValueProvider> _realProvider;

        public WrapperValueProvider(Func<IValueProvider> func)
        {
            _realProvider = new Lazy<IValueProvider>(func);
        }

        bool IValueProvider.ContainsPrefix(string prefix)
        {
            return _realProvider.Value.ContainsPrefix(prefix);
        }

        ValueProviderResult IValueProvider.GetValue(string key)
        {
            return _realProvider.Value.GetValue(key);
        }
    }

    /// <summary>
    /// Collection of ValueProviders which use unvalidated providers that are decoupled from HttpContext.Current.
    /// </summary>
    internal class UnvalidatedValueProviderFactory
    {
         private static readonly ValueProviderFactoryCollection _factories = new ValueProviderFactoryCollection() {
            new ChildActionValueProviderFactory(),
            new DelegateValueProviderFactory<UnvalidatedFormValueProvider>(c=>new UnvalidatedFormValueProvider(c)),
            new JsonValueProviderFactory(),
            new RouteDataValueProviderFactory(),
            new DelegateValueProviderFactory<UnvalidatedQueryStringValueProvider>(c=>new UnvalidatedQueryStringValueProvider(c)),
            new HttpFileCollectionValueProviderFactory()
        };

        public static IValueProvider GetValueProvider(ControllerContext controllerContext)
        {
            return _factories.GetValueProvider(controllerContext);
        }
        
        private class DelegateValueProviderFactory<T> : ValueProviderFactory where T:IValueProvider
        {
            private Func<ControllerContext, T> _func;

            public DelegateValueProviderFactory(Func<ControllerContext, T> func)
            {
                _func = func;
            }

            public override IValueProvider GetValueProvider(ControllerContext controllerContext)
            {
                return _func(controllerContext);
            }
        }

        private class UnvalidatedQueryStringValueProvider : NameValueCollectionValueProvider
        {
            public UnvalidatedQueryStringValueProvider(ControllerContext context) : 
                base(context.HttpContext.Request.QueryString, CultureInfo.InvariantCulture) { }
        }

        private class UnvalidatedFormValueProvider : NameValueCollectionValueProvider
        {
            public UnvalidatedFormValueProvider(ControllerContext context) :
                base(context.HttpContext.Request.Form, CultureInfo.InvariantCulture) { }
        }
    }
}
