using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using mvcTest.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace mvcTest.Controllers {
    public class MovieController : Controller {
        MyProjectContext db = new MyProjectContext ();
        public IActionResult Index () {
            var movie = db.Movie.ToList ();
            return View (movie);
        }
        public string Welcome (string name, string id) {
            return $"WelcomeJa Name:{name} ID:{id}";
        }

        public IActionResult Create () {
            return View ();
        }

        [HttpPost]
        public async Task<IActionResult> Create (MovieModel model, IFormFile fileUpload) {
            if (model.duration < 1) {
                ModelState.AddModelError ("errDuration", "The duration field is required.");
                return View ();
            }
            if (fileUpload == null) {
                ModelState.AddModelError ("errFileUpload", "The file upload field is required.");
                return View ();
            }
            if (ModelState.IsValid) {
                string pathImgMovie = "/images/movie";
                string pathSave = $"wwwroot{pathImgMovie}";
                if (!Directory.Exists (pathSave)) {
                    Directory.CreateDirectory (pathSave);
                }
                string extFile = Path.GetExtension (fileUpload.FileName);
                string fileName = DateTime.Now.ToString ("dd-MM-yyyy-hh-mm-ss") + extFile;
                var path = Path.Combine (Directory.GetCurrentDirectory (), pathSave, fileName);
                using (var stream = new FileStream (path, FileMode.Create)) {
                    await fileUpload.CopyToAsync (stream);
                }
                DateTime dateNow = DateTime.Now;
                model.coverImg = pathImgMovie + "/" + fileName;
                model.createDate = dateNow;
                model.modifyDate = dateNow;
                db.Movie.Add (model);
                await db.SaveChangesAsync ();
                return RedirectToAction ("Index");
            }
            return View ();
        }

        [HttpPost]
        public ActionResult Delete (int id) {
            MovieModel movie = db.Movie.Find (id);
            db.Movie.Remove (movie);
            db.SaveChanges ();
            return RedirectToAction ("Index");
        }

        public ActionResult Edit (int id) {
            MovieModel movie = db.Movie.Find (id);
            return View (movie);
        }

        [HttpPost]
        public async Task<IActionResult> Edit (MovieModel model, IFormFile fileUpload) {
            if (model.duration < 1) {
                ModelState.AddModelError ("errDuration", "The duration field is required.");
                return View ();
            }

            //set old data
            db.Movie.Attach (model);
            // MovieModel oldMovie = db.Movie.Find(model.id);
            MovieModel oldMovie = new MyProjectContext ().Movie.Find (model.id);
            model.coverImg = oldMovie.coverImg;
            model.createDate = oldMovie.createDate;
            oldMovie = null;
            if (ModelState.IsValid) {
                if (fileUpload != null) {
                    string pathImgMovie = "/images/movie";
                    string pathSave = $"wwwroot{pathImgMovie}";
                    if (!Directory.Exists (pathSave)) {
                        Directory.CreateDirectory (pathSave);
                    }
                    string extFile = Path.GetExtension (fileUpload.FileName);
                    string fileName = DateTime.Now.ToString ("dd-MM-yyyy-hh-mm-ss") + extFile;
                    var path = Path.Combine (Directory.GetCurrentDirectory (), pathSave, fileName);
                    using (var stream = new FileStream (path, FileMode.Create)) {
                        await fileUpload.CopyToAsync (stream);
                    }
                    model.coverImg = pathImgMovie + fileName;
                }
            }
            model.modifyDate=DateTime.Now;
            db.Entry(model).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}