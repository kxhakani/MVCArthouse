using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using solution_MVC_Arthouse.DAL.AHEntities;
using solution_MVC_Arthouse.Models;

namespace solution_MVC_Arthouse.Controllers
{
    [Authorize(Roles ="Admin,Director")]
    public class ArtTypesController : Controller
    {
        private ArthouseEntities db = new ArthouseEntities();

        // GET: ArtTypes
        public ActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                return View("IndexAdmin", db.ArtTypes.ToList());
            }
            return View(db.ArtTypes.ToList());
        }

        // GET: ArtTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ArtType artType = db.ArtTypes.Include(a => a.Artworks)
                .Where(a => a.ID == id).SingleOrDefault();
            if (artType == null)
            {
                return HttpNotFound();
            }
            return View(artType);
        }

        // GET: ArtTypes/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: ArtTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePost([Bind(Include = "ID,Type")] ArtType artType)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.ArtTypes.Add(artType);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException)
            {
                ModelState.AddModelError("", "Unable to create the Art Type. Try again, and if the problem persists see your system administrator.");
            }

            return View(artType);
        }

        // GET: ArtTypes/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ArtType artType = db.ArtTypes.Find(id);
            if (artType == null)
            {
                return HttpNotFound();
            }
            return View(artType);
        }

        // POST: ArtTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var arttypeToUpdate = db.ArtTypes.Find(id);
            if (TryUpdateModel(arttypeToUpdate,"",
                new string[] { "Type"}))
            {
                try
                {
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }

            }
            return View(arttypeToUpdate);
        }

        // GET: ArtTypes/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ArtType artType = db.ArtTypes.Find(id);
            if (artType == null)
            {
                return HttpNotFound();
            }
            return View(artType);
        }

        // POST: ArtTypes/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ArtType arttype = db.ArtTypes.Find(id);
            if (id > 6)//Since there are no audit fields, this is a poor mans way to protect the original seeded data.
            {
                try
                {
                    db.ArtTypes.Remove(arttype);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (DataException dex)
                {
                    if (dex.InnerException.InnerException.Message.Contains("FK_"))
                    {
                        ModelState.AddModelError("", "Unable to delete art type. Remember, you cannot delete an art type that has artworks in the system.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Unable to delete art type. Try again, and if the problem persists see your system administrator.");
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "Unable to delete art type. You cannot delete data automatically seeded into the system.");
            }
            
            return View(arttype);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
