using Mono.Areas.Auction.Models;
using Mono.Data;
using Mono.Model;
using Mono.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Data;

namespace Mono.Areas.Auction.Controllers
{
    [Authorize(Roles = "user")]
    public class ItemController : Controller
    {       
        private UnitOfWork unitOfWork;

        public ItemController()
        {
            unitOfWork = new UnitOfWork();
        }

        public ItemController(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        //
        // GET: /Auction/Item/
        public ActionResult Index(int? id)
        {
            ItemViewModel itemViewModel;

            if (id == null)
            {
                var subCategories = unitOfWork.CategoryRepository.Get(filter: c => c.ParentCategoryID == null, orderBy: q => q.OrderBy(c => c.Name), includeProperties: "Food");

                itemViewModel = AddCategoryFood("All", null, subCategories, new List<Food>());
            }
            else
            {
                Category category = unitOfWork.CategoryRepository.GetByID(id);

                if (category == null)
                    return HttpNotFound();

                var subCategories = category.ChildCategory.OrderBy(c => c.Name).ToList();
                var foods = category.Food.OrderBy(f => f.Name).ToList();

                itemViewModel = AddCategoryFood(category.Name, category.ParentCategoryID, subCategories, foods);
            }
            
            if(Request.IsAjaxRequest())
            {
                return PartialView("_Category", itemViewModel);
            }

            return View("Index", itemViewModel);
        }

        //
        // GET: /Auction/Add/
        public ActionResult Add(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Food food = unitOfWork.FoodRepository.GetByID(id);

            if (food == null)
            {
                return HttpNotFound();
            }

            Category category = unitOfWork.CategoryRepository.GetByID(food.CategoryID);
            ViewBag.CategorySizeID = new SelectList(unitOfWork.SizeValues(category.SizeType), "ID", "Value");
            ViewBag.IngredientsList = new MultiSelectList(unitOfWork.IngredientsForFood(food), "ID", "Name");

            FoodIngredientViewModel foodIngredientViewModel = new FoodIngredientViewModel();
            foodIngredientViewModel.FoodID = (int)id;
            foodIngredientViewModel.FoodName = food.Name;
            foodIngredientViewModel.Pieces = 1;

            return View("Add", foodIngredientViewModel);
        }

        // POST: /Auction/Item/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(FoodIngredientViewModel foodIngredientViewModel)
        {
            if (foodIngredientViewModel.FoodID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var food = unitOfWork.FoodRepository.GetByID(foodIngredientViewModel.FoodID);

            if (food == null)
            {
                return HttpNotFound();
            }

            try 
            { 
                if (ModelState.IsValid)
                {
                    FoodIngredient foodIngredient = new FoodIngredient();

                    foodIngredient.UserID = User.Identity.GetUserId();
                    foodIngredient.FoodID = (int)foodIngredientViewModel.FoodID;
                    foodIngredient.Description = foodIngredientViewModel.Description;
                    foodIngredient.CategorySizeID = foodIngredientViewModel.CategorySizeID;
                    foodIngredient.Pieces = foodIngredientViewModel.Pieces;

                    foodIngredient.Ingredients = new List<Ingredient>();

                    if (foodIngredientViewModel.Ingredients != null)
                        foreach (string stringId in foodIngredientViewModel.Ingredients)
                        {
                            int id;
                    
                            if (Int32.TryParse(stringId, out id))
                            {
                                Ingredient ingredient = unitOfWork.IngredientRepository.GetByID(id);

                                if (ingredient != null)
                                {
                                    foodIngredient.Ingredients.Add(ingredient);
                                }
                            }
                        }

                    unitOfWork.FoodIngredientRepository.Insert(foodIngredient);
                    unitOfWork.Save();

                    return RedirectToAction("Index");
                }
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                ModelState.AddModelError(string.Empty, "Unable to save changes. Try again, and if the problem persists contact your system administrator.");
            }

            Category category = unitOfWork.CategoryRepository.GetByID(food.CategoryID);

            ViewBag.CategorySizeID = new SelectList(unitOfWork.SizeValues(category.SizeType), "ID", "Value");
            ViewBag.IngredientsList = new MultiSelectList(unitOfWork.IngredientsForFood(food), "ID", "Name");

            return View("Add", foodIngredientViewModel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                unitOfWork.Dispose();
            }
            base.Dispose(disposing);
        }

        #region HelperFunctions

        private ItemViewModel AddCategoryFood(String name, int? parentCategoryID, IEnumerable<Category> subCategories, IEnumerable<Food> foods)
        {
            ItemViewModel itemViewModel = new ItemViewModel
            {
                Name = name,
                ParentCategoryID = parentCategoryID,
                ListCategoryFood = new List<ListCategoryFoodItem>()
            };

            foreach (var subcategory in subCategories)
            {
                ListCategoryFoodItem listCategoryFoodItem = new ListCategoryFoodItem
                {
                    Name = subcategory.Name,
                    ItemID = subcategory.ID,
                    Type = ListCategoryFoodItemType.Category
                };
                itemViewModel.ListCategoryFood.Add(listCategoryFoodItem);
            }

            foreach (var food in foods)
            {
                ListCategoryFoodItem listCategoryFoodItem = new ListCategoryFoodItem
                {
                    Name = food.Name,
                    ItemID = food.ID,
                    Type = ListCategoryFoodItemType.Food
                };
                itemViewModel.ListCategoryFood.Add(listCategoryFoodItem);
            }

            return itemViewModel;
        }

        #endregion
    }
}