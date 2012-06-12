using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcMusicStore.Models;

namespace MvcMusicStore.Controllers
{
    public class StoreController : Controller
    {
        MusicStoreEntities storeDB = new MusicStoreEntities();

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
        // GET: /Store/

        public ActionResult Index()
        {
            var genres = storeDB.Genres.ToList();

            return View(genres);
        }

        //
        // GET: /Store/Browse?genre=Disco

        public ActionResult Browse(string genre)
        {
            // Retrieve Genre and its Associated Albums from database
            var genreModel = storeDB.Genres.Include("Albums")
                .Single(g => g.Name == genre);

            return View(genreModel);
        }

        //
        // GET: /Store/Details/5

        public ActionResult Details(int id)
        {
            var album = storeDB.Albums.Find(id);

            return View(album);
        }

        //
        // GET: /Store/GenreMenu

        [ChildActionOnly]
        public ActionResult GenreMenu()
        {
            var genres = storeDB.Genres.ToList();

            return PartialView(genres);
        }

    }
}