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
    public class FoodController : Controller
    {
        private IUnitOfWork unitOfWork;

        public FoodController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        // GET: /Admin/Food/
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, IEnumerable<Food> foodsForCategory)
        {
            int pageNumber = ControllerHelper.newSearchPageNumber(ref searchString, page, currentFilter);   

            Expression<Func<Food, bool>> filter = null;
            if (!String.IsNullOrEmpty(searchString))
            {
                filter = (f =>
                    f.Name.ToUpper().Contains(searchString.ToUpper()) ||
                    (f.Category != null && f.Category.Name.ToUpper().Contains(searchString.ToUpper()))
                );
            }

            Func<IQueryable<Food>, IOrderedQueryable<Food>> orderBy = null;
            switch (sortOrder)
            {
                case "Name_desc":
                    orderBy = (q => q.OrderByDescending(f => f.Name));
                    break;
                case "Category":
                    orderBy = (q => q.OrderBy(f => f.Category.Name));
                    break;
                case "Category_desc":
                    orderBy = (q => q.OrderByDescending(f => f.Category.Name));
                    break;
                default:
                    orderBy = (q => q.OrderBy(f => f.Name));
                    break;
            }

            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Name_desc" : "";
            ViewBag.CategorySortParm = sortOrder == "Category" ? "Category_desc" : "Category";

            if (foodsForCategory == null)
            {
                var foods = unitOfWork.FoodRepository.Get(filter: filter, orderBy: orderBy, includeProperties: "Category");
                
                return View("Index", foods.ToPagedList(pageNumber, int.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["PageSize"].ToString()) ));
            }
            else
            {
                var query = foodsForCategory.AsQueryable();

                if (filter != null)
                {
                    query = query.Where(filter);
                }

                var foods = orderBy(query);
                
                return View("Category", foods.ToPagedList(pageNumber, int.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["PageSize"].ToString()) ));
            }
        }
      
        // GET: /Admin/Food/Category/5
        public ActionResult Category(int id, string sortOrder, string currentFilter, string searchString, int? page)
        {
            Category category = unitOfWork.CategoryRepository.GetByID(id);
            if (category == null)
            {
                return HttpNotFound();
            }

            ViewBag.Category = category.Name;
            ViewBag.CategoryID = id;

            return Index(sortOrder, currentFilter, searchString, page, unitOfWork.FoodRepository.FoodInCategory(category));
        }

        // GET: /Admin/Food/Details/5
        public ActionResult Details(int id)
        {
            Food food = unitOfWork.FoodRepository.GetByID(id);
            if (food == null)
            {
                return HttpNotFound();
            }

            return View("Details", food);
        }

        // GET: /Admin/Food/Create
        public ActionResult Create()
        {
            setViewBagsParametres(null);
            return View("Create");
        }

        // POST: /Admin/Food/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ID,Name,CategoryID,PhotoID")] Food food)
        {
            try 
            { 
                if (ModelState.IsValid)
                {
                    unitOfWork.FoodRepository.Insert(food);
                    unitOfWork.Save();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                ModelState.AddModelError(string.Empty, "Unable to save changes. Try again, and if the problem persists contact your system administrator.");
            }

            setViewBagsParametres(food.CategoryID);

            return View("Create", food);
        }

        // GET: /Admin/Food/Edit/5
        public ActionResult Edit(int id)
        {
            Food food = unitOfWork.FoodRepository.GetByID(id);
            if (food == null)
            {
                return HttpNotFound();
            }

            setViewBagsParametres(food.CategoryID);

            return View("Edit", food);
        }

        // POST: /Admin/Food/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID,Name,CategoryID,PhotoID")] Food food)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    unitOfWork.FoodRepository.Update(food);
                    unitOfWork.Save();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                ModelState.AddModelError(string.Empty, "Unable to save changes. Try again, and if the problem persists contact your system administrator.");
            }

            setViewBagsParametres(food.CategoryID);

            return View("Edit", food);
        }

        // GET: /Admin/Food/Delete/5
        public ActionResult Delete(int id, bool? saveChangesError = false)
        {
            Food food = unitOfWork.FoodRepository.GetByID(id);
            if (food == null)
            {
                return HttpNotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }

            return View("Delete", food);
        }

        // POST: /Admin/Food/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Food food = unitOfWork.FoodRepository.GetByID(id);
                unitOfWork.FoodRepository.Delete(id);
                unitOfWork.Save();
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }
            return RedirectToAction("Index");
        }

        // GET: /Admin/Food/Ingredients/5
        public ActionResult Ingredients(int id)
        {
            Food food = unitOfWork.FoodRepository.GetByID(id);
            if (food == null)
            {
                return HttpNotFound();
            }
            
            ViewBag.Food = food.Name;

            return View("Ingredients", unitOfWork.IngredientRepository.IngredientsForFood(food));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                unitOfWork.Dispose();
            }
            base.Dispose(disposing);
        }

        private void setViewBagsParametres(int? categoryID)
        {
            ViewBag.CategoryID = new SelectList(unitOfWork.CategoryRepository.Get(orderBy: q => q.OrderBy(c => c.Name)), "ID", "Name", categoryID);
            ViewBag.Photos = new SelectList(unitOfWork.PhotoRepository.Get().Select(p => p.FileName));
        }
    }
}
