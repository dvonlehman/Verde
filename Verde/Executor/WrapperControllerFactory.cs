using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;

namespace Verde.Executor
{
    /// <summary>
    /// Wrapper that allows us to hold a reference to the created controllers for subsequent use.
    /// </summary>
    internal class WrapperControllerFactory : IControllerFactory
    {
        private readonly IControllerFactory _factory;
        private readonly IList<Controller> _controllers;
        
        public WrapperControllerFactory(IControllerFactory factory)
        {
            _factory = factory;
            _controllers = new List<Controller>();
        }

        IController IControllerFactory.CreateController(RequestContext requestContext, string controllerName)
        {
            var controller = (Controller)_factory.CreateController(requestContext, controllerName);
            controller.ValueProvider = new WrapperValueProvider(()=>UnvalidatedValueProviderFactory.GetValueProvider(controller.ControllerContext));

            _controllers.Add(controller);
            return controller;
        }

        SessionStateBehavior IControllerFactory.GetControllerSessionBehavior(RequestContext requestContext, string controllerName)
        {
            return _factory.GetControllerSessionBehavior(requestContext, controllerName);
        }

        void IControllerFactory.ReleaseController(IController controller)
        {
            _factory.ReleaseController(controller);
        }

        /// <summary>
        /// Get the list of IControllers that have been created by this factory.
        /// </summary>
        public IList<Controller> CreatedControllers
        {
            get { return _controllers; }
        }
    }
}
