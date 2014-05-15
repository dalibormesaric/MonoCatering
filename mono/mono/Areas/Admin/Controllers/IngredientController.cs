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
using Mono.Helper;

namespace Mono.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    public class IngredientController : Controller
    {
        private IUnitOfWork unitOfWork;

        public IngredientController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        // GET: /Admin/Ingredient/
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            int pageNumber = ControllerHelper.newSearchPageNumber(ref searchString, page, currentFilter);   

            Expression<Func<Ingredient, bool>> filter = null;
            if (!String.IsNullOrEmpty(searchString))
            {
                filter = (i =>
                    i.Name.ToUpper().Contains(searchString.ToUpper()) ||
                    (i.Category != null && i.Category.Name.ToUpper().Contains(searchString.ToUpper())) ||
                    (i.Food != null && i.Food.Name.ToUpper().Contains(searchString.ToUpper()))
                );
            }

            Func<IQueryable<Ingredient>, IOrderedQueryable<Ingredient>> orderBy = null;
            switch (sortOrder)
            {
                case "Name_desc":
                    orderBy = (q => q.OrderByDescending(i => i.Name));
                    break;
                case "Category":
                    orderBy = (q => q.OrderBy(i => i.Category == null ? "" : i.Category.Name));
                    break;
                case "Category_desc":
                    orderBy = (q => q.OrderByDescending(i => i.Category == null ? "" : i.Category.Name));
                    break;
                case "Food":
                    orderBy = (q => q.OrderBy(i => i.Food == null ? "" : i.Food.Name));
                    break;
                case "Food_desc":
                    orderBy = (q => q.OrderByDescending(i => i.Food == null ? "" : i.Food.Name));
                    break;
                default:
                    orderBy = (q => q.OrderBy(i => i.Name));
                    break;
            }

            var ingredients = unitOfWork.IngredientRepository.Get(filter: filter, orderBy: orderBy, includeProperties: "Category, Food");

            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Name_desc" : "";
            ViewBag.CategorySortParm = sortOrder == "Category" ? "Category_desc" : "Category";
            ViewBag.FoodSortParm = sortOrder == "Food" ? "Food_desc" : "Food";

            return View("Index", ingredients.ToPagedList(pageNumber, int.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["PageSize"].ToString()) ));
        }

        // GET: /Admin/Ingredient/Details/5
        public ActionResult Details(int id)
        {
            Ingredient ingredient = unitOfWork.IngredientRepository.GetByID(id);
            if (ingredient == null)
            {
                return HttpNotFound();
            }

            return View("Details", ingredient);
        }

        // GET: /Admin/Ingredient/Create
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(unitOfWork.CategoryRepository.Get(), "ID", "Name");
            ViewBag.FoodID = new SelectList(unitOfWork.FoodRepository.Get(), "ID", "Name");

            return View("Create");
        }

        // POST: /Admin/Ingredient/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ID,Name,FoodID,CategoryID")] Ingredient ingredient)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    unitOfWork.IngredientRepository.Insert(ingredient);
                    unitOfWork.Save();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException /* dex */)
            {
                ModelState.AddModelError(string.Empty, "Unable to save changes. Try again, and if the problem persists contact your system administrator.");
            }

            ViewBag.CategoryID = new SelectList(unitOfWork.CategoryRepository.Get(orderBy: q => q.OrderBy(c => c.Name)), "ID", "Name", ingredient.CategoryID);
            ViewBag.FoodID = new SelectList(unitOfWork.FoodRepository.Get(orderBy: q => q.OrderBy(c => c.Name)), "ID", "Name", ingredient.FoodID);
            return View("Create", ingredient);
        }

        // GET: /Admin/Ingredient/Edit/5
        public ActionResult Edit(int id)
        {
            Ingredient ingredient = unitOfWork.IngredientRepository.GetByID(id);
            if (ingredient == null)
            {
                return HttpNotFound();
            }

            ViewBag.CategoryID = new SelectList(unitOfWork.CategoryRepository.Get(orderBy: q => q.OrderBy(c => c.Name)), "ID", "Name", ingredient.CategoryID);
            ViewBag.FoodID = new SelectList(unitOfWork.FoodRepository.Get(orderBy: q => q.OrderBy(c => c.Name)), "ID", "Name", ingredient.FoodID);

            return View("Edit", ingredient);
        }

        // POST: /Admin/Ingredient/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID,Name,FoodID,CategoryID")] Ingredient ingredient)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    unitOfWork.IngredientRepository.Update(ingredient);
                    unitOfWork.Save();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                ModelState.AddModelError(string.Empty, "Unable to save changes. Try again, and if the problem persists contact your system administrator.");
            }

            ViewBag.CategoryID = new SelectList(unitOfWork.CategoryRepository.Get(), "ID", "Name", ingredient.CategoryID);
            ViewBag.FoodID = new SelectList(unitOfWork.FoodRepository.Get(), "ID", "Name", ingredient.FoodID);

            return View("Edit", ingredient);
        }

        // GET: /Admin/Ingredient/Delete/5
        public ActionResult Delete(int id, bool? saveChangesError = false)
        {
            Ingredient ingredient = unitOfWork.IngredientRepository.GetByID(id);
            if (ingredient == null)
            {
                return HttpNotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }

            return View("Delete", ingredient);
        }

        // POST: /Admin/Ingredient/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Ingredient ingredient = unitOfWork.IngredientRepository.GetByID(id);
                unitOfWork.IngredientRepository.Delete(id);
                unitOfWork.Save();
            }
            catch (DataException /* dex */)
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
    }
}
