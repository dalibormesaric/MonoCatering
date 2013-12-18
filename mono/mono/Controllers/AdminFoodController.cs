﻿using System;
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

namespace mono.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminFoodController : Controller
    {
        private IFoodRepository foodRepository;
        private ICategoryRepository categoryRepository;

        public AdminFoodController()
        {
            this.foodRepository = new FoodRepository(new MonoDbContext());
            this.categoryRepository = new CategoryRepository(new MonoDbContext());
        }

        public AdminFoodController(IFoodRepository foodRepository, ICategoryRepository categoryRepository)
        {
            this.foodRepository = foodRepository;
            this.categoryRepository = categoryRepository;
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

            var food = foodRepository.GetFoods();

            if (!String.IsNullOrEmpty(searchString))
            {
                food = food.Where(f =>
                    f.Name.ToUpper().Contains(searchString.ToUpper()) ||
                    f.Category.Name.ToUpper().Contains(searchString.ToUpper())
                );
            }

            switch (sortOrder)
            {
                case "Name_desc":
                    food = food.OrderByDescending(f => f.Name);
                    break;
                case "Category":
                    food = food.OrderBy(f => f.Category.Name);
                    break;
                case "Category_desc":
                    food = food.OrderByDescending(f => f.Category.Name);
                    break;
                default:
                    food = food.OrderBy(f => f.Name);
                    break;
            }

            int pageSize = 3;
            int pageNumber = (page ?? 1);

            return View(food.ToPagedList(pageNumber, pageSize));
        }

        // GET: /AdminCategory/Category/5
        public ActionResult Category(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Category category = categoryRepository.GetCategoryByID((int)id);

            var foods = foodRepository.GetFoodsByCategeoryID((int)id);

            if (category == null)
            {
                return HttpNotFound();
            }

            ViewBag.Category = category.Name;
          
            //???????????????
            List<int> list = new List<int>();
            List<int> listChilds = new List<int>();

            listChilds.Add(category.ID);

            do
            {
                List<int> listChildsTemp = new List<int>();

                foreach (var childID in listChilds)
                {
                    var child = categoryRepository.GetCategoryByID(childID);
                    var childs = child.ChildCategory.Select(c => c.ID).ToList();

                    list.AddRange(childs);
                    listChildsTemp.AddRange(childs);
                }

                listChilds.Clear();
                listChilds .AddRange(listChildsTemp);
            } while (listChilds.Count != 0);
            
            foreach (var c in list)
                foods = foods.Union(foodRepository.GetFoodsByCategeoryID(c));

            return View(foods.ToList());
        }

        // GET: /AdminFood/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Food food = foodRepository.GetFoodByID((int)id);
            if (food == null)
            {
                return HttpNotFound();
            }
            return View(food);
        }

        // GET: /AdminFood/Create
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(categoryRepository.GetCategories(), "ID", "Name");
            return View();
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
                    foodRepository.InsertFood(food);
                    foodRepository.Save();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                ModelState.AddModelError(string.Empty, "Unable to save changes. Try again, and if the problem persists contact your system administrator.");
            }


            ViewBag.CategoryID = new SelectList(categoryRepository.GetCategories(), "ID", "Name", food.CategoryID);
            return View(food);
        }

        // GET: /AdminFood/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Food food = foodRepository.GetFoodByID((int)id);
            if (food == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(categoryRepository.GetCategories(), "ID", "Name", food.CategoryID);
            return View(food);
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
                    foodRepository.UpdateFood(food);
                    foodRepository.Save();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                ModelState.AddModelError(string.Empty, "Unable to save changes. Try again, and if the problem persists contact your system administrator.");
            }
            ViewBag.CategoryID = new SelectList(categoryRepository.GetCategories(), "ID", "Name", food.CategoryID);
            return View(food);
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
            Food food = foodRepository.GetFoodByID((int)id);
            if (food == null)
            {
                return HttpNotFound();
            }
            return View(food);
        }

        // POST: /AdminFood/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Food food = foodRepository.GetFoodByID(id);
                foodRepository.DeleteFood(id);
                foodRepository.Save();
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
            Food food = foodRepository.GetFoodByID((int)id);
            
            if (food == null)
            {
                return HttpNotFound();
            }
            
            ViewBag.Food = food.Name;

            Category category = categoryRepository.GetCategoryByID(food.CategoryID);

            var ingredients = category.Ingredients.Union(food.Ingredients);

            while (category.ParentCategoryID != null)
            {
                category = categoryRepository.GetCategoryByID((int)category.ParentCategoryID);

                ingredients = ingredients.Union(category.Ingredients);
            }

            return View(ingredients.ToList());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foodRepository.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
