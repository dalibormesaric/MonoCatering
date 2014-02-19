using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using mono.Models;
using mono.DAL;
using PagedList;

namespace mono.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    public class CategorySizeController : Controller
    {
        private UnitOfWork unitOfWork;

        public CategorySizeController()
        {
            unitOfWork = new UnitOfWork();
        }

        public CategorySizeController(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        // GET: /Admin/CategorySize/
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Value_desc" : "";
            ViewBag.TypeSortParm = sortOrder == "Type" ? "Type_desc" : "Type";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var categorySizes = unitOfWork.CategorySizeRepository.Get();

            if (!String.IsNullOrEmpty(searchString))
            {
                categorySizes = categorySizes.Where(s =>
                    s.Type.ToString().Contains(searchString.ToUpper()) ||
                    s.Value.ToUpper().Contains(searchString.ToUpper())
                );
            }

            switch (sortOrder)
            {
                case "Value_desc":
                    categorySizes = categorySizes.OrderByDescending(s => s.Value);
                    break;
                case "Type":
                    categorySizes = categorySizes.OrderBy(s => s.Type);
                    break;
                case "Type_desc":
                    categorySizes = categorySizes.OrderByDescending(s => s.Type);
                    break;
                default:
                    categorySizes = categorySizes.OrderBy(s => s.Value);
                    break;
            }

            int pageSize = 3;
            int pageNumber = (page ?? 1);

            return View("Index", categorySizes.ToPagedList(pageNumber, pageSize));
        }

        // GET: /Admin/CategorySize/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CategorySize categorySize = unitOfWork.CategorySizeRepository.GetByID((int)id);
            if (categorySize == null)
            {
                return HttpNotFound();
            }
            return View("Details", categorySize);
        }

        // GET: /Admin/CategorySize/Create
        public ActionResult Create()
        {
            return View("Create");
        }

        // POST: /Admin/CategorySize/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ID,Type,Value,Order")] CategorySize categorySize)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    unitOfWork.CategorySizeRepository.Insert(categorySize);
                    unitOfWork.Save();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                ModelState.AddModelError(string.Empty, "Unable to save changes. Try again, and if the problem persists contact your system administrator.");
            }

            return View("Create", categorySize);
        }

        // GET: /Admin/CategorySize/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CategorySize categorySize = unitOfWork.CategorySizeRepository.GetByID((int)id);
            if (categorySize == null)
            {
                return HttpNotFound();
            }
            return View("Edit", categorySize);
        }

        // POST: /Admin/CategorySize/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID,Type,Value,Order")] CategorySize categorySize)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    unitOfWork.CategorySizeRepository.Update(categorySize);
                    unitOfWork.Save();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                ModelState.AddModelError(string.Empty, "Unable to save changes. Try again, and if the problem persists contact your system administrator.");
            }

            return View("Edit", categorySize);
        }

        // GET: /Admin/CategorySize/Delete/5
        public ActionResult Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }
            CategorySize categorySize = unitOfWork.CategorySizeRepository.GetByID((int)id);
            if (categorySize == null)
            {
                return HttpNotFound();
            }
            return View("Delete", categorySize);
        }

        // POST: /Admin/CategorySize/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                CategorySize categorySize = unitOfWork.CategorySizeRepository.GetByID(id);
                unitOfWork.CategorySizeRepository.Delete(id);
                unitOfWork.Save();
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }
            return RedirectToAction("Index");
        }

        // GET: /Admin/CategorySize/Type/5
        public ActionResult Type(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            if (unitOfWork.CategorySizeRepository.Get(s => s.Type == (int)id).FirstOrDefault() == null)
            {
                return HttpNotFound();
            }

            ViewBag.SizeValuesString = unitOfWork.SizeValuesString((int)id);

            return View("Type");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                unitOfWork.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
