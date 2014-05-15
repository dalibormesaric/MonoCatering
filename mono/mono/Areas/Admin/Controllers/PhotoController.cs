using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Mono.Model;
using Mono.Data;
using PagedList;
using System.Linq.Expressions;
using Mono.Areas.Admin.Models;
using System.Web.Helpers;
using Mono.Helper;

namespace Mono.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    public class PhotoController : Controller
    {
        private IUnitOfWork unitOfWork;
        private IPhotoManager photoManager;
        private HttpServerUtilityBase server;

        public PhotoController(IUnitOfWork unitOfWork, IPhotoManager photoManager, HttpServerUtilityBase server)
        {
            this.unitOfWork = unitOfWork;
            this.photoManager = photoManager;
            this.server = server;
        }

        // GET: /Admin/Photo/
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            int pageNumber = ControllerHelper.newSearchPageNumber(ref searchString, page, currentFilter);   
            
            Expression<Func<Photo, bool>> filter = null;
            if (!String.IsNullOrEmpty(searchString))
            {
                filter = (c => c.FileName.ToUpper().Contains(searchString.ToUpper()));
            }

            Func<IQueryable<Photo>, IOrderedQueryable<Photo>> orderBy = null;
            switch (sortOrder)
            {
                case "Name_desc":
                    orderBy = (q => q.OrderByDescending(c => c.FileName));
                    break;
                default:
                    orderBy = (q => q.OrderBy(c => c.FileName));
                    break;
            }

            var categories = unitOfWork.PhotoRepository.Get(filter: filter, orderBy: orderBy);

            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Name_desc" : "";

            return View("Index", categories.ToPagedList(pageNumber, int.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["PageSize"].ToString()) ));
        }

        // GET: /Admin/Photo/PhotoUpload
        public ActionResult PhotoUpload()
        {
            return View("PhotoUpload");
        }

        // POST: /ImageUpload
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("PhotoUpload")]
        public ActionResult PhotoUploadSubmit([Bind(Include = "FileName, Image")] PhotoUploadViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Photo photo = new Photo { FileName = model.FileName };
                    WebImage image = photoManager.getImageFromRequest();

                    if (image == null)
                    {
                        ModelState.AddModelError(string.Empty, "No image.");
                    }
                    else if(unitOfWork.PhotoRepository.GetByID(photo.FileName) != null)
                    {
                        ModelState.AddModelError(string.Empty, "File name already taken.");
                    }
                    else
                    {
                        unitOfWork.PhotoRepository.Insert(photo);
                        unitOfWork.Save();

                        photoManager.resize(image, width: 300, height: 300);
                        photoManager.savePhoto(image, filePath(photo.FileName), "png");

                        return RedirectToAction("Index");
                    }
                }
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                ModelState.AddModelError(string.Empty, "Unable to save changes. Try again, and if the problem persists contact your system administrator.");
            }

            return View("PhotoUpload", model);
        }

        // GET: /Admin/Photo/Edit/5
        public ActionResult Edit(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Photo photo = unitOfWork.PhotoRepository.GetByID(id);
            if (photo == null)
            {
                return HttpNotFound();
            }

            return View("Edit", new PhotoEditViewModel { FileName = photo.FileName, NewFileName = photo.FileName });
        }

        // POST: /Admin/Photo/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "FileName, NewFileName")] PhotoEditViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (unitOfWork.PhotoRepository.GetByID(model.NewFileName) != null)
                    {
                        ModelState.AddModelError(string.Empty, "File name already taken.");
                    }
                    else if (!photoManager.photoExist(serverFilePath(model.FileName)))
                    {
                        ModelState.AddModelError(string.Empty, "Photo doesn't exist.");
                    }
                    else
                    {
                        unitOfWork.PhotoRepository.Delete(model.FileName);
                        var photo = new Photo { FileName = model.NewFileName };
                        unitOfWork.PhotoRepository.Insert(photo);
                        unitOfWork.Save();

                        photoManager.changePhotoName(serverFilePath(model.FileName), serverFilePath(model.NewFileName));

                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception /* dex */)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                ModelState.AddModelError(string.Empty, "Unable to save changes. Try again, and if the problem persists contact your system administrator.");
            }

            return View("Edit", model);
        }

        // GET: /Admin/Photo/Delete/5
        public ActionResult Delete(string id, bool? saveChangesError = false)
        {
            if (String.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Photo photo = unitOfWork.PhotoRepository.GetByID(id);
            if (photo == null)
            {
                return HttpNotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }

            return View("Delete", photo);
        }

        // POST: /Admin/Photo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            try
            {
                Photo photo = unitOfWork.PhotoRepository.GetByID(id);
                var path = serverFilePath(photo.FileName);
               
                if(!photoManager.photoExist(path))
                    throw new Exception();

                foreach(var category in unitOfWork.CategoryRepository.Get(c => c.PhotoID == photo.FileName))
                {
                    category.PhotoID = "";
                    unitOfWork.CategoryRepository.Update(category);
                }

                foreach (var food in unitOfWork.FoodRepository.Get(c => c.PhotoID == photo.FileName))
                {
                    food.PhotoID = "";
                    unitOfWork.FoodRepository.Update(food);
                }

                unitOfWork.PhotoRepository.Delete(photo.FileName);
                unitOfWork.Save();
                photoManager.deletePhoto(path);

            }
            catch (Exception /* dex */)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                unitOfWork.Dispose();
            }
            base.Dispose(disposing);
        }

        private string filePath(string fileName)
        {
            return (@"~/Content/Images/" + fileName + ".png");
        }

        private string serverFilePath(string fileName)
        {
            return server.MapPath(filePath(fileName));
        }

    }
}
