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
    public class CategoryController : Controller
    {
        private UnitOfWork unitOfWork;

        public CategoryController()
        {
            unitOfWork = new UnitOfWork();
        }

        public CategoryController(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        // GET: /AdminCategory/
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Name_desc" : "";
            ViewBag.ParentCategorySortParm = sortOrder == "ParentCategory" ? "ParentCategory_desc" : "ParentCategory";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var categories = unitOfWork.CategoryRepository.Get();

            if (!String.IsNullOrEmpty(searchString))
            {
                categories = categories.Where(c =>
                    c.Name.ToUpper().Contains(searchString.ToUpper())
                );
            }

            switch (sortOrder)
            {
                case "Name_desc":
                    categories = categories.OrderByDescending(c => c.Name);
                    break;
                case "ParentCategory":
                    categories = categories.OrderBy(c => c.ParentCategory == null ? "" : c.ParentCategory.Name);
                    break;
                case "ParentCategory_desc":
                    categories = categories.OrderByDescending(c => c.ParentCategory == null ? "" : c.ParentCategory.Name);
                    break;
                default:
                    categories = categories.OrderBy(c => c.Name);
                    break;
            }

            int pageSize = 3;
            int pageNumber = (page ?? 1);

            return View("Index", categories.ToPagedList(pageNumber, pageSize));
        }

        // GET: /AdminCategory/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = unitOfWork.CategoryRepository.GetByID((int)id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View("Details", category);
        }

        // GET: /AdminCategory/Create
        public ActionResult Create()
        {
            ViewBag.ParentCategoryID = new SelectList(unitOfWork.CategoryRepository.Get(orderBy: q => q.OrderBy(c => c.Name)), "ID", "Name");
            return View("Create");
        }

        // POST: /AdminCategory/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ID,Name,ParentCategoryID")] Category category)
        {
            try 
            { 
                if (ModelState.IsValid)
                {
                    unitOfWork.CategoryRepository.Insert(category);
                    unitOfWork.Save();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                ModelState.AddModelError(string.Empty, "Unable to save changes. Try again, and if the problem persists contact your system administrator.");
            }

            ViewBag.ParentCategoryID = new SelectList(unitOfWork.CategoryRepository.Get(orderBy: q => q.OrderBy(c => c.Name)), "ID", "Name", category.ParentCategoryID);
            return View("Create", category);
        }

        // GET: /AdminCategory/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = unitOfWork.CategoryRepository.GetByID((int)id);
            if (category == null)
            {
                return HttpNotFound();
            }
            ViewBag.ParentCategoryID = new SelectList(unitOfWork.CategoryRepository.Get(orderBy: q => q.OrderBy(c => c.Name)), "ID", "Name", category.ParentCategoryID);
            return View("Edit", category);
        }

        // POST: /AdminCategory/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID,Name,ParentCategoryID")] Category category)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    unitOfWork.CategoryRepository.Update(category);
                    unitOfWork.Save();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                ModelState.AddModelError(string.Empty, "Unable to save changes. Try again, and if the problem persists contact your system administrator.");
            }

            ViewBag.ParentCategoryID = new SelectList(unitOfWork.CategoryRepository.Get(orderBy: q => q.OrderBy(c => c.Name)), "ID", "Name", category.ParentCategoryID);
            return View("Edit", category);
        }

        // GET: /AdminCategory/Delete/5
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
            Category category = unitOfWork.CategoryRepository.GetByID((int)id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View("Delete", category);
        }

        // POST: /AdminCategory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Category category = unitOfWork.CategoryRepository.GetByID(id);
                unitOfWork.CategoryRepository.Delete(id);
                unitOfWork.Save();
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }
            return RedirectToAction("Index");
        }

        // GET: /AdminCategory/Ingredients/5
        public ActionResult Ingredients(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = unitOfWork.CategoryRepository.GetByID((int)id);

            if (category == null)
            {
                return HttpNotFound();
            }

            ViewBag.Category = category.Name;

            IEnumerable<Ingredient> ingredients = category.Ingredients;

            while (category.ParentCategoryID != null)
            {
                category = unitOfWork.CategoryRepository.GetByID((int)category.ParentCategoryID);

                ingredients = ingredients.Union(category.Ingredients);
            }

            ingredients = ingredients.OrderBy(i => i.Name);

            return View("Ingredients", ingredients.ToList());
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
