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
using System.IO;

namespace solution_MVC_Arthouse.Controllers
{
    [Authorize]
    public class ArtworksController : Controller
    {
        private ArthouseEntities db = new ArthouseEntities();

        // GET: Artworks
        public ActionResult Index(string sortDirection, string sortField, string sortButton, string SearchString, int? ArtistID, int? ArtTypeID)
        {
            PopulateDropDownLists();
            ViewBag.Filtering = ""; //Assume not filtering

            //Start with Includes
            var artworks = db.Artworks.Include(a => a.Artist).Include(a => a.ArtType);

            //Add as many filters as needed
            if (ArtistID.HasValue)
            {
                artworks = artworks.Where(p => p.ArtistID == ArtistID);
                ViewBag.Filtering = " in";//Flag filtering
            }
                
            if (ArtTypeID.HasValue)
            {
                artworks = artworks.Where(p => p.ArtTypeID == ArtTypeID);
                ViewBag.Filtering = " in";//Flag filtering
            }
            if (!String.IsNullOrEmpty(SearchString))
            {
                artworks = artworks.Where(p => p.Name.ToUpper().Contains(SearchString.ToUpper())
                                       || p.Description.ToUpper().Contains(SearchString.ToUpper()));
                ViewBag.Filtering = " in";//Flag filtering
            }

            //Add sorting
            if (!String.IsNullOrEmpty(sortButton)) //Form Submitted so lets sort!
            {
                if(sortButton!="Filter")//Change of sort is requested
                {
                    sortField = sortButton;//Sort by the button clicked - Else will take care of Filter submit
                    if (sortButton == sortField) //Reverse order on same field
                    {
                        sortDirection = String.IsNullOrEmpty(sortDirection) ? "desc" : "";
                    }
                }
                //Now we know which field and direction to sort by, but a Switch is hard to use for 2 criteria
                //so we will use an if() structure instead.
                if (sortField.Contains("Value"))//Sorting by Est. Value
                {
                    if (String.IsNullOrEmpty(sortDirection))
                    {
                        artworks = artworks.OrderBy(p => p.Value);
                    }
                    else
                    {
                        artworks = artworks.OrderByDescending(p => p.Value);
                    }
                }else if (sortField.Contains("Artwork"))//Sorting by Artwork Summay
                {
                    if (String.IsNullOrEmpty(sortDirection))
                    {
                        artworks = artworks
                        .OrderBy(p => p.Name)
                        .ThenBy(p => p.Finished);
                    }
                    else
                    {
                        artworks = artworks
                       .OrderByDescending(p => p.Name)
                       .ThenByDescending(p => p.Finished);
                    }
                }
            }

            //Set sort for next time
            ViewBag.sortField = sortField;
            ViewBag.sortDirection = sortDirection;

            if (User.IsInRole("Admin"))
            {
                return View("IndexAdmin",artworks.ToList());
            }
            else if (User.IsInRole("Director"))
            {
                return View("IndexDirector", artworks.ToList());
            }
                return View(artworks.ToList());
        }

        // GET: Artworks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Artwork artwork = db.Artworks.Find(id);
            if (artwork == null)
            {
                return HttpNotFound();
            }
            return View(artwork);
        }

        // GET: Artworks/
        [Authorize(Roles = "Admin,Director")]
        public ActionResult Create()
        {
            PopulateDropDownLists();
            return View();
        }

        // POST: Artworks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,Director")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,Started,Finished,Description,Value,ArtistID,ArtTypeID")] Artwork artwork, IEnumerable<HttpPostedFileBase> artworkFiles)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    HttpPostedFileBase f = Request.Files["thePicture"];
                    string mimeType = f.ContentType;
                    int fileLength = f.ContentLength;
                    if(!(mimeType==""  ||  fileLength==0))
                    {
                        string fileName = Path.GetFileName(f.FileName);
                        Stream fileStream = f.InputStream;
                        byte[] fileData = new byte[fileLength];
                        fileStream.Read(fileData, 0, fileLength);

                        if(mimeType.Contains("image"))
                        {
                            artwork.imageContent = fileData;
                            artwork.imageMimeType = mimeType;
                            artwork.imageFileName = fileName;
                        }
                    }

