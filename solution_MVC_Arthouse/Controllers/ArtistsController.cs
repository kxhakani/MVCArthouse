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
using System.Data.Entity.Infrastructure;
using solution_MVC_Arthouse.ViewModels;

namespace solution_MVC_Arthouse.Controllers
{
    [Authorize]
    public class ArtistsController : Controller
    {
        private ArthouseEntities db = new ArthouseEntities();

        // GET: Artists
        public ActionResult Index()
        {
            var artists = db.Artists
                .Include(a => a.Studios)
                .OrderBy(a=>a.FirstName)
                .ThenBy(a => a.LastName);

            if (User.IsInRole("Admin"))
            {
                return View("IndexAdmin", artists.ToList());
            }
            else if (User.IsInRole("Director"))
            {
                return View("IndexDirector", artists.ToList());
            }
            return View(artists.ToList());
        }

        // GET: Artists/Details/5
        [Authorize(Roles = "Admin,Director")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Artist artist = db.Artists
                .Include(a=>a.Artworks)
                .Include(a =>a.Studios)
                .Where(a=>a.ID==id).SingleOrDefault();
            if (artist == null)
            {
                return HttpNotFound();
            }
            return View(artist);
        }

        // GET: Artists/Create
        [Authorize(Roles = "Admin,Director")]
        public ActionResult Create()
        {
            var artist = new Artist();
            artist.Studios = new List<Studio>();
            PopulateAssignedStudioData(artist);
            return View();
        }

        // POST: Artists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,Director")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,FirstName,MiddleName,LastName,Phone,DOB,Rate")] Artist artist, string[] selectedStudios)
        {
            try
            {
                //Add the selected conditions
                if (selectedStudios != null)
                {
                    artist.Studios = new List<Studio>();
                    foreach (var st in selectedStudios)
                    {
                        var studioToAdd = db.Studios.Find(int.Parse(st));
                        artist.Studios.Add(studioToAdd);
                    }
                }
                if (ModelState.IsValid)
                {
                    db.Artists.Add(artist);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes - Retry Limit Exceeded. Try again, and if the problem persists, see your system administrator.");
            }
            catch (DataException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            PopulateAssignedStudioData(artist);
            return View(artist);
        }

        // GET: Artists/Edit/5
        [Authorize(Roles = "Admin,Director")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Artist artist = db.Artists
                .Include(p => p.Studios)
                .Where(i => i.ID == id)
                .Single();
            if (artist == null)
            {
                return HttpNotFound();
            }
            PopulateAssignedStudioData(artist);
            return View(artist);
        }

        // POST: Artists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,Director")]
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? id, Byte[] rowVersion, string[] selectedStudios)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var artistToUpdate = db.Artists
                .Include(p => p.Studios)
                .Where(i => i.ID == id)
                .Single();
            //Only ADMIN or the CreatedBy can edit
            if (!(artistToUpdate.CreatedBy == User.Identity.Name || User.IsInRole("Admin")))
            {
                ModelState.AddModelError("", "Unable to modify Artist. Only an Administrator or the person who created the record can change it.");
            }
            else
            {
                if (TryUpdateModel(artistToUpdate, "",
                new string[] { "FirstName", "MiddleName", "LastName", "Phone", "DOB", "Rate" }))
                {
                    try
                    {
                        UpdateArtistStudios(selectedStudios, artistToUpdate);
                        db.Entry(artistToUpdate).OriginalValues["RowVersion"] = rowVersion;
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    catch (RetryLimitExceededException /* dex */)
                    {
                        //Log the error (uncomment dex variable name and add a line here to write a log.
                        ModelState.AddModelError("", "Unable to save changes - Retry Limit Exceeded. Try again, and if the problem persists, see your system administrator.");
                    }
                    catch (DbUpdateConcurrencyException ex)// Added for concurrency
                    {
                        var entry = ex.Entries.Single();
                        var clientValues = (Artist)entry.Entity;
                        var databaseEntry = entry.GetDatabaseValues();
                        if (databaseEntry == null)
                        {
                            ModelState.AddModelError(string.Empty,
                                "Unable to save changes. The Artist was deleted by another user.");
                        }
                        else
                        {
                            var databaseValues = (Artist)databaseEntry.ToObject();
                            if (databaseValues.FirstName != clientValues.FirstName)
                                ModelState.AddModelError("FirstName", "Current value: "
                                    + databaseValues.FirstName);
                            if (databaseValues.MiddleName != clientValues.MiddleName)
                                ModelState.AddModelError("MiddleName", "Current value: "
                                    + databaseValues.MiddleName);
                            if (databaseValues.LastName != clientValues.LastName)
                                ModelState.AddModelError("LastName", "Current value: "
                                    + databaseValues.LastName);
                            if (databaseValues.Phone != clientValues.Phone)
                                ModelState.AddModelError("Phone", "Current value: "
                                    + String.Format("{0:(###) ###-####}", databaseValues.Phone));
                            if (databaseValues.DOB != clientValues.DOB)
                                ModelState.AddModelError("DOB", "Current value: "
                                    + String.Format("{0:d}", databaseValues.DOB));
                            if (databaseValues.Rate != clientValues.Rate)
                                ModelState.AddModelError("Rate", "Current value: "
                                    + databaseValues.Rate);
                            ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                                + "was modified by another user after you received your values. The "
                                + "edit operation was canceled and the current values in the database "
                                + "have been displayed. If you still want to edit this record, click "
                                + "the Save button again. Otherwise click the 'Back to List' hyperlink.");
                            artistToUpdate.RowVersion = databaseValues.RowVersion;
                        }
                    }
                    catch (DataException)
                    {
                        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                    }
                }
            }
            PopulateAssignedStudioData(artistToUpdate);
            return View(artistToUpdate);
        }

