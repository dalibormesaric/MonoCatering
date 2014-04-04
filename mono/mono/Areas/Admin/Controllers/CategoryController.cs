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

namespace Mono.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    public class CategoryController : Controller
    {
        private IUnitOfWork unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        // GET: /Admin/Category/
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, IEnumerable<Category> subCategories)
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

            Expression<Func<Category, bool>> filter = null;

            if (!String.IsNullOrEmpty(searchString))
            {
                filter = (c =>
                    c.Name.ToUpper().Contains(searchString.ToUpper()) || 
                    (c.ParentCategory != null && c.ParentCategory.Name.ToUpper().Contains(searchString.ToUpper()))
                );
            }

            Func<IQueryable<Category>, IOrderedQueryable<Category>> orderBy = null;

            switch (sortOrder)
            {
                case "Name_desc":
                    orderBy = (q => q.OrderByDescending(c => c.Name));
                    break;
                case "ParentCategory":
                    orderBy = (q => q.OrderBy(c => c.ParentCategory == null ? "" : c.ParentCategory.Name));
                    break;
                case "ParentCategory_desc":
                    orderBy = (q => q.OrderByDescending(c => c.ParentCategory == null ? "" : c.ParentCategory.Name));
                    break;
                default:
                    orderBy = (q => q.OrderBy(c => c.Name));
                    break;
            }

            int pageNumber = (page ?? 1);

            //todo viewModel with unitOfWork.SizeValuesString(category.SizeType);

            if(subCategories == null)
            {
                var categories = unitOfWork.CategoryRepository.Get(filter: filter, orderBy: orderBy, includeProperties: "ParentCategory");
                return View("Index", categories.ToPagedList(pageNumber, Global.PageSize));
            }
            else
            {
                var query = subCategories.AsQueryable();

                if (filter != null)
                {
                    query = query.Where(filter);
                }

                var categories = orderBy(query);

                return View("SubCategory", categories.ToPagedList(pageNumber, Global.PageSize));
            }

        }

        // GET: /Admin/Category/SubCategory/5
        public ActionResult SubCategory(int? id, string sortOrder, string currentFilter, string searchString, int? page)
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
            ViewBag.CategoryID = id;

            var categories = category.ChildCategory.AsEnumerable();

            Stack<Category> childs = new Stack<Category>();
            childs.Push(category);

            do
            {
                category = childs.Pop();

                foreach (var child in category.ChildCategory)
                {
                    categories = categories.Union(child.ChildCategory);
                    childs.Push(child);
                }

            } while (childs.Count != 0);

            return Index(sortOrder, currentFilter, searchString, page, categories);
        }

        // GET: /Admin/Category/Details/5
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

            ViewBag.SizeValues = unitOfWork.SizeValuesString(category.SizeType);
            return View("Details", category);
        }

        // GET: /Admin/Category/Create
        public ActionResult Create()
        {
            setViewBagsParametres(null, null);
            return View("Create");
        }

        // POST: /Admin/Category/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,ParentCategoryID,PhotoID,SizeType")] Category category)
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

            setViewBagsParametres(category.ParentCategoryID, category.SizeType);
            return View("Create", category);
        }

        // GET: /Admin/Category/Edit/5
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
            setViewBagsParametres(category.ParentCategoryID, category.SizeType);
            return View("Edit", category);
        }

        // POST: /Admin/Category/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,ParentCategoryID,PhotoID,SizeType")] Category category)
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
            setViewBagsParametres(category.ParentCategoryID, category.SizeType);
            return View("Edit", category);
        }

        // GET: /Admin/Category/Delete/5
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
            ViewBag.SizeValues = unitOfWork.SizeValuesString(category.SizeType);
            return View("Delete", category);
        }

        // POST: /Admin/Category/Delete/5
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

        // GET: /Admin/Category/Ingredients/5
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

        private void setViewBagsParametres(int? parentCategoryID, int? sizeType)
        {
            ViewBag.ParentCategoryID = new SelectList(unitOfWork.CategoryRepository.Get(orderBy: q => q.OrderBy(c => c.Name)), "ID", "Name", parentCategoryID);
            ViewBag.SizeType = new SelectList(unitOfWork.SizeValuesSelectList(), "ID", "SizeValuesString", sizeType);
            ViewBag.Photos = new SelectList(unitOfWork.PhotoRepository.Get().Select(p => p.FileName));
        }
    }
}