                    AddDocuments(ref artwork, artworkFiles);
                    db.Artworks.Add(artwork);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes - Retry Limit Exceeded. Try again, and if the problem persists, see your system administrator.");
            }
            catch (DataException dex)
            {
                if (dex.InnerException.InnerException.Message.Contains("IX_"))
                {
                    ModelState.AddModelError("Name", "Unable to save changes. Remember, you cannot have duplicate Names on the same Started date.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }
            
            PopulateDropDownLists(artwork);
            return View(artwork);
        }


        private void AddDocuments(ref Artwork artwork, IEnumerable<HttpPostedFileBase> artworkFiles)
        {
            foreach (var doc in artworkFiles)
            {
                if (doc != null)
                {
                    string mimeType = doc.ContentType;
                    string fileName = Path.GetFileName(doc.FileName);
                    int fileLength = doc.ContentLength;

                    if (!(fileName == "" || fileLength == 0))
                    {
                        Stream docfileStream = doc.InputStream;
                        byte[] docfileData = new byte[fileLength];
                        docfileStream.Read(docfileData, 0, fileLength);

                        UploadedFiles newFile = new UploadedFiles
                        {
                            FileContent = docfileData,
                            MimeType = mimeType,
                            FileName = fileName
                        };

                        artwork.UploadedFiles.Add(newFile);
                    }
                }
            }
        }
    

        // GET: Artworks/Edit/5
        [Authorize(Roles = "Admin,Director")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Artwork artwork = db.Artworks.Find(id);
            if (artwork == null)
            {
                return HttpNotFound();
            }
            PopulateDropDownLists(artwork);
            return View(artwork);
        }

        // POST: Artworks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,Director")]
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? id, Byte[] rowVersion, string chkRemoveImage)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var artworkToUpdate = db.Artworks.Find(id);
            //Only ADMIN or the CreatedBy can edit
            if (!(artworkToUpdate.CreatedBy == User.Identity.Name || User.IsInRole("Admin")))
            {
                ModelState.AddModelError("", "Unable to modify Artwork. Only an Administrator or the person who created the record can change it.");
            }
            else
            {
                if (TryUpdateModel(artworkToUpdate, "",
                new string[] { "Name", "Started", "Finished", "Description", "Value", "ArtistID", "ArtTypeID" }))
                {
                    try
                    {
                        if(chkRemoveImage !=null)
                        {
                            artworkToUpdate.imageContent = null;
                            artworkToUpdate.imageMimeType = null;
                            artworkToUpdate.imageFileName = null;
                        }

                        HttpPostedFileBase f = Request.Files["newPicture"];
                        string mimeType = f.ContentType;
                        int fileLength = f.ContentLength;
                        if(!(mimeType == ""  ||  fileLength == 0))
                        {
                            string fileName = Path.GetFileName(f.FileName);
                            Stream filestream = f.InputStream;
                            byte[] fileData = new byte[fileLength];
                            filestream.Read(fileData, 0, fileLength);

                            if(mimeType.Contains("image"))
                            {
                                artworkToUpdate.imageContent = fileData;
                                artworkToUpdate.imageMimeType = mimeType;
                                artworkToUpdate.imageFileName = fileName;
                            }
                        }

                        db.Entry(artworkToUpdate).OriginalValues["RowVersion"] = rowVersion;
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
                        var clientValues = (Artwork)entry.Entity;
                        var databaseEntry = entry.GetDatabaseValues();
                        if (databaseEntry == null)
                        {
                            ModelState.AddModelError(string.Empty,
                                "Unable to save changes. The Artwork was deleted by another user.");
                        }
                        else
                        {
                            var databaseValues = (Artwork)databaseEntry.ToObject();
                            if (databaseValues.Name != clientValues.Name)
                                ModelState.AddModelError("Name", "Current value: "
                                    + databaseValues.Name);
                            if (databaseValues.Started != clientValues.Started)
                                ModelState.AddModelError("Started", "Current value: "
                                    + String.Format("{0:d}", databaseValues.Started));
                            if (databaseValues.Finished != clientValues.Finished)
                                ModelState.AddModelError("Finished", "Current value: "
                                    + String.Format("{0:d}", databaseValues.Finished));
                            if (databaseValues.Description != clientValues.Description)
                                ModelState.AddModelError("Description", "Current value: "
                                    + databaseValues.Description);
                            if (databaseValues.Value != clientValues.Value)
                                ModelState.AddModelError("Value", "Current value: "
                                    + String.Format("{0:c2}", databaseValues.Value));
                            if (databaseValues.ArtistID != clientValues.ArtistID)
                                ModelState.AddModelError("ArtistID", "Current value: "
                                    + db.Artists.Find(databaseValues.ArtistID).FullName);
                            if (databaseValues.ArtTypeID != clientValues.ArtTypeID)
                                ModelState.AddModelError("ArtTypeID", "Current value: "
                                    + db.ArtTypes.Find(databaseValues.ArtTypeID).Type);
                            ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                                + "was modified by another user after you received your values. The "
                                + "edit operation was canceled and the current values in the database "
                                + "have been displayed. If you still want to save over this record, click "
                                + "the Save button again. Otherwise click the 'Back to List' hyperlink.");
                            artworkToUpdate.RowVersion = databaseValues.RowVersion;
                        }
                    }
                    catch (DataException dex)
                    {
                        if (dex.InnerException.InnerException.Message.Contains("IX_"))
                        {
                            ModelState.AddModelError("Name", "Unable to save changes. Remember, you cannot have duplicate Names on the same Started date.");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                        }
                    }
                }
            }

            PopulateDropDownLists(artworkToUpdate);
            return View(artworkToUpdate);
        }

        // GET: Artworks/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Artwork artwork = db.Artworks.Find(id);
            if (artwork == null)
            {
                return HttpNotFound();
            }
            return View(artwork);
        }

        // POST: Artworks/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Artwork artwork = db.Artworks.Find(id);
            //Protect the seeded data
            if(artwork.CreatedBy=="Unknown")
            {
                ModelState.AddModelError("", "Unable to delete. You cannot delete data automatically seeded into the system.");
            }
            else
            {
                try
                {
                    db.Artworks.Remove(artwork);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes - Retry Limit Exceeded. Try again, and if the problem persists, see your system administrator.");
                }
                catch (DataException)//Note: there is really no reason a delete should fail if you can "talk" to the database.
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }
            
            return View(artwork);
        }
        private void PopulateDropDownLists(Artwork artwork = null)
        {
            ViewBag.ArtistID = new SelectList(db.Artists
                .OrderBy(a=>a.LastName)
                .ThenBy(a=>a.FirstName), "ID", "FormalName", artwork?.ArtistID);
            ViewBag.ArtTypeID = new SelectList(db.ArtTypes
                .OrderBy(t=>t.Type), "ID", "Type", artwork?.ArtTypeID);
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
