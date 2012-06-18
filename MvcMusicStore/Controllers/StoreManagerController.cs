using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcMusicStore.Models;

namespace MvcMusicStore.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class StoreManagerController : Controller
    {
        private readonly IMusicStoreEntities _entities;

        public StoreManagerController(IMusicStoreEntities entities)
        {
            _entities = entities;
        }

        //
        // GET: /StoreManager/

        public ViewResult Index()
        {
            var albums = _entities.Albums.Include(a => a.Genre).Include(a => a.Artist);
            return View(albums.ToList());
        }

        //
        // GET: /StoreManager/Details/5

        public ViewResult Details(int id)
        {
            Album album = _entities.Albums.Find(id);
            return View(album);
        }

        //
        // GET: /StoreManager/Create

        public ActionResult Create()
        {
            ViewBag.GenreId = new SelectList(_entities.Genres, "GenreId", "Name");
            ViewBag.ArtistId = new SelectList(_entities.Artists, "ArtistId", "Name");
            return View();
        } 

        //
        // POST: /StoreManager/Create

        [HttpPost]
        public ActionResult Create(Album album)
        {
            if (ModelState.IsValid)
            {
                _entities.Albums.Add(album);
                _entities.SaveChanges();
                return RedirectToAction("Index");  
            }

            ViewBag.GenreId = new SelectList(_entities.Genres, "GenreId", "Name", album.GenreId);
            ViewBag.ArtistId = new SelectList(_entities.Artists, "ArtistId", "Name", album.ArtistId);
            return View(album);
        }
        
        //
        // GET: /StoreManager/Edit/5
 
        public ActionResult Edit(int id)
        {
            Album album = _entities.Albums.Find(id);
            ViewBag.GenreId = new SelectList(_entities.Genres, "GenreId", "Name", album.GenreId);
            ViewBag.ArtistId = new SelectList(_entities.Artists, "ArtistId", "Name", album.ArtistId);
            return View(album);
        }

        //
        // POST: /StoreManager/Edit/5

        [HttpPost]
        public ActionResult Edit(Album album)
        {
            if (ModelState.IsValid)
            {
                _entities.Entry(album).State = EntityState.Modified;
                _entities.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.GenreId = new SelectList(_entities.Genres, "GenreId", "Name", album.GenreId);
            ViewBag.ArtistId = new SelectList(_entities.Artists, "ArtistId", "Name", album.ArtistId);
            return View(album);
        }

        //
        // GET: /StoreManager/Delete/5
 
        public ActionResult Delete(int id)
        {
            Album album = _entities.Albums.Find(id);
            return View(album);
        }

        //
        // POST: /StoreManager/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Album album = _entities.Albums.Find(id);
            _entities.Albums.Remove(album);
            _entities.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            _entities.Dispose();
            base.Dispose(disposing);
        }
    }
}