        // GET: Artists/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Artist artist = db.Artists.Find(id);
            if (artist == null)
            {
                return HttpNotFound();
            }
            return View(artist);
        }

        // POST: Artists/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Artist artist = db.Artists.Find(id);
            //Proptect teh seeded data
            if (artist.CreatedBy == "Unknown")
            {
                ModelState.AddModelError("", "Unable to delete Artist. You cannot delete data automatically seeded into the system.");
            }
            else
            {
                try
                {
                    db.Artists.Remove(artist);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes - Retry Limit Exceeded. Try again, and if the problem persists, see your system administrator.");
                }
                catch (DataException dex)
                {
                    if (dex.InnerException.InnerException.Message.Contains("FK_"))
                    {
                        ModelState.AddModelError("", "Unable to delete artist. Remember, you cannot delete an artist that has artworks in the system.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Unable to delete artist. Try again, and if the problem persists see your system administrator.");
                    }
                }
            }
            return View(artist);
        }

        private void PopulateAssignedStudioData(Artist artist)
        {
            var allStudios = db.Studios;
            var pStudios = new HashSet<int>(artist.Studios.Select(b => b.ID));
            var viewModel = new List<ArtistStudioVM>();
            foreach (var st in allStudios)
            {
                viewModel.Add(new ArtistStudioVM
                {
                    StudioID = st.ID,
                    StudioName = st.StudioName,
                    Assigned = pStudios.Contains(st.ID)
                });
            }
            ViewBag.Studios = viewModel;
        }

        private void UpdateArtistStudios(string[] selectedStudios, Artist artistToUpdate)
        {
            if (selectedStudios == null)
            {
                artistToUpdate.Studios = new List<Studio>();
                return;
            }

            var selectedStudiosHS = new HashSet<string>(selectedStudios);
            var artistStudios = new HashSet<int>
                (artistToUpdate.Studios.Select(c => c.ID));//IDs of the currently selected Studios
            foreach (var studio in db.Studios)
            {
                if (selectedStudiosHS.Contains(studio.ID.ToString()))
                {
                    if (!artistStudios.Contains(studio.ID))
                    {
                        artistToUpdate.Studios.Add(studio);
                    }
                }
                else
                {
                    if (artistStudios.Contains(studio.ID))
                    {
                        artistToUpdate.Studios.Remove(studio);
                    }
                }
            }
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
