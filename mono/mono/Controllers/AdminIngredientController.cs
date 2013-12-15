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

namespace mono.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminIngredientController : Controller
    {
        private MonoDbContext db = new MonoDbContext();

        // GET: /AdminIngredient/
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Name_desc" : "";
            ViewBag.CategorySortParm = sortOrder == "Category" ? "Category_desc" : "Category";
            ViewBag.FoodSortParm = sortOrder == "Food" ? "Food_desc" : "Food";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var ingredients = db.Ingredients.Include(i => i.Category).Include(i => i.Food);

            if (!String.IsNullOrEmpty(searchString))
            {
                ingredients = ingredients.Where(i =>
                    i.Name.ToUpper().Contains(searchString.ToUpper()) || 
                    i.Category.Name.ToUpper().Contains(searchString.ToUpper()) ||
                    i.Food.Name.ToUpper().Contains(searchString.ToUpper())
                );
            }

            switch (sortOrder)
            {
                case "Name_desc":
                    ingredients = ingredients.OrderByDescending(i => i.Name);
                    break;
                case "Category":
                    ingredients = ingredients.OrderBy(i => i.Category.Name);
                    break;
                case "Category_desc":
                    ingredients = ingredients.OrderByDescending(i => i.Category.Name);
                    break;
                case "Food":
                    ingredients = ingredients.OrderBy(i => i.Food.Name);
                    break;
                case "Food_desc":
                    ingredients = ingredients.OrderByDescending(i => i.Food.Name);
                    break;
                default:
                    ingredients = ingredients.OrderBy(i => i.Name);
                    break;
            }

            int pageSize = 3;
            int pageNumber = (page ?? 1);

            return View(ingredients.ToPagedList(pageNumber, pageSize));
        }

        // GET: /AdminIngredient/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ingredient ingredient = db.Ingredients.Find(id);
            if (ingredient == null)
            {
                return HttpNotFound();
            }
            return View(ingredient);
        }

        // GET: /AdminIngredient/Create
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "ID", "Name");
            ViewBag.FoodID = new SelectList(db.Foods, "ID", "Name");
            return View();
        }

        // POST: /AdminIngredient/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ID,Name,FoodID,CategoryID")] Ingredient ingredient)
        {
            if (ModelState.IsValid)
            {
                db.Ingredients.Add(ingredient);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "ID", "Name", ingredient.CategoryID);
            ViewBag.FoodID = new SelectList(db.Foods, "ID", "Name", ingredient.FoodID);
            return View(ingredient);
        }

        // GET: /AdminIngredient/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ingredient ingredient = db.Ingredients.Find(id);
            if (ingredient == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "ID", "Name", ingredient.CategoryID);
            ViewBag.FoodID = new SelectList(db.Foods, "ID", "Name", ingredient.FoodID);
            return View(ingredient);
        }

        // POST: /AdminIngredient/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID,Name,FoodID,CategoryID")] Ingredient ingredient)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ingredient).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "ID", "Name", ingredient.CategoryID);
            ViewBag.FoodID = new SelectList(db.Foods, "ID", "Name", ingredient.FoodID);
            return View(ingredient);
        }

        // GET: /AdminIngredient/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ingredient ingredient = db.Ingredients.Find(id);
            if (ingredient == null)
            {
                return HttpNotFound();
            }
            return View(ingredient);
        }

        // POST: /AdminIngredient/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ingredient ingredient = db.Ingredients.Find(id);
            db.Ingredients.Remove(ingredient);
            db.SaveChanges();
            return RedirectToAction("Index");
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
