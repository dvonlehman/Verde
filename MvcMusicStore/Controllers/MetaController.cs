using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcMusicStore.Controllers
{
    public class MetaController : Controller
    {
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            // Simply marking that this method fired
            filterContext.HttpContext.Items["OnActionExecuted"] = true;
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Simply marking that this method fired
            filterContext.HttpContext.Items["OnActionExecuting"] = true;
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);
        }

        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            // Simply marking that this method fired
            filterContext.HttpContext.Items["OnResultExecuting"] = true;
        }

        protected override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            // Simply marking that this method fired
            filterContext.HttpContext.Items["OnResultExecuted"] = true;
        }

        //
        // GET: /Meta/
        public ActionResult Index()
        {
            return View();
        }
    }
}
