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
using System.Linq.Expressions;

namespace mono.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    public class FoodController : Controller
    {
        private UnitOfWork unitOfWork;

        public FoodController()
        {
            unitOfWork = new UnitOfWork();
        }

        public FoodController(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        // GET: /AdminFood/
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Name_desc" : "";
            ViewBag.CategorySortParm = sortOrder == "Category" ? "Category_desc" : "Category";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

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

            var foods = unitOfWork.FoodRepository.Get(filter: filter, orderBy: orderBy, includeProperties: "Category");
            int pageNumber = (page ?? 1);

            return View("Index", foods.ToPagedList(pageNumber, Global.PageSize));
        }
      
        // GET: /AdminCategory/Category/5
        public ActionResult Category(int? id)
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
          
            var foods = category.Food.AsEnumerable();

            Stack<Category> childs = new Stack<Category>();
            childs.Push(category);

            do
            {
                category = childs.Pop();

                foods = foods.Union(category.Food);

                foreach (var child in category.ChildCategory)
                    childs.Push(child);

            } while (childs.Count != 0);

            foods = foods.OrderBy(f => f.Name);

            /*
            if(Request.IsAjaxRequest())
            {
                return Json(foods.ToList());
            }
            */
            
            return View("Category", foods.ToList());
        }

        // GET: /AdminFood/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Food food = unitOfWork.FoodRepository.GetByID((int)id);
            if (food == null)
            {
                return HttpNotFound();
            }
            return View("Details", food);
        }

        // GET: /AdminFood/Create
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(unitOfWork.CategoryRepository.Get(orderBy: q => q.OrderBy(c => c.Name)), "ID", "Name");
            return View("Create");
        }

        // POST: /AdminFood/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ID,Name,Size,Pieces,CategoryID")] Food food)
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

            ViewBag.CategoryID = new SelectList(unitOfWork.CategoryRepository.Get(orderBy: q => q.OrderBy(c => c.Name)), "ID", "Name", food.CategoryID);
            return View("Create", food);
        }

        // GET: /AdminFood/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Food food = unitOfWork.FoodRepository.GetByID((int)id);
            if (food == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(unitOfWork.CategoryRepository.Get(orderBy: q => q.OrderBy(c => c.Name)), "ID", "Name", food.CategoryID);
            return View("Edit", food);
        }

        // POST: /AdminFood/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID,Name,Size,Pieces,CategoryID")] Food food)
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
            ViewBag.CategoryID = new SelectList(unitOfWork.CategoryRepository.Get(orderBy: q => q.OrderBy(c => c.Name)), "ID", "Name", food.CategoryID);
            return View("Edit", food);
        }

        // GET: /AdminFood/Delete/5
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
            Food food = unitOfWork.FoodRepository.GetByID((int)id);
            if (food == null)
            {
                return HttpNotFound();
            }
            return View("Delete", food);
        }

        // POST: /AdminFood/Delete/5
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

        // GET: /AdminFood/Ingredients/5
        public ActionResult Ingredients(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Food food = unitOfWork.FoodRepository.GetByID((int)id);
            
            if (food == null)
            {
                return HttpNotFound();
            }
            
            ViewBag.Food = food.Name;

            return View("Ingredients", unitOfWork.IngredientsForFood(food));
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
